import * as functionsLogger from "firebase-functions/logger";

export const info = (message: string, metadata?: object): void => {
  functionsLogger.info(message, metadata);
};

export const warn = (message: string, metadata?: object): void => {
  functionsLogger.warn(message, metadata);
};

export const error = (message: string, error?: Error, metadata?: object): void => {
  const logData = { ...metadata, stack: error?.stack };
  functionsLogger.error(message, logData);
};