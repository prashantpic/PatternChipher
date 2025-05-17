#!/bin/bash
set -euxo pipefail

# Purpose: Deploy Unity builds (iOS/Android) using Fastlane.
# Inputs (expected as environment variables):
#   PLATFORM: Target platform (ios, android).
#   LANE_NAME: Fastlane lane to execute (e.g., beta, release).
#   BUILD_PATH: Path to the compiled application artifact (.ipa for iOS, .aab or .apk for Android).
# Fastlane credentials and API keys are expected to be set as environment variables by the CI system from secrets:
#   FASTLANE_USER, FASTLANE_PASSWORD (for Apple ID)
#   FASTLANE_APPLE_APPLICATION_SPECIFIC_PASSWORD or APPLE_APP_SPECIFIC_PASSWORD_SECRET
#   FASTLANE_SESSION (alternative to user/password)
#   MATCH_PASSWORD or FASTLANE_MATCH_PASSWORD_SECRET (for match Nuke)
#   MATCH_GIT_URL or FASTLANE_MATCH_GIT_URL_SECRET
#   MATCH_GIT_BASIC_AUTHORIZATION or FASTLANE_MATCH_GIT_AUTHORIZATION_SECRET
#   SUPPLY_JSON_KEY or GOOGLE_PLAY_JSON_KEY_SECRET (for Google Play)
#   SUPPLY_PACKAGE_NAME (Android package name)

echo "Starting deployment via Fastlane..."
echo "Platform: ${PLATFORM}"
echo "Lane: ${LANE_NAME}"
echo "Build Path: ${BUILD_PATH}"

if [[ -z "${PLATFORM}" || -z "${LANE_NAME}" || -z "${BUILD_PATH}" ]]; then
    echo "Error: PLATFORM, LANE_NAME, and BUILD_PATH environment variables must be set."
    exit 1
fi

if [[ ! -f "${BUILD_PATH}" && ! -d "${BUILD_PATH}" ]]; then # .dSYM is a directory
    echo "Error: Build artifact not found at ${BUILD_PATH}"
    exit 1
fi

# Navigate to the Fastlane configuration directory within this CI/CD repository
FASTLANE_DIR_PATH="./tool-config/fastlane" # Relative to REPO-CI-CD root
if [[ ! -d "${FASTLANE_DIR_PATH}" ]]; then
    echo "Error: Fastlane directory not found at ${FASTLANE_DIR_PATH}"
    exit 1
fi
cd "${FASTLANE_DIR_PATH}"

echo "Current directory: $(pwd)"
echo "Listing files in Fastlane directory:"
ls -la

# Construct the Fastlane command
# Pass build_path as a Fastlane option
FASTLANE_COMMAND="fastlane ${LANE_NAME} platform:${PLATFORM} build_path:'${BUILD_PATH}'"

echo "Executing Fastlane command: ${FASTLANE_COMMAND}"
# The `bundle exec` prefix is often used with Fastlane to ensure it runs with the correct Gem versions
# defined in a Gemfile. If a Gemfile and Gemfile.lock are present and managed, this is best practice.
# Assuming bundler is available and gems are installed.
if command -v bundle &> /dev/null && [[ -f "Gemfile" ]]; then
    echo "Using 'bundle exec fastlane'..."
    bundle exec ${FASTLANE_COMMAND}
else
    echo "Using 'fastlane' directly (bundler not found or no Gemfile)..."
    ${FASTLANE_COMMAND}
fi

DEPLOY_EXIT_CODE=$?

if [ $DEPLOY_EXIT_CODE -eq 0 ]; then
    echo "Fastlane deployment successful for ${PLATFORM} to lane ${LANE_NAME}."
else
    echo "Fastlane deployment failed. Exit code: $DEPLOY_EXIT_CODE"
    # Fastlane usually provides detailed error messages.
    exit $DEPLOY_EXIT_CODE
fi

echo "Fastlane deployment script finished."