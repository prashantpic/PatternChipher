# Software Design Specification (SDS) for AuthenticationServiceEndpoints

## 1.0 Introduction

### 1.1 Purpose
This document specifies the design and configuration for the **AuthenticationServiceEndpoints** (`REPO-PATT-006`). This repository is not a traditional code repository; it defines the configuration-as-code and associated policies for the managed **Firebase Authentication** service. Its purpose is to declare which authentication methods are available to the Pattern Cipher game, how user accounts are managed, and how this configuration complies with legal and business requirements.

### 1.2 Scope
The scope of this specification is limited to the configuration of Firebase Authentication providers and the documentation of related setup and compliance policies. It includes:
- Enabling and configuring Anonymous, Google, and Apple sign-in providers.
- Defining the account linking strategy to allow users to upgrade from anonymous to permanent accounts.
- Documenting the manual setup steps required in external developer consoles (Google Cloud, Apple Developer).
- Formalizing the compliance policy regarding child data privacy (COPPA/GDPR-K).

This specification **does not** cover the client-side implementation of the UI for login screens or the logic for presenting authentication options to the user. That logic resides in the `GameClientApplication` (`REPO-PATT-001`) and its sub-components, which will consume the configuration defined here.

## 2.0 System Overview
This service is a foundational component of the backend architecture, providing secure user identity management. It has no direct code dependencies but is a prerequisite for all other online services that require user identification, such as Cloud Save (`REPO-PATT-007`) and Leaderboards (`REPO-PATT-008`). The architecture is serverless, relying entirely on the managed Firebase Authentication platform.

## 3.0 Detailed Design

### 3.1 File: `firebase/auth/authentication.config.json`

This file serves as the declarative source of truth for the authentication service's configuration. The client application may read this configuration via Firebase Remote Config to dynamically adjust its behavior, but its primary purpose is to define the backend state.

#### 3.1.1. Purpose
To specify the enabled authentication providers and account management settings for the Firebase project.

#### 3.1.2. JSON Schema and Configuration Details
The file will adhere to the following JSON structure:

json
{
  "enabledProviders": {
    "anonymous": {
      "enabled": true,
      "description": "Provides a low-friction, PII-free entry point for all users, including those under the age of consent. This is the default authentication method. Fulfills NFR-LC-002a."
    },
    "google": {
      "enabled": true,
      "description": "Enables Google Sign-In. The client application MUST gate this option, offering it only to users who pass the age gate (>= 13 years or applicable age of consent). Fulfills FR-ONL-003 by providing a persistent identity."
    },
    "apple": {
      "enabled": true,
      "description": "Enables Sign in with Apple. The client application MUST gate this option, offering it only to users who pass the age gate (>= 13 years or applicable age of consent). Fulfills FR-ONL-003 by providing a persistent identity."
    }
  },
  "accountManagement": {
    "accountLinking": {
      "enabled": true,
      "description": "Allows a single user account to be linked with multiple sign-in providers. This is critical for allowing an anonymous user to upgrade to a Google or Apple account, thereby preserving their game progress. Fulfills FR-ONL-003 for cross-device synchronization."
    }
  }
}


**Implementation Notes:**
-   **`enabledProviders`**: This object lists all potential providers. A value of `true` means the provider is enabled in the Firebase Console.
-   **Client-Side Gating**: It is critical to understand that enabling `google` and `apple` here only makes them available on the backend. The client application holds the responsibility to implement an age gate and only display these sign-in options to users who are of appropriate age, as per the policy in `compliance.policy.md`.
-   **`accountLinking`**: This setting refers to the "One account per email address" option in the Firebase Authentication settings. It must be enabled to allow anonymous users to link their existing progress to a permanent Google or Apple account.

---

### 3.2 File: `firebase/auth/setup.guide.md`

This document provides essential, step-by-step instructions for developers to perform the manual configuration steps required for social sign-in providers.

#### 3.2.1. Purpose
To guide developers through the necessary setup in the Google Cloud Platform Console and Apple Developer Portal, which cannot be automated.

#### 3.2.2. Content Specification

The document must contain the following sections:

**1. Google Sign-In Setup (for Android & iOS)**
-   **Step 1: Configure Firebase Project:** Instructions on adding SHA-1 and SHA-256 certificate fingerprints for the Android app in the Firebase project settings.
-   **Step 2: Google Cloud Console Configuration:**
    -   Instructions on navigating to the "APIs & Services" > "Credentials" page for the linked GCP project.
    -   Detailed steps for creating/configuring the "OAuth 2.0 Client IDs" for both Android and iOS.
    -   Instructions on configuring the "OAuth consent screen," including specifying the app name, user support email, and developer contact information.
-   **Step 3: Client Integration Files:** Instructions on downloading the updated `google-services.json` (for Android) and `GoogleService-Info.plist` (for iOS) and placing them in the correct directory within the Unity project.

**2. Apple Sign-In Setup (for iOS)**
-   **Step 1: Apple Developer Portal Configuration:**
    -   Instructions on navigating to "Certificates, Identifiers & Profiles".
    -   Detailed steps for enabling the "Sign in with Apple" capability for the game's App ID.
    -   Instructions on how to create a "Services ID" for the app.
-   **Step 2: Xcode Project Configuration:** Instructions on adding the "Sign in with Apple" capability in the "Signing & Capabilities" tab of the Xcode project generated by Unity.

---

### 3.3 File: `firebase/auth/compliance.policy.md`

This document formalizes the operational policy for ensuring the authentication service is used in a manner compliant with child data privacy laws.

#### 3.3.1. Purpose
To provide an auditable policy that links the technical configuration to the legal requirements of `NFR-LC-002a` (COPPA/GDPR-K compliance).

#### 3.3.2. Content Specification

The document must contain the following sections:

**1. Policy Objective**
-   To ensure the protection of personal data for users identified as children (under 13 or the applicable regional age of digital consent).

**2. Authentication Provider Access Policy**
-   **2.1 Users Under the Age of Consent:**
    -   **Permitted Provider:** Only **Anonymous Authentication** shall be offered. This method does not require or collect PII.
    -   **Restricted Providers:** **Google Sign-In** and **Sign in with Apple** options **MUST NOT** be presented to these users.
-   **2.2 Users At or Above the Age of Consent:**
    -   **Permitted Providers:** All enabled providers (Anonymous, Google, Apple) may be offered.
    -   **Account Linking:** Users who initially signed in anonymously may be offered the option to link their account to a Google or Apple account to enable cross-device progress synchronization (`FR-ONL-003`).

**3. Enforcement Responsibility**
-   This section must explicitly state that while the Firebase backend is configured to *enable* all providers, the **client application** (`REPO-PATT-001`) is solely responsible for implementing and enforcing this access policy.
-   The client application **MUST** implement a neutral age gate upon first launch.
-   The client application's UI **MUST** conditionally display the Google and Apple sign-in options only after verifying the user meets the age requirement.