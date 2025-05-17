#!/bin/bash
set -euxo pipefail

# Purpose: Deploy specified Firebase components using Firebase CLI.
# Inputs:
#   $1 (FIREBASE_PROJECT_ALIAS): Firebase project alias (e.g., dev, staging, production) as defined in .firebaserc.
#   $2 (DEPLOY_TARGETS): Comma-separated list of components to deploy (e.g., functions,firestore:rules,storage:rules).
#   $3 (FIREBASE_IAC_PATH): Path to the Firebase Infrastructure-as-Code directory where firebase.json resides.
# Assumes Firebase CLI (firebase-tools) is installed and authenticated (e.g., via setup-firebase-tools.sh using service account).

echo "Starting Firebase component deployment..."

FIREBASE_PROJECT_ALIAS="${1}"
DEPLOY_TARGETS="${2}"
FIREBASE_IAC_PATH="${3}"

if [[ -z "${FIREBASE_PROJECT_ALIAS}" ]]; then
    echo "Error: Firebase project alias (argument 1) is not provided."
    exit 1
fi
if [[ -z "${DEPLOY_TARGETS}" ]]; then
    echo "Error: Deployment targets (argument 2) are not provided."
    exit 1
fi
if [[ -z "${FIREBASE_IAC_PATH}" ]]; then
    echo "Error: Firebase IaC path (argument 3) is not provided."
    exit 1
fi

echo "Firebase Project Alias: ${FIREBASE_PROJECT_ALIAS}"
echo "Deployment Targets: ${DEPLOY_TARGETS}"
echo "Firebase IaC Path: ${FIREBASE_IAC_PATH}"

if [[ ! -d "${FIREBASE_IAC_PATH}" ]]; then
    echo "Error: Firebase IaC directory does not exist: ${FIREBASE_IAC_PATH}"
    exit 1
fi

echo "Navigating to Firebase IaC directory: ${FIREBASE_IAC_PATH}"
cd "${FIREBASE_IAC_PATH}"

echo "Verifying firebase.json exists..."
if [[ ! -f "firebase.json" ]]; then
    echo "Error: firebase.json not found in ${FIREBASE_IAC_PATH}"
    exit 1
fi

# Construct Firebase CLI command
# The --token flag might not be needed if GOOGLE_APPLICATION_CREDENTIALS is set correctly
# by setup-firebase-tools.sh. The CLI will automatically pick up the service account.
# If FIREBASE_TOKEN is set, it will be used.
# Using --non-interactive for CI environments.
# Using --debug can provide more verbose output if needed.
FIREBASE_DEPLOY_COMMAND="firebase deploy --project ${FIREBASE_PROJECT_ALIAS} --only ${DEPLOY_TARGETS} --non-interactive --force"

# Add --debug for more verbose logging if needed for troubleshooting
# FIREBASE_DEPLOY_COMMAND="${FIREBASE_DEPLOY_COMMAND} --debug"

echo "Executing Firebase deploy command: ${FIREBASE_DEPLOY_COMMAND}"
${FIREBASE_DEPLOY_COMMAND}

DEPLOY_EXIT_CODE=$?

if [ $DEPLOY_EXIT_CODE -eq 0 ]; then
    echo "Firebase deployment successful for project ${FIREBASE_PROJECT_ALIAS}, targets ${DEPLOY_TARGETS}."
else
    echo "Firebase deployment failed. Exit code: $DEPLOY_EXIT_CODE"
    # Firebase CLI usually provides detailed error messages.
    exit $DEPLOY_EXIT_CODE
fi

echo "Firebase component deployment script finished."