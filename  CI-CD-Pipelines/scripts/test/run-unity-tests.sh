#!/bin/bash
set -euxo pipefail

# Purpose: Execute Unity tests (EditMode or PlayMode).
# Inputs (expected as environment variables):
#   UNITY_EXECUTABLE_PATH: Path to the Unity executable.
#   PROJECT_PATH: Path to the Unity project (REPO-UNITY-CLIENT).
#   TEST_PLATFORM: The platform to run tests on (e.g., editmode, playmode).
#   TEST_RESULTS_PATH: Path where the NUnit XML test results file will be saved.

echo "Starting Unity tests..."
echo "Unity Executable Path: ${UNITY_EXECUTABLE_PATH}"
echo "Project Path: ${PROJECT_PATH}"
echo "Test Platform: ${TEST_PLATFORM}"
echo "Test Results Path: ${TEST_RESULTS_PATH}"

# Ensure the directory for test results exists
mkdir -p "$(dirname "${TEST_RESULTS_PATH}")"
LOG_FILE="${PROJECT_PATH}/Logs/unity_tests_${TEST_PLATFORM}.log"
mkdir -p "$(dirname "${LOG_FILE}")"


# Construct Unity command line arguments
UNITY_ARGS=(
    -batchmode
    -projectPath "${PROJECT_PATH}"
    -runTests
    -testPlatform "${TEST_PLATFORM}"
    -testResults "${TEST_RESULTS_PATH}"
    -logFile "${LOG_FILE}"
    # -quit argument is implicitly handled by -runTests in recent Unity versions,
    # but can be added if issues arise.
    # Adding -nographics can be useful for CI servers if no GPU is available,
    # but might affect some PlayMode tests.
    # -nographics
)

echo "Executing Unity test command..."
"${UNITY_EXECUTABLE_PATH}" "${UNITY_ARGS[@]}"

TEST_EXIT_CODE=$?

# Unity's exit codes for tests:
# 0: All tests passed
# 2: Tests ran, some failed
# 3: Tests ran, some failed, some inconclusive
# Other non-zero: Error during test execution itself (e.g., compilation error, license issue)

if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo "Unity tests passed for platform: ${TEST_PLATFORM}."
elif [ $TEST_EXIT_CODE -eq 2 ] || [ $TEST_EXIT_CODE -eq 3 ]; then
    echo "Unity tests completed with failures or inconclusive tests for platform: ${TEST_PLATFORM}. Exit code: $TEST_EXIT_CODE"
    echo "Test results are available at ${TEST_RESULTS_PATH}"
    echo "Check logs at ${LOG_FILE}"
    cat "${LOG_FILE}" || echo "Failed to cat log file."
    # We still exit with the specific code from Unity to signal test failures to CI.
    exit $TEST_EXIT_CODE
else
    echo "Unity tests failed to execute or an error occurred for platform: ${TEST_PLATFORM}. Exit code: $TEST_EXIT_CODE"
    echo "Test results might not be available or complete."
    echo "Check logs at ${LOG_FILE}"
    cat "${LOG_FILE}" || echo "Failed to cat log file."
    exit $TEST_EXIT_CODE
fi

echo "Unity tests script finished for platform: ${TEST_PLATFORM}."