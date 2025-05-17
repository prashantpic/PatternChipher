#!/bin/bash
set -euxo pipefail

# Purpose: Execute Unity command-line build for specified target platform.
# Inputs (expected as environment variables):
#   UNITY_EXECUTABLE_PATH: Path to the Unity executable.
#   PROJECT_PATH: Path to the Unity project (REPO-UNITY-CLIENT).
#   BUILD_TARGET: Target platform (e.g., Android, iOS, StandaloneWindows64).
#   BUILD_CONFIG: Build configuration (e.g., Release, Debug).
#   OUTPUT_PATH: Directory where the build output will be placed.
#   OUTPUT_NAME: Name of the output build file/folder.
#   BUILD_VERSION: Version string for the build.
#   BUILD_NUMBER: Build number (integer).
#   ANDROID_KEYSTORE_BASE64: (Optional) Base64 encoded keystore file for Android.
#   ANDROID_KEYSTORE_PASS: (Optional) Password for the Android keystore.
#   ANDROID_KEYALIAS_NAME: (Optional) Alias name for the Android key.
#   ANDROID_KEYALIAS_PASS: (Optional) Password for the Android key alias.
#   UNITY_BUILD_METHOD: (Optional) Custom C# build method, defaults to CiBuilder.PerformBuild.

echo "Starting Unity client build..."
echo "Unity Executable Path: ${UNITY_EXECUTABLE_PATH}"
echo "Project Path: ${PROJECT_PATH}"
echo "Build Target: ${BUILD_TARGET}"
echo "Build Configuration: ${BUILD_CONFIG}"
echo "Output Path: ${OUTPUT_PATH}"
echo "Output Name: ${OUTPUT_NAME}"
echo "Build Version: ${BUILD_VERSION}"
echo "Build Number: ${BUILD_NUMBER}"

# Ensure output directory exists
mkdir -p "${OUTPUT_PATH}"
LOG_FILE="${OUTPUT_PATH}/unity_build.log"

# Construct Unity command line arguments
UNITY_ARGS=(
    -batchmode
    -quit
    -projectPath "${PROJECT_PATH}"
    -logFile "${LOG_FILE}"
    -buildTarget "${BUILD_TARGET}"
    -executeMethod "${UNITY_BUILD_METHOD:-CiBuilder.PerformBuild}"
    +buildConfig "${BUILD_CONFIG}"
    +outputPath "${OUTPUT_PATH}/${OUTPUT_NAME}"
    +buildVersion "${BUILD_VERSION}"
    +buildNumber "${BUILD_NUMBER}"
)

# Add Android signing arguments if provided
if [[ "${BUILD_TARGET}" == "Android" ]]; then
    if [[ -n "${ANDROID_KEYSTORE_BASE64}" && -n "${ANDROID_KEYSTORE_PASS}" && -n "${ANDROID_KEYALIAS_NAME}" && -n "${ANDROID_KEYALIAS_PASS}" ]]; then
        echo "Android signing information provided. Decoding keystore..."
        KEYSTORE_PATH="${PROJECT_PATH}/Temp/signing.keystore"
        mkdir -p "$(dirname "${KEYSTORE_PATH}")"
        echo "${ANDROID_KEYSTORE_BASE64}" | base64 -d > "${KEYSTORE_PATH}"

        UNITY_ARGS+=(
            +androidKeystoreName "${KEYSTORE_PATH}"
            +androidKeystorePass "${ANDROID_KEYSTORE_PASS}"
            +androidKeyaliasName "${ANDROID_KEYALIAS_NAME}"
            +androidKeyaliasPass "${ANDROID_KEYALIAS_PASS}"
        )
        echo "Using keystore for Android build: ${KEYSTORE_PATH}"
    else
        echo "Android signing information not fully provided. Build may not be signable for release."
    fi
fi

echo "Executing Unity build command..."
"${UNITY_EXECUTABLE_PATH}" "${UNITY_ARGS[@]}"

BUILD_EXIT_CODE=$?

if [ $BUILD_EXIT_CODE -eq 0 ]; then
    echo "Unity build successful."
else
    echo "Unity build failed. Exit code: $BUILD_EXIT_CODE"
    echo "Check logs at ${LOG_FILE}"
    # Cat log for easier debugging in CI, but be mindful of log size limits
    cat "${LOG_FILE}" || echo "Failed to cat log file."
    exit $BUILD_EXIT_CODE
fi

# Clean up temporary keystore if it was created
if [[ -f "${PROJECT_PATH}/Temp/signing.keystore" ]]; then
    echo "Cleaning up temporary keystore..."
    rm -f "${PROJECT_PATH}/Temp/signing.keystore"
fi

echo "Unity client build script finished."