/**
 * @file logger.js
 * @summary Logging utility module. Provides standardized methods for emitting logs at various severity levels.
 * @description Standardizes application logging, integrating with Google Cloud Logging to provide structured,
 * searchable logs for monitoring, debugging, and auditing critical backend operations.
 * REQ-9-014
 * Namespace: PatternCipher.Backend.Core
 * @version 1.0.0
 * @since 2024-07-16
 */

const { Logging } = require('@google-cloud/logging');

// Creates a client
const logging = new Logging();

// Selects the log to write to
// Configurable via environment variable or constants file
const LOG_NAME = process.env.LOG_NAME || 'patterncipher-backend-log';
const log = logging.log(LOG_NAME);

// Helper to create a structured log entry
const makeEntry = (severity, message, metadata) => {
  // The metadata or payload for the log entry.
  const payload = {
    message,
    serviceContext: {
      service: process.env.K_SERVICE || 'unknown-service', // For Cloud Run/Functions
      version: process.env.K_REVISION || 'unknown-version',
    },
    ...metadata,
  };
  return log.entry({ severity, ...payload.serviceContext }, payload); // GCP expects resource for serviceContext
};


/**
 * Logs an informational message.
 * @param {string} message - The log message.
 * @param {object} [metadata] - Optional. Additional structured data to log.
 */
function info(message, metadata) {
  const entry = makeEntry('INFO', message, metadata);
  log.write(entry).catch(err => console.error('Failed to write INFO log:', err, message, metadata));
}

/**
 * Logs a warning message.
 * @param {string} message - The log message.
 * @param {object} [metadata] - Optional. Additional structured data to log.
 */
function warn(message, metadata) {
  const entry = makeEntry('WARNING', message, metadata);
  log.write(entry).catch(err => console.error('Failed to write WARNING log:', err, message, metadata));
}

/**
 * Logs an error message.
 * @param {string} message - The log message.
 * @param {Error} [error] - Optional. The error object.
 * @param {object} [metadata] - Optional. Additional structured data to log.
 */
function error(message, error, metadata) {
  let fullMetadata = { ...metadata };
  if (error instanceof Error) {
    fullMetadata = {
      ...fullMetadata,
      errorMessage: error.message,
      errorStack: error.stack,
      errorName: error.name,
    };
  } else if (error) { // If 'error' is not an Error object but something else was passed
    fullMetadata = {
      ...fullMetadata,
      errorDetails: error,
    };
  }
  const entry = makeEntry('ERROR', message, fullMetadata);
  log.write(entry).catch(err => console.error('Failed to write ERROR log:', err, message, fullMetadata));
}

/**
 * Logs a debug message.
 * Debug logs might be filtered out in production environments by default.
 * @param {string} message - The log message.
 * @param {object} [metadata] - Optional. Additional structured data to log.
 */
function debug(message, metadata) {
  // In Google Cloud Logging, 'DEBUG' is a standard severity.
  // Consider environment-based filtering if DEBUG logs are too verbose for production.
  if (process.env.NODE_ENV !== 'production' || process.env.LOG_LEVEL === 'DEBUG') {
    const entry = makeEntry('DEBUG', message, metadata);
    log.write(entry).catch(err => console.error('Failed to write DEBUG log:', err, message, metadata));
  }
}

module.exports = {
  info,
  warn,
  error,
  debug,
};