# Specification

# 1. Sequence Design Overview

- **Sequence Diagram:**
  ### . Player Authentication Flow
  JWT-based authentication and session management

  #### .4. Purpose
  Secure access to game services

  #### .5. Type
  AuthenticationFlow

  #### .6. Participant Repository Ids
  
  - REPO-MOBILE-CLIENT
  - REPO-SECURITY
  - REPO-BACKEND-API
  
  #### .7. Key Interactions
  
  - Client initiates platform authentication
  - Security service validates credentials
  - Backend API issues JWT token
  - Client stores encrypted session
  
  #### .8. Related Feature Ids
  
  - REQ-SEC-001
  - REQ-SEC-008
  
  #### .9. Domain
  Security

  #### .10. Metadata
  
  - **Complexity:** Medium
  - **Priority:** High
  


---

# 2. Sequence Diagram Details
## 2. Player Authentication Flow
Detailed JWT-based authentication and session management, showing identity verification, token issuance, and secure client-side session storage.

### 2.1. Diagram Id
SD-AUTH-FLOW

### 2.4. Participants

- **Repository Id:** REPO-MOBILE-CLIENT  
**Display Name:** Mobile Client  
**Type:** MobileApplication  
**Technology:** Native (iOS/Android) or Cross-Platform (e.g., Unity)  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #DDDDDD
    - **Stereotype:** «MobileClient»
    
- **Repository Id:** REPO-BACKEND-API  
**Display Name:** Backend API  
**Type:** APIService  
**Technology:** Node.js/Express.js  
**Order:** 2  
**Style:**
    
    - **Shape:** rectangle
    - **Color:** #ADD8E6
    - **Stereotype:** «APIService»
    
- **Repository Id:** REPO-SECURITY  
**Display Name:** Security Service  
**Type:** Microservice  
**Technology:** Authentication/Authorization Service (e.g., JWT-based, OAuth2)  
**Order:** 3  
**Style:**
    
    - **Shape:** rectangle
    - **Color:** #FFCCCB
    - **Stereotype:** «SecurityService»
    

### 2.5. Interactions

- **Source Id:** REPO-MOBILE-CLIENT  
**Target Id:** REPO-BACKEND-API  
**Message:** POST /auth/login (credentials or platformAuthToken)  
**Sequence Number:** 1  
**Type:** Request  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    
- **Source Id:** REPO-BACKEND-API  
**Target Id:** REPO-SECURITY  
**Message:** validateCredentials(credentials or platformAuthToken)  
**Sequence Number:** 2  
**Type:** Request  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    
- **Source Id:** REPO-SECURITY  
**Target Id:** REPO-SECURITY  
**Message:** Perform credential/token validation logic  
**Sequence Number:** 3  
**Type:** Loop  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    
- **Source Id:** REPO-SECURITY  
**Target Id:** REPO-BACKEND-API  
**Message:** ValidationResult(isValid, userId, userRoles)  
**Sequence Number:** 4  
**Type:** Response  
**Is Synchronous:** True  
**Return Message:** ValidationResult(isValid, userId, userRoles)  
**Has Return:** True  
**Is Activation:** False  
**Nested Interactions:**
    
    
- **Source Id:** REPO-BACKEND-API  
**Target Id:** REPO-BACKEND-API  
**Message:** [isValid === true]  
**Sequence Number:** 5  
**Type:** Alternative  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    - **Source Id:** REPO-BACKEND-API  
**Target Id:** REPO-BACKEND-API  
**Message:** Generate JWT(userId, userRoles)  
**Sequence Number:** 5.1  
**Type:** Request  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    
    - **Source Id:** REPO-BACKEND-API  
**Target Id:** REPO-MOBILE-CLIENT  
**Message:** HTTP 200 OK (jwtToken, sessionDetails)  
**Sequence Number:** 5.2  
**Type:** Response  
**Is Synchronous:** True  
**Return Message:** jwtToken, sessionDetails  
**Has Return:** True  
**Is Activation:** False  
**Nested Interactions:**
    
    
    - **Source Id:** REPO-MOBILE-CLIENT  
**Target Id:** REPO-MOBILE-CLIENT  
**Message:** Store JWT securely (e.g., encrypted storage)  
**Sequence Number:** 5.3  
**Type:** Request  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    
    
- **Source Id:** REPO-BACKEND-API  
**Target Id:** REPO-BACKEND-API  
**Message:** [else (isValid === false)]  
**Sequence Number:** 6  
**Type:** Alternative  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Nested Interactions:**
    
    - **Source Id:** REPO-BACKEND-API  
**Target Id:** REPO-MOBILE-CLIENT  
**Message:** HTTP 401 Unauthorized (Authentication Failed)  
**Sequence Number:** 6.1  
**Type:** Response  
**Is Synchronous:** True  
**Return Message:** Error: Authentication Failed  
**Has Return:** True  
**Is Activation:** False  
**Nested Interactions:**
    
    
    

### 2.6. Notes

- **Content:** Security Service may interact with external Identity Providers (e.g., Apple/Google) if platform authentication token is used.  
**Position:** over  
**Participant Id:** REPO-SECURITY  
**Sequence Number:** 3  
- **Content:** Backend API uses a secure signing key and configured claims (e.g., expiry, issuer, subject, roles) to issue the JWT.  
**Position:** over  
**Participant Id:** REPO-BACKEND-API  
**Sequence Number:** 5.1  
- **Content:** Client stores JWT (e.g., in Keychain/Keystore or SharedPreferences with encryption) for use in 'Authorization: Bearer <JWT>' header in subsequent API calls.  
**Position:** over  
**Participant Id:** REPO-MOBILE-CLIENT  
**Sequence Number:** 5.3  

### 2.7. Mermaid Diagram
sequenceDiagram
  participant MC as "REPO-MOBILE-CLIENT (Mobile Client)"
  participant BA as "REPO-BACKEND-API (Backend API)"
  participant SEC as "REPO-SECURITY (Security Service)"

  activate MC
  MC->>+BA: POST /auth/login (credentials/platformAuthToken)
  Note over BA: Receives authentication request
  BA->>+SEC: validateCredentials(credentials/platformAuthToken)
  activate SEC
  SEC->>SEC: Perform credential/token validation logic
  Note over SEC: Security Service may interact with external Identity Providers (e.g., Apple/Google) if platform authentication token is used.
  SEC-->>-BA: ValidationResult(isValid, userId, userRoles)
  deactivate SEC

  alt isValid === true
    BA->>BA: Generate JWT(userId, userRoles)
    Note over BA: Backend API uses a secure signing key and configured claims (e.g., expiry, issuer, subject, roles) to issue the JWT.
    BA-->>MC: HTTP 200 OK (jwtToken, sessionDetails)
    activate MC
    MC->>MC: Store JWT securely (e.g., encrypted storage)
    Note over MC: Client stores JWT (e.g., in Keychain/Keystore or SharedPreferences with encryption) for use in 'Authorization: Bearer <JWT>' header in subsequent API calls.
    deactivate MC
  else isValid === false
    BA-->>MC: HTTP 401 Unauthorized (Authentication Failed)
  end
  deactivate BA
  deactivate MC


---

