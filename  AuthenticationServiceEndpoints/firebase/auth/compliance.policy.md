# Compliance Policy: Child Data Privacy (COPPA/GDPR-K)

This document formalizes the operational policy for the use of configured authentication providers to ensure compliance with legal requirements regarding child data privacy, specifically `NFR-LC-002a` (COPPA/GDPR-K).

---

## 1. Policy Objective
To ensure the protection of personal data for users identified as children (under 13 or the applicable regional age of digital consent).

---

## 2. Authentication Provider Access Policy

The presentation of sign-in options to the end-user is governed by the user's age, which must be determined via a neutral age-gate mechanism.

### 2.1 Users Under the Age of Consent
For users who are identified as being under the age of digital consent:

-   **Permitted Provider:** Only **Anonymous Authentication** shall be offered. This method does not require or collect Personally Identifiable Information (PII).
-   **Restricted Providers:** **Google Sign-In** and **Sign in with Apple** options **MUST NOT** be presented to these users, as these services involve the processing of PII (e.g., name, email address).

### 2.2 Users At or Above the Age of Consent
For users who are identified as being at or above the age of digital consent:

-   **Permitted Providers:** All enabled providers (Anonymous, Google, Apple) may be offered to the user.
-   **Account Linking:** Users who initially signed in anonymously may be offered the option to link their account to a Google or Apple account to enable cross-device progress synchronization (`FR-ONL-003`).

---

## 3. Enforcement Responsibility

This policy outlines the rules for compliant use of the authentication service. The enforcement of these rules is the explicit responsibility of the client application.

-   While the Firebase backend is configured to *enable* all providers, the **client application** (`REPO-PATT-001`) is solely responsible for implementing and enforcing this access policy.
-   The client application **MUST** implement a neutral age gate upon first launch to determine the user's age status.
-   The client application's UI **MUST** conditionally display the Google and Apple sign-in options only after verifying the user meets the age requirement defined in this policy.