using System;
using System.IO;
using System.Text;
using PatternCipher.Utilities.Logging;
using PatternCipher.Utilities.Logging.Output;

namespace PatternCipher.Utilities.Logging.Output.Internal
{
    /// <summary>
    /// Internal implementation of ILogOutput that writes log messages to a specified file. Supports log rotation and formatting.
    /// </summary>
    /// <remarks>
    /// Persists log messages to local files, aiding in troubleshooting and diagnostics, especially for client-side issues. Facilitates log packaging.
    /// Writes formatted log entries to a specified file. Implements basic log rotation based on file size and number of backup files.
    /// Note: This implementation is NOT thread-safe for concurrent writes from multiple threads/loggers without external synchronization or using a thread-safe writer like `System.IO.TextWriter` with appropriate locking. For simple Unity client use, it might suffice.
    /// </remarks>
    internal class FileLogOutput : ILogOutput, IDisposable
    {
        private readonly string _filePath;
        private readonly long _maxFileSize;
        private readonly int _maxFiles;
        private StreamWriter _writer;
        private DateTime _lastWriteTime = DateTime.MinValue;

        private readonly object _writeLock = new object(); // Simple lock for writes and rotation checks

        /// <summary>
        /// Initializes file logger with path and rotation settings.
        /// </summary>
        /// <param name="filePath">The path for the primary log file.</param>
        /// <param name="maxFileSize">Maximum size of the primary log file in bytes before rotation (default 10MB).</param>
        /// <param name="maxFiles">Maximum number of rotated log files to keep (default 5).</param>
        /// <exception cref="ArgumentNullException">Thrown if filePath is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if maxFileSize or maxFiles are invalid.</exception>
        public FileLogOutput(string filePath, long maxFileSize = 10485760, int maxFiles = 5)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "Log file path cannot be null or empty.");
            }
            if (maxFileSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFileSize), "Max file size must be positive.");
            }
             if (maxFiles < 0) // maxFiles can be 0 if no backups are desired.
            {
                throw new ArgumentOutOfRangeException(nameof(maxFiles), "Max files cannot be negative.");
            }

            _filePath = filePath;
            _maxFileSize = maxFileSize;
            _maxFiles = maxFiles;

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Open the initial file stream. Append mode.
            OpenFileStream();
        }

        /// <summary>
        /// Writes a log entry to the configured log file. Performs log rotation if necessary.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="categoryName">The logger category name.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An optional exception.</param>
        /// <param name="eventInfo">Optional structured properties.</param>
        public void WriteLog(LogLevel level, string categoryName, string message, Exception exception, LogEventInfo eventInfo)
        {
            lock (_writeLock)
            {
                // Perform rotation check *before* writing
                CheckForRotation();
                
                // If writer is null (e.g., failed to open), attempt to reopen.
                if (_writer == null)
                {
                    OpenFileStream();
                    if (_writer == null) // Still null, cannot write.
                    {
                         System.Console.WriteLine($"ERROR: Log writer for '{_filePath}' is not available. Log: {message}");
                         return;
                    }
                }


                // Format the log message (similar to ConsoleLogOutput for consistency)
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff UTC");
                StringBuilder logEntry = new StringBuilder();
                logEntry.Append($"{timestamp} [{level.ToString().ToUpper()}] ({categoryName}) - {message}");

                // Append exception details if present
                if (exception != null)
                {
                    logEntry.AppendLine();
                    logEntry.Append($"Exception: {exception.GetType().FullName}: {exception.Message}");
                    logEntry.AppendLine();
                    logEntry.Append($"StackTrace: {exception.StackTrace}");
                     // Optionally include inner exceptions
                    if (exception.InnerException != null)
                    {
                        logEntry.AppendLine();
                        logEntry.Append($"Inner Exception: {exception.InnerException.GetType().FullName}: {exception.InnerException.Message}");
                    }
                }

                // Append event info properties if present
                if (eventInfo != null && eventInfo.Properties != null && eventInfo.Properties.Count > 0)
                {
                    logEntry.AppendLine();
                    logEntry.Append("Properties:");
                    foreach(var pair in eventInfo.Properties)
                    {
                         logEntry.AppendLine();
                         logEntry.Append($"  {pair.Key}: {pair.Value}");
                    }
                }

                try
                {
                     _writer.WriteLine(logEntry.ToString());
                     _writer.Flush(); // Ensure data is written to disk immediately
                     _lastWriteTime = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // If file writing fails, what do we do?
                    // Log to console as a fallback? Ignore?
                    // For robustness, maybe try console fallback here.
                    System.Console.WriteLine($"ERROR: Failed to write log to file '{_filePath}'. Log: {logEntry.ToString()}");
                    System.Console.WriteLine($"Exception: {ex}");
                    // Optionally, close the writer to attempt reopening on next log
                    CloseFileStream();
                }
            }
        }

        /// <summary>
        /// Checks if log rotation is needed based on file size and performs rotation.
        /// </summary>
        private void CheckForRotation()
        {
             if (_maxFiles <= 0) // If maxFiles is 0, rotation to backups is disabled, but current file might still be cleared or truncated.
                               // For simplicity, if _maxFiles is 0, we don't rotate to backups, just keep writing to the main file, capped by _maxFileSize.
                               // A more advanced implementation might truncate the main file. Current behavior: main file grows indefinitely if _maxFiles is 0.
                               // Based on SDS: "keeps up to maxFiles backups", so if maxFiles is 0, no backups.
             {
                 // If _maxFiles is 0, we might still want to check if the primary file exceeds _maxFileSize
                 // and potentially clear it or stop writing. For this version, if _maxFiles is 0, rotation to backups
                 // doesn't happen. The primary file could grow beyond _maxFileSize.
                 // Let's refine this: if _maxFiles is 0, and file exceeds size, we could just reopen (truncate) the main file.
                 // However, the SDS says "keeps up to maxFiles backups", implying rotation is tied to backups.
                 // If _maxFiles is 0, the "keeps up to" part is moot. Let's assume for now that if _maxFiles is 0,
                 // we don't rotate to backups, and the main file size is only checked if _maxFiles > 0.
                 // This could be clarified. Let's stick to "backup" based rotation.
                 if (_maxFiles == 0) return;
             }

            try
            {
                FileInfo fileInfo = new FileInfo(_filePath);
                if (fileInfo.Exists && fileInfo.Length >= _maxFileSize)
                {
                    PerformRotation();
                }
            }
            catch (Exception ex)
            {
                 // Handle potential errors during file check or rotation (e.g., permissions)
                 System.Console.WriteLine($"ERROR: Log file rotation check failed for '{_filePath}'. Exception: {ex}");
            }
        }

        /// <summary>
        /// Performs the log file rotation process.
        /// </summary>
        private void PerformRotation()
        {
            // Close the current file stream
            CloseFileStream();

            string baseName = Path.GetFileNameWithoutExtension(_filePath);
            string extension = Path.GetExtension(_filePath);
            string directory = Path.GetDirectoryName(_filePath);

            // Delete the oldest file(s) if maxFiles is reached
            // Files are named like: log.txt.N-1, log.txt.N-2, ..., log.txt.0
            string oldestFilePath = Path.Combine(directory, $"{baseName}{extension}.{_maxFiles - 1}");
            if (File.Exists(oldestFilePath))
            {
                try { File.Delete(oldestFilePath); } catch (Exception ex) { System.Console.WriteLine($"ERROR: Could not delete oldest log file '{oldestFilePath}'. Exception: {ex}"); }
            }


            // Shift existing backup files: log.txt.i -> log.txt.i+1
            for (int i = _maxFiles - 2; i >= 0; i--)
            {
                string currentRotatedPath = Path.Combine(directory, $"{baseName}{extension}.{i}");
                if (File.Exists(currentRotatedPath))
                {
                    string newRotatedPath = Path.Combine(directory, $"{baseName}{extension}.{i + 1}");
                    try { File.Move(currentRotatedPath, newRotatedPath); } catch (Exception ex) { System.Console.WriteLine($"ERROR: Could not move log file '{currentRotatedPath}' to '{newRotatedPath}'. Exception: {ex}"); }
                }
            }

            // Move the current file to the first backup index (e.g., log.txt -> log.txt.0)
             if (File.Exists(_filePath))
            {
                string firstRotatedPath = Path.Combine(directory, $"{baseName}{extension}.0");
                 try { File.Move(_filePath, firstRotatedPath); } catch (Exception ex) { System.Console.WriteLine($"ERROR: Could not move current log file '{_filePath}' to '{firstRotatedPath}'. Exception: {ex}"); }
            }

            // Open a new file stream for the primary log file
            OpenFileStream();
        }

        /// <summary>
        /// Opens the file stream for writing (append mode).
        /// </summary>
        private void OpenFileStream()
        {
            try
            {
                 // Use FileShare.ReadWrite to allow other processes to read the log file while we're writing
                var fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                _writer = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = false }; // Manual flush in WriteLog
            }
            catch (Exception ex)
            {
                 System.Console.WriteLine($"ERROR: Failed to open log file stream for '{_filePath}'. Exception: {ex}");
                 // Set writer to null so WriteLog knows it failed
                 _writer = null;
            }
        }

        /// <summary>
        /// Closes the current file stream writer.
        /// </summary>
        private void CloseFileStream()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.Flush(); // Final flush
                    _writer.Dispose(); // Dispose handles Close
                }
                catch (Exception ex)
                {
                     System.Console.WriteLine($"ERROR: Failed to close log file stream for '{_filePath}'. Exception: {ex}");
                }
                finally
                {
                    _writer = null;
                }
            }
        }

        /// <summary>
        /// Disposes the file writer resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_writeLock)
                {
                    CloseFileStream();
                }
            }
        }

        ~FileLogOutput()
        {
            Dispose(false);
        }
    }
}