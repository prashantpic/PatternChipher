#!/bin/bash
set -euxo pipefail

# Purpose: Run dependency vulnerability scans on client and backend projects.
# Inputs:
#   $1 (CLIENT_PROJECT_PATH): Path to the Unity client project (REPO-UNITY-CLIENT).
#   $2 (BACKEND_PROJECT_PATH): Path to the Firebase functions project (REPO-FIREBASE-BACKEND/backend/firebase-functions).
# Environment Variables:
#   SNYK_TOKEN: API token for Snyk authentication (expected to be set from GitHub Secrets).
#   SCAN_SEVERITY_THRESHOLD: (Optional) Snyk severity threshold (e.g., high, medium). Defaults to Snyk's config or dashboard settings.
#   SCAN_FAIL_ON_ISSUES: (Optional) Set to "true" to fail script if issues meeting threshold are found. Defaults to "true".

CLIENT_PROJECT_PATH="${1:-}"
BACKEND_PROJECT_PATH="${2:-}"
FAIL_ON_ISSUES="${SCAN_FAIL_ON_ISSUES:-true}"

echo "Starting dependency vulnerability scan..."

if [[ -z "${SNYK_TOKEN}" ]]; then
    echo "Error: SNYK_TOKEN environment variable is not set. Cannot authenticate with Snyk."
    exit 1
fi

# Ensure Snyk CLI is installed (often handled by a setup step in the workflow)
if ! command -v snyk &> /dev/null; then
    echo "Snyk CLI not found. Please install Snyk CLI first."
    echo "Example: npm install -g snyk"
    exit 1
fi

echo "Authenticating Snyk CLI..."
snyk auth "${SNYK_TOKEN}"

OVERALL_SCAN_PASSED=true

# Scan Unity Client Project (if path provided)
if [[ -n "${CLIENT_PROJECT_PATH}" ]]; then
    echo "--- Scanning Unity Client Project Dependencies at ${CLIENT_PROJECT_PATH} ---"
    if [[ ! -d "${CLIENT_PROJECT_PATH}" ]]; then
        echo "Warning: Client project path ${CLIENT_PROJECT_PATH} not found. Skipping scan."
    else
        # Unity dependencies are typically in Packages/packages-lock.json or Packages/manifest.json
        # Snyk might need specific configurations or plugins for Unity .NET dependencies.
        # This is a generic example; actual Snyk command might vary based on project setup and Snyk's Unity support.
        # For C#/.NET, Snyk usually scans solution (.sln) or project (.csproj) files.
        # If `packages-lock.json` is used for UPM packages with git dependencies, it might be a target.
        # Let's assume a common entry point or that Snyk can auto-detect.
        # The most reliable way is usually `snyk test` in the project root.
        
        # Check for packages-lock.json for UPM packages
        UNITY_MANIFEST_FILE_PRIMARY="${CLIENT_PROJECT_PATH}/Packages/packages-lock.json"
        UNITY_MANIFEST_FILE_SECONDARY="${CLIENT_PROJECT_PATH}/Packages/manifest.json"
        SNYK_UNITY_TARGET_FILE=""

        if [[ -f "${UNITY_MANIFEST_FILE_PRIMARY}" ]]; then
            SNYK_UNITY_TARGET_FILE="--file=${UNITY_MANIFEST_FILE_PRIMARY}"
            echo "Found Unity packages-lock.json. Scanning it."
        elif [[ -f "${UNITY_MANIFEST_FILE_SECONDARY}" ]]; then
             # Snyk may not directly parse manifest.json for vulnerabilities in the same way as lock files
             # For actual .NET dependencies, it would scan .sln or .csproj files
            echo "Found Unity manifest.json. Snyk's .NET scanning will look for .sln/.csproj files."
        else
            echo "No primary manifest (packages-lock.json) found. Snyk .NET scanning will attempt to find .sln/.csproj."
        fi

        SNYK_CLIENT_CMD="snyk test ${SNYK_UNITY_TARGET_FILE} --json --project-name=PatternCipher-UnityClient"
        if [[ -n "${SCAN_SEVERITY_THRESHOLD}" ]]; then
            SNYK_CLIENT_CMD="${SNYK_CLIENT_CMD} --severity-threshold=${SCAN_SEVERITY_THRESHOLD}"
        fi
        
        echo "Running Snyk command for client: ${SNYK_CLIENT_CMD}"
        if ${SNYK_CLIENT_CMD} > snyk-client-results.json; then
            echo "Snyk scan for client project completed. No issues found above threshold or Snyk exited successfully."
        else
            SCAN_EXIT_CODE=$?
            echo "Snyk scan for client project found issues or an error occurred. Exit code: ${SCAN_EXIT_CODE}"
            # Snyk exits with 1 if vulns found, >1 for errors.
             if [[ "$FAIL_ON_ISSUES" == "true" ]]; then
                OVERALL_SCAN_PASSED=false
            fi
        fi
        echo "Snyk client scan results (JSON head):"
        head -n 20 snyk-client-results.json || echo "Could not display Snyk client results."
        # Consider uploading snyk-client-results.json as an artifact
    fi
else
    echo "Client project path not provided. Skipping client scan."
fi

# Scan Firebase Backend Project (if path provided)
if [[ -n "${BACKEND_PROJECT_PATH}" ]]; then
    echo "--- Scanning Firebase Backend Project Dependencies at ${BACKEND_PROJECT_PATH} ---"
    if [[ ! -d "${BACKEND_PROJECT_PATH}" ]]; then
        echo "Warning: Backend project path ${BACKEND_PROJECT_PATH} not found. Skipping scan."
    elif [[ ! -f "${BACKEND_PROJECT_PATH}/package.json" ]]; then
        echo "Warning: package.json not found in ${BACKEND_PROJECT_PATH}. Skipping backend scan."
    else
        # For Node.js, Snyk scans package.json and package-lock.json
        SNYK_BACKEND_CMD="snyk test --json --project-name=PatternCipher-FirebaseBackend"
        if [[ -n "${SCAN_SEVERITY_THRESHOLD}" ]]; then
            SNYK_BACKEND_CMD="${SNYK_BACKEND_CMD} --severity-threshold=${SCAN_SEVERITY_THRESHOLD}"
        fi

        echo "Running Snyk command for backend in ${BACKEND_PROJECT_PATH}: ${SNYK_BACKEND_CMD}"
        (cd "${BACKEND_PROJECT_PATH}" && ${SNYK_BACKEND_CMD} > ../snyk-backend-results.json) # Output relative to BACKEND_PROJECT_PATH parent
        SCAN_EXIT_CODE=$? # Get exit code of the snyk command within subshell

        if [ $SCAN_EXIT_CODE -eq 0 ]; then
            echo "Snyk scan for backend project completed. No issues found above threshold or Snyk exited successfully."
        else
            echo "Snyk scan for backend project found issues or an error occurred. Exit code: ${SCAN_EXIT_CODE}"
             if [[ "$FAIL_ON_ISSUES" == "true" ]]; then
                OVERALL_SCAN_PASSED=false
            fi
        fi
        echo "Snyk backend scan results (JSON head from ../snyk-backend-results.json):"
        # Path is relative to current dir after cd, so it should be:
        head -n 20 "${BACKEND_PROJECT_PATH}/../snyk-backend-results.json" || echo "Could not display Snyk backend results."
        # Consider uploading snyk-backend-results.json as an artifact
    fi
else
    echo "Backend project path not provided. Skipping backend scan."
fi

echo "--- Scan Summary ---"
if [[ "$OVERALL_SCAN_PASSED" == "true" ]]; then
    echo "All scans passed or did not meet failure criteria."
    exit 0
else
    echo "One or more scans found issues meeting the failure criteria."
    exit 1
fi

echo "Dependency vulnerability scan script finished."