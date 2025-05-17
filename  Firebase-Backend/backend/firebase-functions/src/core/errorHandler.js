/**
 * @file errorHandler.js
 * @summary Centralized error handling logic for Cloud Functions. Converts exceptions into consistent error responses or logs.
 * @description Provides centralized error handling utilities for Cloud Functions, standardizing error responses and logging.
 * This ensures consistent error handling across all functions and improving debuggability.
 * Namespace: PatternCipher.Backend.Core
 * @version 1.0.0
 * @since 2024-07-16
 */

const functions = require('firebase-functions');
const logger = require('./logger');

/**
 * Converts a standard Error or specific error types into a standardized `functions.https.HttpsError`.
 * @param {Error} error - The error to convert.
 * @returns {functions.https.HttpsError} A Firebase HTTPS error.
 */
function toHttpsError(error) {
  if (error instanceof functions.https.HttpsError) {
    return error;
  }

  // Default error code
  let code = 'internal';
  let message = 'An unexpected error occurred.';

  // Customize based on error type or properties
  if (error.name === 'ValidationError') { // Example custom error
    code = 'invalid-argument';
    message = error.message || 'Invalid input data.';
  } else if (error.name === 'RateLimitError') { // Example custom error
    code = 'resource-exhausted';
    message = error.message || 'Request limit exceeded. Please try again later.';
  } else if (error.message) {
      message = error.message;
  }
  // Add more specific error type checks as needed

  // Log the original error internally for debugging, regardless of what's sent to client
  logger.error(`Converting to HttpsError: ${message}`, error, { originalErrorName: error.name });

  return new functions.https.HttpsError(code, message, {
    originalError: error.name, // Optionally include some non-sensitive detail
  });
}

/**
 * Handles errors for HTTPS Callable Functions.
 * Logs the error and sends a standardized HttpsError response to the client.
 * @param {Error} error - The error object.
 * @param {functions.Response} [response] - DEPRECATED for Callable Functions. The HttpsError is thrown.
 *                                        This parameter is kept for potential future use with HTTP onRequest functions.
 *                                        For Callable Functions, just throw the HttpsError.
 * @throws {functions.https.HttpsError} - Throws an HttpsError to be sent to the client.
 */
function handleHttpsError(error, response) { // Response parameter is mostly illustrative for non-Callable HTTPS
  const httpsError = toHttpsError(error);
  // For Callable Functions, we typically throw the HttpsError.
  // The 'response' object is not directly used by Callable functions in the same way as onRequest.
  // Logging is done by toHttpsError or can be enhanced here.
  logger.error(`HTTPS Error Handled: ${httpsError.message}`, error, {
    code: httpsError.code,
    details: httpsError.details
  });

  // If this were an onRequest function, you might do:
  // if (response) {
  //   response.status(httpsError.httpErrorCode.status).send({ error: httpsError.toJSON() });
  //   return;
  // }

  // For Callable functions, throwing the error is the correct pattern.
  throw httpsError;
}

/**
 * Handles errors for background Cloud Functions (e.g., Firestore triggers, Auth triggers).
 * Logs the error comprehensively.
 * @param {Error} error - The error object.
 * @param {object} [context] - Optional. The event context for the background function.
 * @returns {void}
 */
function handleBackgroundError(error, context) {
  const metadata = {
    functionName: process.env.FUNCTION_NAME || 'unknown_function', // available in Cloud Functions env
    ...(context ? { eventId: context.eventId, eventType: context.eventType, resource: context.resource } : {}),
  };
  logger.error(`Background Function Error: ${error.message}`, error, metadata);
  // For background functions, you typically log and then either let the function retry (if applicable)
  // or acknowledge completion to prevent infinite retries for non-recoverable errors.
  // Depending on the trigger type and error, you might return a specific value or Promise.
  // For many background functions, simply logging and allowing the error to propagate (if not caught)
  // is sufficient for Firebase to handle retries or mark as failed.
}


module.exports = {
  handleHttpsError,
  handleBackgroundError,
  toHttpsError,
};