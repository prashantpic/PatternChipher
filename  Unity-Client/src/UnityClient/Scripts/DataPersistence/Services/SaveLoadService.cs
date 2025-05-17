using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine; // For JsonUtility, Application.persistentDataPath, Debug
using PatternCipher.Client.DataPersistence.Migrators; // For SaveDataMigrator
using PatternCipher.Client.DataPersistence.Models; // For PlayerProfileData (to check schema version)

namespace PatternCipher.Client.DataPersistence.Services
{
    public class SaveLoadService
    {
        private readonly SaveDataMigrator _saveDataMigrator;
        private const string CHECKSUM_SEPARATOR = "--CHECKSUM:";
        // A simple key for XOR obfuscation. KEEP THIS SECRET or use more robust encryption.
        private readonly byte[] _obfuscationKey = Encoding.UTF8.GetBytes("AFairlySecretKey123!@#"); 

        public SaveLoadService(SaveDataMigrator saveDataMigrator)
        {
            _saveDataMigrator = saveDataMigrator;
        }

        public async Task SaveAsync<T>(T data, string filename) where T : class
        {
            if (data == null || string.IsNullOrEmpty(filename))
            {
                Debug.LogError("SaveLoadService: Data or filename is null/empty.");
                return;
            }

            string path = Path.Combine(Application.persistentDataPath, filename);
            try
            {
                string jsonData = JsonUtility.ToJson(data, true);
                byte[] obfuscatedData = ObfuscateData(Encoding.UTF8.GetBytes(jsonData));
                string checksum = CalculateChecksum(obfuscatedData);
                
                string contentToWrite = Convert.ToBase64String(obfuscatedData) + CHECKSUM_SEPARATOR + checksum;

                await WriteFileAsync(path, contentToWrite);
                Debug.Log($"SaveLoadService: Data saved to {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveLoadService: Failed to save data to {path}. Error: {ex.Message}");
                throw; // Re-throw so the caller can handle it
            }
        }

        public async Task<T> LoadAsync<T>(string filename) where T : class
        {
            if (string.IsNullOrEmpty(filename))
            {
                Debug.LogError("SaveLoadService: Filename is null/empty.");
                return null;
            }

            string path = Path.Combine(Application.persistentDataPath, filename);

            if (!File.Exists(path))
            {
                Debug.Log($"SaveLoadService: File not found at {path}. Returning default for type {typeof(T).Name}.");
                return null; 
            }

            try
            {
                string content = await ReadFileAsync(path);
                
                int checksumSeparatorIndex = content.LastIndexOf(CHECKSUM_SEPARATOR);
                if (checksumSeparatorIndex == -1)
                {
                    Debug.LogError($"SaveLoadService: Checksum separator not found in file {path}. Data might be corrupted or old format.");
                    File.Delete(path); // Delete corrupted file
                    return null;
                }

                string base64Data = content.Substring(0, checksumSeparatorIndex);
                string storedChecksum = content.Substring(checksumSeparatorIndex + CHECKSUM_SEPARATOR.Length);
                
                byte[] obfuscatedData = Convert.FromBase64String(base64Data);
                string calculatedChecksum = CalculateChecksum(obfuscatedData);

                if (calculatedChecksum != storedChecksum)
                {
                    Debug.LogError($"SaveLoadService: Checksum mismatch for file {path}. Data corrupted. Deleting file.");
                    File.Delete(path); // Delete corrupted file
                    return null;
                }

                byte[] jsonDataBytes = DeobfuscateData(obfuscatedData);
                string jsonData = Encoding.UTF8.GetString(jsonDataBytes);

                T loadedData = JsonUtility.FromJson<T>(jsonData);

                // Handle data migration if applicable (example for PlayerProfileData)
                if (loadedData is PlayerProfileData profileData && _saveDataMigrator != null)
                {
                    // Assume PlayerProfileData has a SchemaVersion property and a static CurrentSchemaVersion
                    int currentVersion = PlayerProfileData.CurrentSchemaVersion; // This needs to be defined in PlayerProfileData
                    if (profileData.SchemaVersion < currentVersion)
                    {
                        Debug.Log($"SaveLoadService: Migrating data for {filename} from version {profileData.SchemaVersion} to {currentVersion}.");
                        profileData = _saveDataMigrator.Migrate(profileData, currentVersion);
                        // After migration, re-save the data in the new format
                        await SaveAsync(profileData as T, filename); // Cast back to T
                        loadedData = profileData as T;
                    }
                }
                
                Debug.Log($"SaveLoadService: Data loaded from {path}");
                return loadedData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveLoadService: Failed to load data from {path}. Error: {ex.Message}. Deleting potentially corrupted file.");
                if (File.Exists(path)) File.Delete(path); // Attempt to delete corrupted file
                return null; 
            }
        }
        
        public bool FileExists(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            string path = Path.Combine(Application.persistentDataPath, filename);
            return File.Exists(path);
        }

        public void DeleteFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return;
            string path = Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Debug.Log($"SaveLoadService: Deleted file {path}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"SaveLoadService: Error deleting file {path}: {ex.Message}");
                }
            }
        }


        private async Task WriteFileAsync(string path, string content)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(content);
            using (FileStream sourceStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        private async Task<string> ReadFileAsync(string path)
        {
            using (FileStream sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();
                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        private string CalculateChecksum(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private byte[] ObfuscateData(byte[] data)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ _obfuscationKey[i % _obfuscationKey.Length]);
            }
            return result;
        }

        private byte[] DeobfuscateData(byte[] data)
        {
            // XOR is symmetric
            return ObfuscateData(data);
        }
    }
}