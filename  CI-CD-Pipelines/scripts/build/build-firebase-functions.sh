#!/bin/bash
set -euxo pipefail

# Purpose: Build and package Firebase Cloud Functions.
# Inputs (expected as environment variables):
#   FIREBASE_FUNCTIONS_DIR: Path to the Firebase functions source directory (within REPO-FIREBASE-BACKEND).

echo "Starting Firebase functions build..."

if [[ -z "${FIREBASE_FUNCTIONS_DIR}" ]]; then
    echo "Error: FIREBASE_FUNCTIONS_DIR environment variable is not set."
    exit 1
fi

if [[ ! -d "${FIREBASE_FUNCTIONS_DIR}" ]]; then
    echo "Error: Firebase functions directory does not exist: ${FIREBASE_FUNCTIONS_DIR}"
    exit 1
fi

echo "Navigating to Firebase functions directory: ${FIREBASE_FUNCTIONS_DIR}"
cd "${FIREBASE_FUNCTIONS_DIR}"

echo "Verifying package.json and package-lock.json exist..."
if [[ ! -f "package.json" ]]; then
    echo "Error: package.json not found in ${FIREBASE_FUNCTIONS_DIR}"
    exit 1
fi
if [[ ! -f "package-lock.json" ]]; then
    echo "Warning: package-lock.json not found in ${FIREBASE_FUNCTIONS_DIR}. Using npm install instead of npm ci."
    NPM_COMMAND="install"
else
    NPM_COMMAND="ci"
fi

echo "Installing dependencies using 'npm ${NPM_COMMAND}'..."
npm "${NPM_COMMAND}"

# Check if a build script exists in package.json (common for TypeScript projects)
echo "Checking for build script in package.json..."
if grep -q '"build":' package.json; then
    echo "Build script found. Running 'npm run build'..."
    npm run build
else
    echo "No 'build' script found in package.json. Assuming JavaScript project or build is handled differently."
fi

echo "Firebase functions build process finished."
# The output is the current directory (FIREBASE_FUNCTIONS_DIR) ready for deployment.