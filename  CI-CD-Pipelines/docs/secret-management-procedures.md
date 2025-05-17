# Secret Management Operational Procedures

## 1. Introduction

This document provides detailed operational procedures for managing secrets within the PatternCipher project, complementing the `config/secrets/secrets-management-policy.md`. Adherence to these procedures is crucial for maintaining the security and integrity of our CI/CD pipelines and deployed applications, addressing requirement `REQ-CPS-014`.

## 2. Managing Secrets in GitHub Actions

GitHub Actions secrets are encrypted environment variables created in an organization, repository, or repository environment.

### 2.1. Adding or Updating Secrets

**Scope:** Repository secrets are accessible to actions in that repository. Organization secrets are accessible to actions in selected repositories within the organization. Environment secrets are specific to deployment environments.

**Procedure:**

1.  **Navigate to Settings:**
    *   **Repository Secrets:** Go to your repository on GitHub -> `Settings` -> `Secrets and variables` -> `Actions`.
    *   **Organization Secrets:** Go to your organization on GitHub -> `Settings` -> `Secrets and variables` -> `Actions`.
    *   **Environment Secrets:** Go to your repository -> `Settings` -> `Environments` -> Select or create an environment -> `Add secret`.
2.  **Select `Secrets` Tab.**
3.  **Click `New repository secret`, `New organization secret`, or `Add secret` (for environments).**
4.  **Name:** Enter a descriptive name for your secret (e.g., `FIREBASE_SERVICE_ACCOUNT_PROD_SECRET`, `SNYK_TOKEN_SECRET`). Use uppercase letters with underscores.
5.  **Value:** Paste the secret value.
    *   **Important:** Ensure there are no leading/trailing spaces or newlines unless they are part of the secret itself.
    *   For multi-line secrets (like some private keys or JSON service accounts), ensure the entire content is pasted correctly. Base64 encoding can sometimes simplify handling multi-line secrets if the consuming script decodes it.
6.  **Repository Access (for Organization Secrets):** Configure which repositories can access the organization secret. Apply the principle of least privilege.
7.  **Click `Add secret`.**

### 2.2. Accessing Secrets in Workflows

Secrets are exposed to GitHub Actions workflows as environment variables but are also accessible via the `secrets` context.

```yaml
jobs:
  example_job:
    runs-on: ubuntu-latest
    steps:
      - name: Example step using a secret
        env:
          MY_API_KEY: ${{ secrets.API_KEY_NAME }}
        run: |
          echo "API Key will be masked in logs if used directly: $MY_API_KEY"
          # It's better to pass to scripts or tools that expect env vars
          # or use it in a way that doesn't log it.
```

### 2.3. Using Secrets in Scripts Called by Workflows

Scripts invoked by workflows can access secrets that have been explicitly passed as environment variables from the workflow file.

**Workflow Step:**
```yaml
- name: Run script with secret
  env:
    SCRIPT_SECRET_VAR: ${{ secrets.MY_SCRIPT_SECRET }}
  run: ./scripts/my_script.sh
```

**Shell Script (`./scripts/my_script.sh`):**
```bash
#!/bin/bash
set -euo pipefail

echo "Accessing secret from environment variable: $SCRIPT_SECRET_VAR"
# Use $SCRIPT_SECRET_VAR as needed by the script's logic
```

## 3. Managing Firebase Service Account Keys

Firebase service account keys are JSON files that grant server-to-server access to your Firebase project.

### 3.1. Generating a Service Account Key

1.  In the Firebase console, open **Settings > Service accounts**.
2.  Click **Generate new private key**, then confirm by clicking **Generate key**.
3.  Securely store the JSON file that is downloaded. **This key provides significant access to your project.**

### 3.2. Storing Service Account Key in GitHub Secrets

1.  **Option 1 (Raw JSON - if small and simple):** Copy the entire content of the downloaded JSON file.
2.  **Option 2 (Base64 Encoded):** To avoid issues with multi-line JSON in some environments, you can Base64 encode the JSON file content:
    ```bash
    cat /path/to/your-service-account-file.json | base64
    ```
    Copy the resulting Base64 string.
3.  Create a new GitHub secret (e.g., `FIREBASE_SERVICE_ACCOUNT_PROD_SECRET`) and paste the raw JSON content or the Base64 encoded string as its value.

### 3.3. Using Service Account Key in CI/CD

*   **If Base64 Encoded:** The script (`scripts/utils/setup-firebase-tools.sh` or similar) will need to decode it first and save it to a temporary file. The path to this temporary file is then set to the `GOOGLE_APPLICATION_CREDENTIALS` environment variable.
    ```bash
    # In setup-firebase-tools.sh
    echo "${BASE64_ENCODED_SA_KEY}" | base64 --decode > /tmp/service_account.json
    export GOOGLE_APPLICATION_CREDENTIALS=/tmp/service_account.json
    # ... use firebase tools ...
    rm /tmp/service_account.json # Clean up
    ```
*   **If Raw JSON:** The script might directly write the secret content to a temporary file.
    ```bash
    # In setup-firebase-tools.sh
    echo "${RAW_JSON_SA_KEY}" > /tmp/service_account.json
    export GOOGLE_APPLICATION_CREDENTIALS=/tmp/service_account.json
    # ... use firebase tools ...
    rm /tmp/service_account.json # Clean up
    ```
    Or, Firebase tools might support `FIREBASE_CONFIG` directly for some auth methods.

## 4. Procedures for Secret Rotation

Regular rotation minimizes the risk associated with compromised secrets.

### 4.1. General Rotation Steps

1.  **Generate New Secret:** Create a new key, password, or token from the respective service provider (e.g., Firebase, Apple Developer Portal, Google Play Console, Snyk).
2.  **Add New Secret to Store:** Add the new secret to GitHub Secrets with a temporary or versioned name (e.g., `MY_API_KEY_V2`) or prepare to update the existing one.
3.  **Update Applications/Pipelines:**
    *   Modify CI/CD workflows and configurations to use the new secret.
    *   If the secret is used by deployed applications, deploy an updated version of the application configured with the new secret. This might involve a phased rollout.
4.  **Test:** Thoroughly test that applications and pipelines function correctly with the new secret.
5.  **Deactivate/Revoke Old Secret:** Once confident the new secret is working, revoke or delete the old secret from the service provider.
6.  **Remove Old Secret from Store:** If you used a temporary name, you can now update the primary secret name in GitHub Secrets or remove the old versioned secret.

### 4.2. Specific Secret Types

*   **Firebase Service Account Keys:**
    1.  Generate a new private key in Firebase Console (Settings > Service Accounts).
    2.  Add the new key (content) to GitHub Secrets (e.g., `FIREBASE_SA_STAGING_NEW`).
    3.  Update the relevant GitHub Actions workflow (e.g., `deploy-firebase.yml` inputs or direct env var in `release-cd.yml`) to use the new secret.
    4.  Trigger a deployment to staging. Verify functionality.
    5.  Repeat for production if applicable.
    6.  Once all environments use the new key, delete the old service account key from the Google Cloud IAM console (Service Accounts section).
    7.  Remove the old secret from GitHub Secrets.
*   **Fastlane API Keys (e.g., App Store Connect API Key):**
    1.  Generate a new API key in App Store Connect (Users and Access > Keys). Download the `.p8` file.
    2.  Note the new Key ID and Issuer ID.
    3.  Encode the `.p8` file content (e.g., Base64).
    4.  Update GitHub Secrets: `FASTLANE_APPLE_API_KEY_ID`, `FASTLANE_APPLE_ISSUER_ID`, `FASTLANE_APPLE_API_KEY_CONTENT_BASE64`.
    5.  Test Fastlane lanes that use this authentication method.
    6.  Revoke the old API key in App Store Connect.
*   **Unity License Secrets (`UNITY_LICENSE_CONTENT_SECRET`, `UNITY_EMAIL_SECRET`, etc.):**
    *   If using `.ulf` file content: Generate a new license file if needed (e.g., due to Unity version changes or seat management) and update `UNITY_LICENSE_CONTENT_SECRET`.
    *   If using email/password/serial: Update these secrets if credentials change.
    *   Test by running a Unity build workflow.
*   **Scanner API Tokens (e.g., `SNYK_TOKEN_SECRET`):**
    1.  Generate a new API token from the scanner service's dashboard (e.g., Snyk Account Settings).
    2.  Update the `SNYK_TOKEN_SECRET` in GitHub Secrets.
    3.  Trigger a workflow that uses the scanner to verify.
    4.  Revoke the old token in the scanner service's dashboard.

## 5. Auditing Secret Access and Usage

*   **GitHub Audit Logs:**
    *   Organization owners can review audit logs for events related to organization secrets (creation, deletion, updates, repository access changes).
    *   Repository admins can review audit logs for repository secrets.
    *   Look for unexpected changes or access patterns.
*   **GitHub Actions Workflow Logs:**
    *   Secrets themselves are masked in logs. However, observe workflow runs to ensure secrets are being used by expected jobs and steps.
    *   Unexpected failures related to authentication or authorization might indicate secret issues.
*   **Cloud Provider Logs (e.g., Google Cloud Audit Logs for Firebase):**
    *   Monitor logs for services authenticated using service accounts managed as secrets.
    *   Look for suspicious activity or unauthorized access attempts.
*   **Periodic Review:** Schedule regular reviews (e.g., quarterly) of who has access to GitHub repository/organization settings where secrets are managed. Review lists of active secrets and confirm their continued necessity.

## 6. Emergency Response for Compromised Secrets

If a secret is suspected or confirmed to be compromised:

1.  **Isolate & Identify:**
    *   Determine which secret(s) are compromised.
    *   Identify the potential scope of impact (what systems/data can be accessed with this secret?).
2.  **Immediate Revocation/Rotation (Containment):**
    *   **Highest Priority:** Revoke the compromised secret immediately at its source (e.g., delete the Firebase service account key, revoke the API key in App Store Connect, change the password). This is the most critical step to prevent further unauthorized access.
    *   If immediate revocation isn't possible, temporarily disable services or restrict network access that relies on the secret, if feasible.
3.  **Generate New Secret:** Create a replacement secret.
4.  **Securely Update:** Update the new secret in GitHub Secrets and any other secure stores.
5.  **Deploy Updates:**
    *   Update CI/CD pipelines to use the new secret.
    *   If the secret is used by deployed applications, urgently deploy updated application versions with the new secret.
6.  **Investigate (Eradication & Recovery):**
    *   Determine how the compromise occurred (e.g., accidental commit, malware, social engineering).
    *   Analyze logs for any unauthorized activity performed using the compromised secret.
    *   Take steps to remediate any damage or unauthorized changes.
7.  **Lessons Learned & Prevention (Post-Mortem):**
    *   Document the incident.
    *   Identify and implement measures to prevent similar compromises in the future (e.g., improved training, better tooling, stricter access controls).
8.  **Communication:** Inform relevant stakeholders as per the project's incident communication plan.

By following these procedures, we aim to maintain a high level of security for all secrets used in the PatternCipher project.