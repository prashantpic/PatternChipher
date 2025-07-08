# Specification

# 1. Files

- **Path:** firebase/auth/authentication.config.json  
**Description:** Declarative configuration file for Firebase Authentication. Specifies which sign-in providers are enabled for the project, their configurations, and settings related to user account management like account linking and multi-factor authentication (MFA) if applicable.  
**Template:** Firebase Configuration  
**Dependency Level:** 0  
**Name:** authentication.config  
**Type:** Configuration  
**Relative Path:** authentication.config.json  
**Repository Id:** REPO-PATT-006  
**Pattern Ids:**
    
    - MBaaS
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Enable Anonymous Authentication
    - Enable Google Sign-In Provider
    - Enable Apple Sign-In Provider
    - Configure Account Linking
    
**Requirement Ids:**
    
    - FR-ONL-003
    - NFR-LC-002a
    
**Purpose:** To define the active authentication methods available to the client application. This file serves as the single source of truth for the authentication strategy.  
**Logic Description:** This file is a JSON representation of Firebase Authentication settings. The 'providers' array lists each enabled method. 'anonymous' is enabled by default for all users. 'google' and 'apple' providers are enabled on the backend, but their client-side presentation is gated by age verification logic (handled by the client). 'accountLinking' is enabled to allow anonymous users to upgrade their accounts to a permanent one (e.g., Google/Apple) to support cross-device progress synchronization (FR-ONL-003). The actual client IDs and secrets are managed as secure secrets, not stored here.  
**Documentation:**
    
    - **Summary:** Defines the enabled Firebase Authentication providers. The client application reads this configuration contextually to determine which sign-in options to present to the user. This configuration is the backend counterpart to the client's UI logic for authentication.
    
**Namespace:** PatternCipher.Services.Auth.Config  
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** firebase/auth/setup.guide.md  
**Description:** Markdown documentation outlining the necessary manual steps to configure the authentication providers in the Firebase console and associated third-party developer portals (Google Cloud Platform, Apple Developer).  
**Template:** Markdown Documentation  
**Dependency Level:** 1  
**Name:** setup.guide  
**Type:** Documentation  
**Relative Path:** setup.guide.md  
**Repository Id:** REPO-PATT-006  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Google Sign-In Setup Guide
    - Apple Sign-In Setup Guide
    
**Requirement Ids:**
    
    - FR-ONL-003
    
**Purpose:** To provide clear, step-by-step instructions for configuring OAuth 2.0 credentials and consent screens required for Google and Apple sign-in, which cannot be fully automated.  
**Logic Description:** This document will contain sections for each external provider. For Google Sign-In, it will detail how to create OAuth 2.0 Client IDs in the Google Cloud Console, configure the consent screen with app information, and add the necessary SHA-1/SHA-256 fingerprints for the Android app. For Apple Sign-In, it will detail how to configure an App ID with Sign in with Apple capability, create a Services ID, and generate a private key for server-side validation. These manual steps are prerequisites for the settings in authentication.config.json to function correctly.  
**Documentation:**
    
    - **Summary:** A procedural guide for developers to correctly set up the required credentials and settings in the Google and Apple developer ecosystems to enable social sign-in functionalities for the game. This is a critical part of the integration process for these providers.
    
**Namespace:** PatternCipher.Services.Auth.Docs  
**Metadata:**
    
    - **Category:** Documentation
    
- **Path:** firebase/auth/compliance.policy.md  
**Description:** Documentation file that explicitly defines the compliance strategy for this service, particularly in relation to COPPA and GDPR-K as required by NFR-LC-002a.  
**Template:** Markdown Documentation  
**Dependency Level:** 1  
**Name:** compliance.policy  
**Type:** Documentation  
**Relative Path:** compliance.policy.md  
**Repository Id:** REPO-PATT-006  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - COPPA Compliance Strategy
    - GDPR-K Compliance Strategy
    
**Requirement Ids:**
    
    - NFR-LC-002a
    
**Purpose:** To provide a clear and auditable record of how the configured authentication providers align with child data privacy regulations.  
**Logic Description:** This document outlines the policy for handling users identified as children by the client-side age gate. It specifies that Anonymous Authentication is the primary method available to all users as it does not inherently collect PII. It mandates that Google Sign-In and Apple Sign-In, which involve sharing PII (like name and email), must only be offered to users who have passed the age gate, confirming they are 13 or older (or the applicable age of consent). This ensures that the service configuration is used in a compliant manner by the client application, fulfilling the requirements of NFR-LC-002a.  
**Documentation:**
    
    - **Summary:** Defines the operational policy for ensuring the use of Firebase Authentication complies with child data protection laws like COPPA. It acts as a bridge between the technical configuration and the legal requirements.
    
**Namespace:** PatternCipher.Services.Auth.Docs  
**Metadata:**
    
    - **Category:** Compliance
    


---

# 2. Configuration

- **Feature Toggles:**
  
  - enableAnonymousAuth
  - enableGoogleAuth
  - enableAppleAuth
  - enableAccountLinking
  
- **Database Configs:**
  
  


---

