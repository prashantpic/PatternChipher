#!/bin/bash
set -euxo pipefail

# Purpose: Execute tests for Firebase Cloud Functions.
# Inputs (expected as environment variables):
#   FIREBASE_FUNCTIONS_DIR: Path to the Firebase functions source directory (within REPO-FIREBASE-BACKEND).

echo "Starting Firebase functions tests..."

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

echo "Verifying package.json exists..."
if [[ ! -f "package.json" ]]; then
    echo "Error: package.json not found in ${FIREBASE_FUNCTIONS_DIR}"
    exit 1
fi

# Ensure devDependencies (which include testing frameworks) are installed
# This might have been done by build-firebase-functions.sh if it used `npm ci` or `npm install`.
# Running it again to be sure, or if tests are run independently of build.
# Consider if `npm ci` is more appropriate if `package-lock.json` is present.
echo "Ensuring development dependencies are installed..."
if [[ -f "package-lock.json" ]]; then
    npm ci
else
    npm install
fi


# Check for test scripts in package.json
TEST_COMMAND=""
if grep -q '"test:ci":' package.json; then
    TEST_COMMAND="npm run test:ci"
    echo "Found 'test:ci' script. Using: ${TEST_COMMAND}"
elif grep -q '"test":' package.json; then
    TEST_COMMAND="npm test"
    echo "Found 'test' script. Using: ${TEST_COMMAND}"
else
    echo "Error: No 'test' or 'test:ci' script found in package.json in ${FIREBASE_FUNCTIONS_DIR}"
    exit 1
fi

# If Firebase Emulator Suite is needed, it should be started by the calling workflow
# before this script is executed. This script assumes emulators are running if tests require them.
# Example: firebase emulators:exec "${TEST_COMMAND}"
# For simplicity, we'll directly run the test command here. If emulators are needed,
# the test script in package.json should handle their lifecycle or expect them to be running.

echo "Executing Firebase functions tests: ${TEST_COMMAND}"
${TEST_COMMAND}

TEST_EXIT_CODE=$?

if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo "Firebase functions tests passed."
else
    echo "Firebase functions tests failed. Exit code: $TEST_EXIT_CODE"
    # Test runners usually output detailed reports to stdout/stderr.
    exit $TEST_EXIT_CODE
fi

echo "Firebase functions test script finished."