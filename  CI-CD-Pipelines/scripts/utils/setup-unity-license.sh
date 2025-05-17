#!/bin/bash
set -euxo pipefail

# Purpose: Automate Unity license activation/return on CI runners.
# Inputs:
#   $1 (ACTION): "activate" or "return".
# Environment Variables (must be set by the calling workflow):
#   UNITY_EXECUTABLE_PATH: Path to the Unity executable.
#   UNITY_LICENSE_CONTENT_SECRET: (Preferred) Base64 encoded Unity license file (.ulf) content.
#   OR (Alternative if UNITY_LICENSE_CONTENT_SECRET is not provided):
#   UNITY_EMAIL_SECRET: Unity account email.
#   UNITY_PASSWORD_SECRET: Unity account password.
#   UNITY_SERIAL_SECRET: Unity license serial key.

ACTION="${1}"

echo "Unity License Setup Script"
echo "Action: ${ACTION}"

if [[ -z "${UNITY_EXECUTABLE_PATH}" ]]; then
    echo "Error: UNITY_EXECUTABLE_PATH environment variable is not set."
    exit 1
fi
if [[ ! -f "${UNITY_EXECUTABLE_PATH}" && ! -x "${UNITY_EXECUTABLE_PATH}" ]]; then
    # For macOS, path might be to the .app directory, executable is inside
    if [[ "$OSTYPE" == "darwin"* ]] && [[ -d "${UNITY_EXECUTABLE_PATH}" ]]; then
        ACTUAL_UNITY_EXEC="${UNITY_EXECUTABLE_PATH}/Contents/MacOS/Unity"
        if [[ ! -f "${ACTUAL_UNITY_EXEC}" || ! -x "${ACTUAL_UNITY_EXEC}" ]]; then
             echo "Error: Unity executable not found or not executable at derived path ${ACTUAL_UNITY_EXEC} for macOS app bundle."
             exit 1
        fi
        UNITY_EXECUTABLE_PATH="${ACTUAL_UNITY_EXEC}"
    else
        echo "Error: UNITY_EXECUTABLE_PATH does not point to a valid Unity executable."
        exit 1
    fi
fi


LOG_FILE_PATH="/tmp/unity_license.log" # Or use /dev/stdout for direct CI logging

if [[ "${ACTION}" == "activate" ]]; then
    echo "Attempting to activate Unity license..."
    UNITY_LICENSE_ARGS=("-batchmode" "-quit" "-logFile" "${LOG_FILE_PATH}")

    if [[ -n "${UNITY_LICENSE_CONTENT_SECRET}" ]]; then
        echo "Activating using license file content (UNITY_LICENSE_CONTENT_SECRET)."
        TEMP_ULF_FILE="/tmp/unity_license.ulf"
        echo "${UNITY_LICENSE_CONTENT_SECRET}" | base64 -d > "${TEMP_ULF_FILE}"
        if [[ ! -s "${TEMP_ULF_FILE}" ]]; then
            echo "Error: Decoded license file is empty. Check UNITY_LICENSE_CONTENT_SECRET."
            rm -f "${TEMP_ULF_FILE}"
            exit 1
        fi
        UNITY_LICENSE_ARGS+=("-manualLicenseFile" "${TEMP_ULF_FILE}")
    elif [[ -n "${UNITY_EMAIL_SECRET}" && -n "${UNITY_PASSWORD_SECRET}" && -n "${UNITY_SERIAL_SECRET}" ]]; then
        echo "Activating using username, password, and serial (UNITY_EMAIL_SECRET, UNITY_PASSWORD_SECRET, UNITY_SERIAL_SECRET)."
        UNITY_LICENSE_ARGS+=(
            "-username" "${UNITY_EMAIL_SECRET}"
            "-password" "${UNITY_PASSWORD_SECRET}"
            "-serial" "${UNITY_SERIAL_SECRET}"
        )
    else
        echo "Error: Insufficient credentials for license activation."
        echo "Please provide either UNITY_LICENSE_CONTENT_SECRET or (UNITY_EMAIL_SECRET, UNITY_PASSWORD_SECRET, and UNITY_SERIAL_SECRET)."
        exit 1
    fi

    echo "Executing Unity license activation command..."
    "${UNITY_EXECUTABLE_PATH}" "${UNITY_LICENSE_ARGS[@]}"
    ACTIVATION_EXIT_CODE=$?

    # Clean up temporary ULF file if created
    if [[ -f "/tmp/unity_license.ulf" ]]; then
        rm -f "/tmp/unity_license.ulf"
    fi

    # Unity exit codes for licensing:
    # 0: Success
    # 1: Generic error / license invalid / activation failed
    # Check log file for specific errors
    echo "Unity activation process finished. Log content from ${LOG_FILE_PATH}:"
    cat "${LOG_FILE_PATH}" || echo "Failed to cat license log file."
    
    if [ $ACTIVATION_EXIT_CODE -ne 0 ]; then
        echo "Unity license activation failed. Exit code: $ACTIVATION_EXIT_CODE"
        exit $ACTIVATION_EXIT_CODE
    else
        echo "Unity license activated successfully."
    fi

elif [[ "${ACTION}" == "return" ]]; then
    echo "Attempting to return Unity license..."
    UNITY_RETURN_ARGS=(
        "-batchmode"
        "-quit"
        "-returnlicense"
        "-logFile" "${LOG_FILE_PATH}"
    )
    # Some versions might require projectPath even for returning license
    # if [[ -n "${PROJECT_PATH}" ]]; then
    #   UNITY_RETURN_ARGS+=("-projectPath" "${PROJECT_PATH}")
    # fi

    echo "Executing Unity license return command..."
    "${UNITY_EXECUTABLE_PATH}" "${UNITY_RETURN_ARGS[@]}"
    RETURN_EXIT_CODE=$?

    echo "Unity return license process finished. Log content from ${LOG_FILE_PATH}:"
    cat "${LOG_FILE_PATH}" || echo "Failed to cat license log file."

    if [ $RETURN_EXIT_CODE -ne 0 ]; then
        echo "Unity license return failed. Exit code: $RETURN_EXIT_CODE"
        # Depending on CI setup, failing to return a license might not be a critical failure for the job
        # exit $RETURN_EXIT_CODE 
        echo "Warning: License return failed. This might affect future activations if seat limit is reached."
    else
        echo "Unity license returned successfully."
    fi
else
    echo "Error: Invalid action '${ACTION}'. Must be 'activate' or 'return'."
    exit 1
fi

echo "Unity license script finished."