# Specification

# 1. Pipelines

## 1.1. Unity Client Build & Test Pipeline
Builds, tests, and prepares Android and iOS release candidates for Pattern Cipher. Fulfills requirements NFR-QA-001 and TR-ID-002.

### 1.1.4. Stages

### 1.1.4.1. Code Quality & Security
#### 1.1.4.1.2. Steps

- run-static-analysis --tool=roslyn
- run-dependency-scan --tool=snyk

#### 1.1.4.1.3. Environment

- **Fail_On_Severity:** critical

#### 1.1.4.1.4. Quality Gates

- **Name:** Vulnerability Scan  
**Criteria:**
    
    - zero critical CVEs
    
**Blocking:** True  
- **Name:** Static Analysis  
**Criteria:**
    
    - no new major issues
    
**Blocking:** True  

### 1.1.4.2. Unit & Integration Tests
#### 1.1.4.2.2. Steps

- unity-run-tests -testPlatform editmode
- unity-run-tests -testPlatform playmode
- generate-coverage-report

#### 1.1.4.2.3. Environment


#### 1.1.4.2.4. Quality Gates

- **Name:** Test Results  
**Criteria:**
    
    - all tests passed
    
**Blocking:** True  
- **Name:** Code Coverage  
**Criteria:**
    
    - critical module coverage >= 80%
    
**Blocking:** True  

### 1.1.4.3. Build Artifacts (Staging)
#### 1.1.4.3.2. Steps

- unity-build -target Android -config staging
- unity-build -target iOS -config staging
- sign-artifact -platform Android
- sign-artifact -platform iOS
- upload-artifact -path build/Android/PatternCipher.aab
- upload-artifact -path build/iOS/PatternCipher.ipa

#### 1.1.4.3.3. Environment

- **Build_Config:** staging
- **Backend_Endpoint:** https://api.staging.patterncipher.com

### 1.1.4.4. Deploy to Test Tracks
#### 1.1.4.4.2. Steps

- deploy-google-play -track internal -artifact PatternCipher.aab
- deploy-testflight -artifact PatternCipher.ipa

#### 1.1.4.4.3. Environment

- **Release_Notes:** Automated build from main branch.

#### 1.1.4.4.4. Quality Gates

- **Name:** Manual QA Go/No-Go  
**Criteria:**
    
    - QA Team approval for release candidate
    
**Blocking:** True  


## 1.2. Firebase Functions Deployment Pipeline
Lints, tests, and deploys backend serverless functions to staging and production environments as per requirement 2.6.3.

### 1.2.4. Stages

### 1.2.4.1. Lint, Test & Security Scan
#### 1.2.4.1.2. Steps

- npm install
- npm run lint
- npm run test
- npm audit --audit-level=critical

#### 1.2.4.1.3. Environment


#### 1.2.4.1.4. Quality Gates

- **Name:** Code Quality Checks  
**Criteria:**
    
    - all tests passed
    - linting successful
    
**Blocking:** True  
- **Name:** Vulnerability Check  
**Criteria:**
    
    - zero critical CVEs
    
**Blocking:** True  

### 1.2.4.2. Deploy to Staging
#### 1.2.4.2.2. Steps

- firebase use pattern-cipher-staging
- firebase deploy --only functions

#### 1.2.4.2.3. Environment

- **Firebase_Token:** ${STAGING_FIREBASE_TOKEN}

### 1.2.4.3. Deploy to Production
#### 1.2.4.3.2. Steps

- firebase use pattern-cipher-prod
- firebase deploy --only functions

#### 1.2.4.3.3. Environment

- **Firebase_Token:** ${PROD_FIREBASE_TOKEN}

#### 1.2.4.3.4. Quality Gates

- **Name:** Manual Production Approval  
**Criteria:**
    
    - Product Owner approval received
    
**Blocking:** True  




---

# 2. Configuration

- **Artifact Repository:** Google Cloud Artifact Registry
- **Default Branch:** main
- **Retention Policy:** 60d
- **Notification Channel:** slack#game-deployments


---

