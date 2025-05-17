# PatternCipher CI/CD Pipelines (`REPO-CI-CD`)

## Overview

This repository (`REPO-CI-CD`) contains the infrastructure-as-code, scripts, and configurations for the Continuous Integration (CI) and Continuous Deployment (CD) pipelines supporting the PatternCipher project. It automates the building, testing, security scanning, and deployment of the Unity client (`REPO-UNITY-CLIENT`) and the Firebase backend (`REPO-FIREBASE-BACKEND`).

The primary goals are to:
- Ensure code quality and stability through automated checks.
- Streamline the release process.
- Maintain a secure and reliable deployment infrastructure.
- Enable rapid iteration and feedback cycles.

## Key Workflows

This repository defines several GitHub Actions workflows:

1.  **`main-ci.yml` (Main CI Pipeline)**
    *   **Triggers:** On `push` or `pull_request` to `main` and `develop` branches.
    *   **Functions:**
        *   Lints configuration files within this repository.
        *   Builds the Unity client (for editor/debug configuration).
        *   Runs unit tests for the Unity client.
        *   Builds Firebase functions.
        *   Runs unit tests for Firebase functions.
        *   Performs vulnerability scanning on dependencies for both client and backend.

2.  **`release-cd.yml` (Release CD Pipeline)**
    *   **Triggers:** On creation of a version tag (e.g., `v1.2.3`) or manually via `workflow_dispatch`.
    *   **Functions:**
        *   Builds release-ready Unity client players (iOS, Android).
        *   Builds Unity Addressable Asset Bundles.
        *   Packages Firebase functions for deployment.
        *   Deploys Firebase components (functions, Firestore rules, Storage rules) to `staging` and then to `production` (with manual approval for production).
        *   Deploys Unity client builds to distribution platforms (e.g., TestFlight, Google Play Console internal/beta tracks) via Fastlane for `staging` and `production` releases.

3.  **`scheduled-tasks.yml` (Scheduled Tasks Pipeline)**
    *   **Triggers:** On a defined CRON schedule (e.g., nightly) or manually via `workflow_dispatch`.
    *   **Functions (Examples):**
        *   Performs nightly builds of the Unity client.
        *   Runs extended test suites (e.g., integration tests).
        *   Conducts periodic comprehensive security scans.

## Setup

### Prerequisites for Contribution/Local Development

To contribute to this CI/CD repository or to run parts of the pipelines locally (e.g., scripts), you might need:

*   **Git:** For version control.
*   **Node.js & npm:** For Firebase tools, `yamllint`, and potentially other Node.js-based CLI tools. (Check `.github/workflows/*.yml` for `NODE_VERSION`).
*   **Unity Hub & Unity Editor:** If you intend to modify or test Unity build/test scripts locally. (Check `.github/workflows/*.yml` for `UNITY_VERSION`).
*   **Firebase CLI:** For interacting with Firebase projects. Install via `npm install -g firebase-tools`.
*   **Fastlane:** If contributing to mobile deployment automation. (Ruby environment required).
*   **Snyk CLI (Optional):** If running vulnerability scans locally.
*   **Shell Environment:** Bash or similar (most scripts are `.sh`).

### Environment Variables & Secrets

These pipelines rely heavily on GitHub Secrets for sensitive information like API keys, service accounts, and passwords.
*   **GitHub Secrets:** Must be configured in the GitHub repository settings (or organization settings). See `docs/secret-management-procedures.md` for details on required secrets and how they are used.
*   **Environment Configuration Files:** Non-sensitive, environment-specific parameters are managed in `config/environments/*.json`. These are typically read by scripts or workflows to adapt behavior for `dev`, `staging`, or `prod` environments.

**DO NOT commit secrets directly into this repository.**

## Contribution Guidelines

1.  **Branching Strategy:**
    *   Propose changes via feature branches created from `develop`.
    *   Submit Pull Requests to `develop` for review.
    *   `main` branch reflects the latest production-ready CI/CD configuration.
2.  **Testing:**
    *   When modifying workflows or scripts, test thoroughly on your feature branch.
    *   Ensure your changes pass all linting and validation steps defined in `main-ci.yml`.
3.  **Code Style:**
    *   Follow existing conventions for YAML (GitHub Actions), shell scripts, and other configuration files.
    *   Use `yamllint` for YAML files (configuration at `tool-config/linters/.yamllint.yml`).
4.  **Documentation:**
    *   Update relevant documentation (READMEs, architecture documents) if your changes impact pipeline behavior or setup.
5.  **Security:**
    *   Adhere strictly to the `config/secrets/secrets-management-policy.md`.
    *   Be mindful of security implications when introducing new tools or modifying deployment processes.

## Troubleshooting

*   **Workflow Failures:** Check the GitHub Actions logs for the specific job and step that failed. Error messages often indicate the cause.
*   **Script Failures:** Ensure scripts have execute permissions (`chmod +x`). Test scripts locally if possible, ensuring all required environment variables are set.
*   **Secret Issues:** Verify that all required GitHub Secrets are correctly configured and accessible to the workflow.
*   **Tool Version Mismatches:** Ensure local tool versions match those used in CI if troubleshooting locally.

## Links to Other Documentation

*   **Pipeline Architecture:** [`docs/pipeline-architecture.md`](./pipeline-architecture.md)
*   **Secret Management Policy:** [`config/secrets/secrets-management-policy.md`](../config/secrets/secrets-management-policy.md)
*   **Secret Management Procedures:** [`docs/secret-management-procedures.md`](./secret-management-procedures.md)

This CI/CD setup is crucial for the PatternCipher project's development lifecycle. Please handle with care and follow best practices.