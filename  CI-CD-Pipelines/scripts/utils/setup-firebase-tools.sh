#!/bin/bash
set -euxo pipefail

# Purpose: Install and configure Firebase CLI (firebase-tools) for CI/CD operations.
# Environment Variables (must be set by the calling workflow):
#   FIREBASE_SERVICE_ACCOUNT_SECRET: (Preferred) Base64 encoded JSON content of the Firebase service account key.
#   OR (Alternative if service account is not used for some reason, or for specific commands)
#   FIREBASE_TOKEN_SECRET: Firebase CI token (obtained via `firebase login:ci`).
#   FIREBASE_CLI_VERSION: (Optional) Specific version of firebase-tools to install (e.g., "latest" or "11.0.0"). Defaults to "latest".

echo "Setting up Firebase CLI (firebase-tools)..."

FIREBASE_CLI_VERSION_TO_INSTALL="${FIREBASE_CLI_VERSION:-latest}"

echo "Installing firebase-tools@${FIREBASE_CLI_VERSION_TO_INSTALL} globally via npm..."
if ! npm install -g "firebase-tools@${FIREBASE_CLI_VERSION_TO_INSTALL}"; then
    echo "Error: Failed to install firebase-tools."
    exit 1
fi
echo "firebase-tools installed successfully."

# Verify installation
firebase --version

# Authentication
if [[ -n "${FIREBASE_SERVICE_ACCOUNT_SECRET}" ]]; then
    echo "Authenticating Firebase CLI using Service Account (FIREBASE_SERVICE_ACCOUNT_SECRET)..."
    
    # Create a temporary file for the service account key
    # Ensure this temp file is cleaned up, e.g., by the CI runner workspace cleanup
    # or explicitly in a post-job step if needed. GitHub Actions runners usually clean workspaces.
    SERVICE_ACCOUNT_KEY_PATH="/tmp/firebase-service-account-key.json"
    
    echo "${FIREBASE_SERVICE_ACCOUNT_SECRET}" | base64 -d > "${SERVICE_ACCOUNT_KEY_PATH}"
    if [[ ! -s "${SERVICE_ACCOUNT_KEY_PATH}" ]]; then
        echo "Error: Decoded service account key file is empty. Check FIREBASE_SERVICE_ACCOUNT_SECRET."
        rm -f "${SERVICE_ACCOUNT_KEY_PATH}"
        exit 1
    fi
    
    # Export environment variable for Firebase CLI and Google Cloud SDKs to use
    export GOOGLE_APPLICATION_CREDENTIALS="${SERVICE_ACCOUNT_KEY_PATH}"
    echo "GOOGLE_APPLICATION_CREDENTIALS environment variable set to ${SERVICE_ACCOUNT_KEY_PATH}"
    echo "Firebase CLI will use this service account for authentication."

elif [[ -n "${FIREBASE_TOKEN_SECRET}" ]]; then
    echo "Authenticating Firebase CLI using CI Token (FIREBASE_TOKEN_SECRET)..."
    export FIREBASE_TOKEN="${FIREBASE_TOKEN_SECRET}"
    echo "FIREBASE_TOKEN environment variable set."
    echo "Firebase CLI will use this token for authentication."
else
    echo "Warning: Neither FIREBASE_SERVICE_ACCOUNT_SECRET nor FIREBASE_TOKEN_SECRET is set."
    echo "Firebase CLI might not be authenticated for operations requiring it."
    # Depending on the workflow, this might be acceptable for some commands (e.g., `firebase emulators:start`)
    # but not for `deploy` or `projects:list`.
fi

# Optional: Verify authentication by listing projects (requires appropriate permissions)
echo "Verifying Firebase CLI access (listing projects)..."
if firebase projects:list; then
    echo "Firebase CLI authenticated and able to list projects."
else
    AUTH_VERIFY_EXIT_CODE=$?
    echo "Warning: Failed to list Firebase projects. Exit code: ${AUTH_VERIFY_EXIT_CODE}."
    echo "This might indicate an authentication issue or insufficient permissions for the authenticated principal."
    # Do not exit here, as some workflows might not need full project listing capabilities.
fi

echo "Firebase CLI setup script finished."
# Note: If GOOGLE_APPLICATION_CREDENTIALS was set using a temp file,
# that file (/tmp/firebase-service-account-key.json) will persist for the duration of the job.
# GitHub Actions runners typically clean the workspace and /tmp between jobs.