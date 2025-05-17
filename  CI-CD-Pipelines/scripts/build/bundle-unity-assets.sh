#!/bin/bash
set -euxo pipefail

# Purpose: Build Unity Addressable Asset Bundles.
# Inputs (expected as environment variables):
#   UNITY_EXECUTABLE_PATH: Path to the Unity executable.
#   PROJECT_PATH: Path to the Unity project (REPO-UNITY-CLIENT).
#   ADDRESSABLES_PROFILE_NAME: (Optional) Name of the Addressables profile to use for the build.
#   UNITY_BUNDLE_METHOD: (Optional) Custom C# build method for Addressables, defaults to CiBuilder.BuildAddressables.

echo "Starting Unity Addressable Asset Bundles build..."
echo "Unity Executable Path: ${UNITY_EXECUTABLE_PATH}"
echo "Project Path: ${PROJECT_PATH}"

if [[ -n "${ADDRESSABLES_PROFILE_NAME}" ]]; then
    echo "Addressables Profile Name: ${ADDRESSABLES_PROFILE_NAME}"
fi

LOG_FILE="${PROJECT_PATH}/Logs/unity_addressables_build.log"
mkdir -p "$(dirname "${LOG_FILE}")"

# Construct Unity command line arguments
UNITY_ARGS=(
    -batchmode
    -quit
    -projectPath "${PROJECT_PATH}"
    -logFile "${LOG_FILE}"
    -executeMethod "${UNITY_BUNDLE_METHOD:-CiBuilder.BuildAddressables}"
)

if [[ -n "${ADDRESSABLES_PROFILE_NAME}" ]]; then
    UNITY_ARGS+=(+addressablesProfileName "${ADDRESSABLES_PROFILE_NAME}")
fi

echo "Executing Unity Addressables build command..."
"${UNITY_EXECUTABLE_PATH}" "${UNITY_ARGS[@]}"

BUILD_EXIT_CODE=$?

if [ $BUILD_EXIT_CODE -eq 0 ]; then
    echo "Unity Addressables build successful."
    echo "Bundles should be in the project's configured Addressables build path (e.g., ServerData/<target>)."
else
    echo "Unity Addressables build failed. Exit code: $BUILD_EXIT_CODE"
    echo "Check logs at ${LOG_FILE}"
    cat "${LOG_FILE}" || echo "Failed to cat log file."
    exit $BUILD_EXIT_CODE
fi

echo "Unity Addressable Asset Bundles build script finished."