# CI/CD Pipeline Architecture for PatternCipher

## 1. Introduction

This document details the architecture of the Continuous Integration (CI) and Continuous Deployment (CD) pipelines for the PatternCipher project. These pipelines are managed within the `REPO-CI-CD` repository and are responsible for building, testing, scanning, and deploying the `REPO-UNITY-CLIENT` (Unity game client) and `REPO-FIREBASE-BACKEND` (Firebase backend services).

The architecture emphasizes automation, modularity, security, and reliability to support an efficient development lifecycle.

## 2. Overall Pipeline Flow

The CI/CD system is composed of several key GitHub Actions workflows:

*   **Main CI (`main-ci.yml`):** Triggered on every push or pull request to `main` and `develop` branches. Its primary goal is to validate code changes rapidly.
    *   `REPO-CI-CD` -> Lint Config Files
    *   `REPO-UNITY-CLIENT` -> Build (Debug/Editor) -> Unit Test -> Vulnerability Scan
    *   `REPO-FIREBASE-BACKEND` -> Build Functions -> Unit Test -> Vulnerability Scan
*   **Release CD (`release-cd.yml`):** Triggered by version tag creation (e.g., `v1.0.0`) or manual dispatch. It handles the process of creating release builds and deploying them to various environments.
    *   Determine Version
    *   Build Release Candidates (Unity Client Player & Asset Bundles for iOS/Android)
    *   Build Firebase Release Package (Functions)
    *   Deploy to Staging Environment (Firebase, Unity Client via Fastlane)
    *   (Manual Approval Gate for Production)
    *   Deploy to Production Environment (Firebase, Unity Client via Fastlane)
*   **Scheduled Tasks (`scheduled-tasks.yml`):** Triggered on a CRON schedule or manually. Used for tasks not suitable for per-commit execution.
    *   Nightly Builds (e.g., Unity client)
    *   Extended Test Runs (e.g., integration tests)
    *   Periodic Comprehensive Security Scans

## 3. Workflow Details

### 3.1. Main CI Workflow (`main-ci.yml`)

*   **Purpose:** Continuous integration and validation.
*   **Triggers:** `push`, `pull_request` to `main`, `develop`.
*   **Key Jobs:**
    *   `lint-config`: Lints YAML files in `REPO-CI-CD` using `yamllint`.
    *   `ci-unity-client`:
        *   Checks out `REPO-UNITY-CLIENT` and `REPO-CI-CD`.
        *   Calls reusable workflow `reusable/build-unity-client.yml` (target: Editor, config: Debug).
        *   Runs Unity unit tests using `scripts/test/run-unity-tests.sh`.
        *   Uploads test results.
        *   Calls reusable workflow `reusable/run-vulnerability-scan.yml` for client dependencies.
    *   `ci-firebase-backend`:
        *   Checks out `REPO-FIREBASE-BACKEND` and `REPO-CI-CD`.
        *   Sets up Node.js and Firebase CLI (using `scripts/utils/setup-firebase-tools.sh`).
        *   Builds Firebase functions (using `scripts/build/build-firebase-functions.sh`).
        *   Runs Firebase unit tests (using `scripts/test/run-firebase-tests.sh`).
        *   Uploads test results.
        *   Calls reusable workflow `reusable/run-vulnerability-scan.yml` for backend dependencies.

### 3.2. Release CD Workflow (`release-cd.yml`)

*   **Purpose:** Automated release creation and deployment.
*   **Triggers:** `create` (tags `v*.*.*`), `workflow_dispatch`.
*   **Key Jobs:**
    *   `determine-version`: Extracts version from tag or input.
    *   `build-release-candidates`: (Matrix for iOS/Android)
        *   Checks out `REPO-UNITY-CLIENT` (at release tag) and `REPO-CI-CD`.
        *   Calls `reusable/build-unity-client.yml` (target: iOS/Android, config: Release).
        *   Builds Addressable Asset Bundles using `scripts/build/bundle-unity-assets.sh`.
        *   Uploads player and asset bundle artifacts.
    *   `build-firebase-release`:
        *   Checks out `REPO-FIREBASE-BACKEND` (at release tag or main) and `REPO-CI-CD`.
        *   Builds Firebase functions.
        *   Uploads functions artifact.
    *   `deploy-firebase-staging`:
        *   Depends on `build-firebase-release`.
        *   Downloads functions artifact.
        *   Calls `reusable/deploy-firebase.yml` (target: staging).
    *   `deploy-unity-staging`: (Matrix for iOS/Android)
        *   Depends on `build-release-candidates`, `deploy-firebase-staging`.
        *   Downloads player artifact.
        *   Sets up Fastlane.
        *   Calls `scripts/deploy/deploy-unity-via-fastlane.sh` (lane: `beta`).
    *   `deploy-firebase-prod`:
        *   Depends on `deploy-firebase-staging`.
        *   Requires manual approval (GitHub Environment protection rule).
        *   Calls `reusable/deploy-firebase.yml` (target: production).
    *   `deploy-unity-prod`: (Matrix for iOS/Android)
        *   Depends on `deploy-unity-staging`, `deploy-firebase-prod`.
        *   Requires manual approval.
        *   Calls `scripts/deploy/deploy-unity-via-fastlane.sh` (lane: `release`).

### 3.3. Scheduled Tasks Workflow (`scheduled-tasks.yml`)

*   **Purpose:** Periodic automation.
*   **Triggers:** `schedule` (CRON), `workflow_dispatch`.
*   **Key Jobs (Examples):**
    *   `nightly-build`: Calls `reusable/build-unity-client.yml` for a debug build of Unity client.
    *   `extended-test-run`: Placeholder for longer integration or performance tests.
    *   `periodic-security-scan`: Calls `reusable/run-vulnerability-scan.yml` for a comprehensive scan.

## 4. Reusable Workflows

To promote modularity and maintainability, common sequences of operations are encapsulated in reusable workflows located in `.github/workflows/reusable/`:

*   **`reusable/build-unity-client.yml`:**
    *   **Purpose:** Builds the Unity client player and optionally Addressable Asset Bundles.
    *   **Inputs:** `targetPlatform`, `buildConfiguration`, `unityVersion`, `projectPath`, `buildVersion`, Android signing details (optional).
    *   **Secrets:** Unity license credentials.
    *   **Steps:** Sets up Unity, activates license, runs `scripts/build/build-unity-client.sh`, (conditionally or always) `scripts/build/bundle-unity-assets.sh`, returns license, uploads artifacts.
*   **`reusable/deploy-firebase.yml`:**
    *   **Purpose:** Deploys Firebase components.
    *   **Inputs:** `firebaseProjectAlias`, `componentsToDeploy`.
    *   **Secrets:** Firebase service account.
    *   **Steps:** Sets up Node.js, Firebase CLI (using `scripts/utils/setup-firebase-tools.sh`), runs `scripts/deploy/deploy-firebase-components.sh`.
*   **`reusable/run-vulnerability-scan.yml`:**
    *   **Purpose:** Performs dependency vulnerability scans.
    *   **Inputs:** `projectPathClient`, `projectPathBackend`, `scanType`.
    *   **Secrets:** Scanner API key (e.g., `SNYK_TOKEN_SECRET`).
    *   **Steps:** Sets up scanner tool, runs `scripts/quality/scan-dependencies.sh`, uploads reports, fails job on critical vulnerabilities.

## 5. Scripting Layer

Shell scripts (`scripts/**/*.sh`) are used to encapsulate specific tool invocations and logic:

*   **Build Scripts (`scripts/build/`):**
    *   `build-unity-client.sh`: Invokes Unity Editor in batch mode for player builds.
    *   `bundle-unity-assets.sh`: Invokes Unity Editor for Addressable Asset Bundle builds.
    *   `build-firebase-functions.sh`: Runs `npm ci` and `npm run build` for Firebase functions.
*   **Test Scripts (`scripts/test/`):**
    *   `run-unity-tests.sh`: Invokes Unity Editor to run EditMode/PlayMode tests.
    *   `run-firebase-tests.sh`: Runs `npm test` for Firebase functions.
*   **Deploy Scripts (`scripts/deploy/`):**
    *   `deploy-unity-via-fastlane.sh`: Invokes Fastlane lanes for mobile deployment.
    *   `deploy-firebase-components.sh`: Uses `firebase deploy` to deploy specified components.
*   **Quality Scripts (`scripts/quality/`):**
    *   `scan-dependencies.sh`: Invokes vulnerability scanner CLI (e.g., Snyk).
*   **Utility Scripts (`scripts/utils/`):**
    *   `setup-unity-license.sh`: Activates/returns Unity licenses.
    *   `setup-firebase-tools.sh`: Installs and authenticates Firebase CLI.

These scripts are designed to be called from GitHub Actions workflows, receiving parameters via environment variables.

## 6. Configuration Management

*   **Environment Configurations (`config/environments/*.json`):** Store non-sensitive, environment-specific parameters (e.g., Firebase project IDs, feature flags). Workflows or scripts load these based on the target environment.
*   **Tool Configurations (`tool-config/`):**
    *   `fastlane/`: Fastlane `Fastfile` and `Appfile`.
    *   `linters/.yamllint.yml`: Configuration for `yamllint`.
    *   `scanners/.snyk`: Snyk policy file.
*   **Infrastructure-as-Code (`iac/firebase/`):**
    *   `firebase.json`: Firebase CLI project structure configuration.
    *   `firestore.rules`, `storage.rules`: Firebase security rules.
    *   `.firebaserc`: Firebase project aliases.

## 7. Secrets Management

Secure handling of secrets is critical (addresses `REQ-CPS-014`):

*   **Storage:** All secrets (API keys, service accounts, passwords) are stored exclusively in GitHub Secrets at the repository or organization level.
*   **Access:** Workflows access secrets via the `secrets` context (e.g., `${{ secrets.MY_SECRET }}`).
*   **Scripts:** Scripts receive secrets as environment variables populated by the calling workflow.
*   **Policy & Procedures:** Documented in `config/secrets/secrets-management-policy.md` and `docs/secret-management-procedures.md`.
*   **No Hardcoding:** Secrets are never hardcoded in code, configuration files, or logs.

## 8. Tooling Integrations

*   **GitHub Actions:** Core workflow engine.
*   **Unity & Unity Build Runner (e.g., `game-ci/unity-builder` or custom scripts):** For building and testing the Unity client.
*   **Fastlane:** For automating mobile app deployment (code signing, uploading to App Store Connect / Google Play Console).
*   **Firebase CLI (`firebase-tools`):** For deploying Firebase functions, rules, and other services.
*   **Snyk (or similar scanner):** For dependency vulnerability scanning.
*   **Yamllint:** For linting YAML files.
*   **Shell (Bash):** For scripting automation logic.
*   **Node.js & npm:** For Firebase development and various CLI tools.

## 9. Interaction Model

*   **Source Repositories:** Pipelines check out code from `REPO-UNITY-CLIENT` and `REPO-FIREBASE-BACKEND`.
*   **External Services:**
    *   **Firebase:** Deploys functions, rules; interacts for testing (emulators).
    *   **Apple App Store Connect & Google Play Console:** For mobile app distribution via Fastlane.
    *   **Snyk (or other scanners):** For vulnerability data.
*   **Artifacts:** Build artifacts (Unity players, bundles, Firebase function packages) and test/scan reports are managed using GitHub Actions artifacts.

This architecture provides a robust framework for automating the development lifecycle of the PatternCipher project, ensuring quality, security, and deployment efficiency.