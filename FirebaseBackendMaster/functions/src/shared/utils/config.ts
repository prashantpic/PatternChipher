import * as functions from "firebase-functions";

/**
 * Provides a typed, centralized interface for accessing environment variables
 * and other configuration data for the backend services.
 * 
 * Secrets and configuration should be set using the Firebase CLI:
 * `firebase functions:config:set some.service.key="VALUE"`
 * 
 * This prevents hardcoding secrets in the source code.
 */
export const config = functions.config();