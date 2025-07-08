# Specification

# 1. Logging And Observability Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - Unity
    - C#
    - Firebase (Firestore, Cloud Functions, Auth, Remote Config, Analytics, Crashlytics)
    - TypeScript
    - Google Cloud Logging
    
  - **Monitoring Requirements:**
    
    - NFR-R-001 (Crash Reporting)
    - NFR-M-005 (Client-Side Logging)
    - REQ-8-003 (Aggregated Error Reports)
    - REQ-8-009 (Client-Side Logging System)
    - NFR-OP-002 (Backend Monitoring)
    - NFR-AU-001 (Audit Logging)
    
  - **System Architecture:** Hybrid Client-Server (BaaS) with a layered client architecture (Presentation, Application, Domain, Infrastructure). Backend is serverless via Firebase.
  - **Environment:** production
  
- **Log Level And Category Strategy:**
  
  - **Default Log Level:** INFO
  - **Environment Specific Levels:**
    
    - **Environment:** development  
**Log Level:** DEBUG  
**Justification:** Provides maximum verbosity for developers during feature implementation and debugging.  
    - **Environment:** staging  
**Log Level:** DEBUG  
**Justification:** Allows for detailed analysis and troubleshooting during the QA and pre-release testing phase.  
    - **Environment:** production  
**Log Level:** INFO  
**Justification:** Captures significant operational events, warnings, and errors without the performance overhead of DEBUG logs, aligning with cost and performance NFRs.  
    
  - **Component Categories:**
    
    - **Component:** LevelManager  
**Category:** PCG  
**Log Level:** INFO  
**Verbose Logging:** False  
**Justification:** Log success/failure of procedural content generation. Failures are critical to investigate (REQ-PCGDS-002).  
    - **Component:** PersistenceService  
**Category:** DataAccess  
**Log Level:** WARN  
**Verbose Logging:** False  
**Justification:** Log non-critical save/load issues at WARN level. Log data corruption or migration failures (REQ-PDP-002, REQ-PDP-003) at ERROR level.  
    - **Component:** FirebaseServiceFacade  
**Category:** BackendComms  
**Log Level:** INFO  
**Verbose Logging:** False  
**Justification:** Log initiation and success/failure of critical backend operations like cloud save and leaderboard submission for tracing purposes.  
    - **Component:** LeaderboardFunction  
**Category:** BackendLogic.Validation  
**Log Level:** INFO  
**Verbose Logging:** False  
**Justification:** Logs the outcome of server-side validation for leaderboard integrity (REQ-CPS-012), providing an audit trail of submissions.  
    
  - **Sampling Strategies:**
    
    - **Component:** FirebaseAnalytics  
**Sampling Rate:** N/A (Handled by provider)  
**Condition:** Default provider sampling  
**Reason:** Firebase Analytics uses its own data sampling for reporting to manage cost and performance, which is acceptable for its intended use in trend analysis.  
    
  - **Logging Approach:**
    
    - **Structured:** True
    - **Format:** JSON
    - **Standard Fields:**
      
      - timestamp
      - logLevel
      - message
      - category
      - correlationId
      - playerId
      - appVersion
      
    - **Custom Fields:**
      
      - levelId
      - gameState
      - stackTrace
      
    
  
- **Log Aggregation Architecture:**
  
  - **Collection Mechanism:**
    
    - **Type:** agent
    - **Technology:** Firebase Crashlytics SDK (Client), Google Cloud Logging (Backend)
    - **Configuration:**
      
      
    - **Justification:** Leverages native, integrated solutions for the chosen technology stack. Firebase Crashlytics is designed for mobile app crash reporting (NFR-R-001). Google Cloud Logging is the default for Cloud Functions.
    
  - **Strategy:**
    
    - **Approach:** hybrid
    - **Reasoning:** Client-side errors/crashes are sent to a central aggregator (Crashlytics). Backend logs are inherently centralized in Google Cloud Logging. Diagnostic client logs remain local unless explicitly submitted by the user (REQ-8-009). This balances performance with observability needs.
    - **Local Retention:** Session-based or ~10MB file size limit for user-submitted logs.
    
  - **Shipping Methods:**
    
    - **Protocol:** HTTP  
**Destination:** Firebase Crashlytics / Google Cloud Logging  
**Reliability:** at-least-once  
**Compression:** True  
    
  - **Buffering And Batching:**
    
    - **Buffer Size:** Managed by SDK
    - **Batch Size:** 0
    - **Flush Interval:** Managed by SDK
    - **Backpressure Handling:** Handled by Firebase SDKs to prevent excessive network usage and battery drain.
    
  - **Transformation And Enrichment:**
    
    - **Transformation:** Add Contextual Fields  
**Purpose:** Enrich client-side logs with player, device, and application state for better debugging.  
**Stage:** collection  
    - **Transformation:** PII Scrubbing  
**Purpose:** Remove or mask sensitive user data before submission to comply with privacy requirements (REQ-11-007).  
**Stage:** collection  
    
  - **High Availability:**
    
    - **Required:** True
    - **Redundancy:** Managed by Google Cloud
    - **Failover Strategy:** Managed by Google Cloud
    
  
- **Retention Policy Design:**
  
  - **Retention Periods:**
    
    - **Log Type:** Crashlytics Logs  
**Retention Period:** 90 days  
**Justification:** Provides sufficient history for debugging recent releases and identifying trends in crashes and non-fatal errors.  
**Compliance Requirement:** NFR-R-001  
    - **Log Type:** Backend Diagnostic Logs (INFO, DEBUG)  
**Retention Period:** 30 days  
**Justification:** Standard retention for operational troubleshooting in Google Cloud Logging.  
**Compliance Requirement:** NFR-OP-002  
    - **Log Type:** Backend Audit Logs (Security, Admin Actions)  
**Retention Period:** 365 days  
**Justification:** Meets security and compliance requirements for maintaining an audit trail of critical events (NFR-AU-001).  
**Compliance Requirement:** NFR-AU-001  
    
  - **Compliance Requirements:**
    
    - **Regulation:** GDPR/COPPA  
**Applicable Log Types:**
    
    - All
    
**Minimum Retention:** N/A  
**Special Handling:** PII must be excluded or pseudonymized from all logs by default. Logs containing PII (e.g., user-submitted support logs) require explicit consent and have their own strict retention and access policies.  
    
  - **Volume Impact Analysis:**
    
    - **Estimated Daily Volume:** Low to Medium
    - **Storage Cost Projection:** Expected to be within the free tier of Google Cloud Logging and Firebase Crashlytics for the initial user base.
    - **Compression Ratio:** Handled by provider
    
  - **Storage Tiering:**
    
    - **Hot Storage:**
      
      - **Duration:** 30 days
      - **Accessibility:** immediate
      - **Cost:** high
      
    - **Warm Storage:**
      
      - **Duration:** N/A
      - **Accessibility:** minutes
      - **Cost:** medium
      
    - **Cold Storage:**
      
      - **Duration:** 365 days (for Audit Logs)
      - **Accessibility:** hours
      - **Cost:** low
      
    
  - **Compression Strategy:**
    
    - **Algorithm:** Managed by Google Cloud
    - **Compression Level:** N/A
    - **Expected Ratio:** N/A
    
  - **Anonymization Requirements:**
    
    - **Data Type:** PII  
**Method:** Exclude or Pseudonymize (using internal GUID)  
**Timeline:** At collection time  
**Compliance:** GDPR, COPPA  
    
  
- **Search Capability Requirements:**
  
  - **Essential Capabilities:**
    
    - **Capability:** Filter by logLevel, category, and timestamp  
**Performance Requirement:** <5s  
**Justification:** Basic requirement for triaging and troubleshooting issues.  
    - **Capability:** Full-text search on log message  
**Performance Requirement:** <10s  
**Justification:** Needed to find specific error messages or log entries without knowing the exact structure.  
    - **Capability:** Filter by correlationId  
**Performance Requirement:** <2s  
**Justification:** Critical for tracing a single user action across client and backend services.  
    
  - **Performance Characteristics:**
    
    - **Search Latency:** <10 seconds for most queries
    - **Concurrent Users:** 5
    - **Query Complexity:** simple
    - **Indexing Strategy:** Default indexing provided by Google Cloud Logging and Crashlytics.
    
  - **Indexed Fields:**
    
    - **Field:** logLevel  
**Index Type:** Keyword  
**Search Pattern:** Exact match  
**Frequency:** high  
    - **Field:** category  
**Index Type:** Keyword  
**Search Pattern:** Exact match  
**Frequency:** high  
    - **Field:** correlationId  
**Index Type:** Keyword  
**Search Pattern:** Exact match  
**Frequency:** medium  
    - **Field:** timestamp  
**Index Type:** Date  
**Search Pattern:** Range queries  
**Frequency:** high  
    
  - **Full Text Search:**
    
    - **Required:** True
    - **Fields:**
      
      - message
      - stackTrace
      
    - **Search Engine:** Google Cloud Logging Search
    - **Relevance Scoring:** True
    
  - **Correlation And Tracing:**
    
    - **Correlation Ids:**
      
      - correlationId
      
    - **Trace Id Propagation:** Manually propagated in event payloads.
    - **Span Correlation:** False
    - **Cross Service Tracing:** False
    
  - **Dashboard Requirements:**
    
    - **Dashboard:** Production Error Summary  
**Purpose:** At-a-glance view of error rates, crash-free users, and top error types (REQ-8-003, NFR-R-001).  
**Refresh Interval:** 15 minutes  
**Audience:** Development Team, Ops  
    - **Dashboard:** Backend Function Health  
**Purpose:** Monitor latency, invocation counts, and error rates for critical Cloud Functions (e.g., Leaderboard Submission) (NFR-OP-002).  
**Refresh Interval:** 5 minutes  
**Audience:** Backend Developers, Ops  
    
  
- **Storage Solution Selection:**
  
  - **Selected Technology:**
    
    - **Primary:** Google Cloud Logging & Firebase Crashlytics
    - **Reasoning:** These are the tightly integrated, native logging and crash reporting solutions for the selected Firebase and Google Cloud backend stack. They require minimal configuration and scale automatically.
    - **Alternatives:**
      
      - Datadog
      - New Relic
      
    
  - **Scalability Requirements:**
    
    - **Expected Growth Rate:** 100x initial user base within 12 months
    - **Peak Load Handling:** Handled automatically by the managed services.
    - **Horizontal Scaling:** True
    
  - **Cost Performance Analysis:**
    
    - **Solution:** Google Cloud Logging & Crashlytics  
**Cost Per Gb:** Follows Google Cloud's pricing; generous free tier is sufficient for launch.  
**Query Performance:** Excellent for indexed fields; adequate for full-text search.  
**Operational Complexity:** low  
    
  - **Backup And Recovery:**
    
    - **Backup Frequency:** Managed by Google Cloud
    - **Recovery Time Objective:** N/A (Logs are for diagnostics, not state)
    - **Recovery Point Objective:** N/A
    - **Testing Frequency:** N/A
    
  - **Geo Distribution:**
    
    - **Required:** False
    - **Regions:**
      
      
    - **Replication Strategy:** Managed by Google Cloud within the selected project region.
    
  - **Data Sovereignty:**
    
    
  
- **Access Control And Compliance:**
  
  - **Access Control Requirements:**
    
    - **Role:** Developer  
**Permissions:**
    
    - read
    
**Log Types:**
    
    - All
    
**Justification:** Required for debugging and troubleshooting issues in all environments.  
    - **Role:** Ops/Admin  
**Permissions:**
    
    - read
    - configure
    
**Log Types:**
    
    - All
    
**Justification:** Required to manage log retention policies, alerting, and access controls.  
    - **Role:** Support (Tier 2)  
**Permissions:**
    
    - read
    
**Log Types:**
    
    - User-Submitted Diagnostic Logs
    
**Justification:** Limited access to user-consented logs to resolve specific support tickets.  
    
  - **Sensitive Data Handling:**
    
    - **Data Type:** PII  
**Handling Strategy:** exclude  
**Fields:**
    
    - email
    - realName
    - devicePersistentId
    
**Compliance Requirement:** GDPR, COPPA, REQ-11-007  
    - **Data Type:** Player ID  
**Handling Strategy:** tokenize  
**Fields:**
    
    - playerId
    
**Compliance Requirement:** Logs should use an internal, pseudonymized player GUID, not the Firebase Auth UID directly where possible.  
    
  - **Encryption Requirements:**
    
    - **In Transit:**
      
      - **Required:** True
      - **Protocol:** TLS 1.2+
      - **Certificate Management:** Managed by Google Cloud
      
    - **At Rest:**
      
      - **Required:** True
      - **Algorithm:** AES-256
      - **Key Management:** Managed by Google Cloud
      
    
  - **Audit Trail:**
    
    - **Log Access:** True
    - **Retention Period:** 400 days (GCP Default)
    - **Audit Log Location:** Google Cloud Audit Logs
    - **Compliance Reporting:** True
    
  - **Regulatory Compliance:**
    
    - **Regulation:** GDPR  
**Applicable Components:**
    
    - All
    
**Specific Requirements:**
    
    - PII Exclusion
    - User Consent for Diagnostics
    
**Evidence Collection:** Code reviews, logging policy documents, consent logs.  
    - **Regulation:** COPPA  
**Applicable Components:**
    
    - All
    
**Specific Requirements:**
    
    - PII Exclusion for under-13 users by default.
    
**Evidence Collection:** Age-gate implementation review, SDK configuration checks.  
    
  - **Data Protection Measures:**
    
    - **Measure:** Automated PII Scrubbing Review  
**Implementation:** Incorporate log content checks into PR review process to prevent accidental PII logging.  
**Monitoring Required:** False  
    
  
- **Project Specific Logging Config:**
  
  - **Logging Config:**
    
    - **Level:** INFO
    - **Retention:** 30 days (default)
    - **Aggregation:** Google Cloud Logging & Firebase Crashlytics
    - **Storage:** Managed by GCP
    - **Configuration:**
      
      
    
  - **Component Configurations:**
    
    - **Component:** PersistenceService  
**Log Level:** WARN  
**Output Format:** JSON  
**Destinations:**
    
    - Crashlytics
    
**Sampling:**
    
    - **Enabled:** False
    - **Rate:** 1.0
    
**Custom Fields:**
    
    - saveFilePath
    - errorCode
    
    - **Component:** LeaderboardFunction  
**Log Level:** INFO  
**Output Format:** JSON  
**Destinations:**
    
    - GoogleCloudLogging
    
**Sampling:**
    
    - **Enabled:** False
    - **Rate:** 1.0
    
**Custom Fields:**
    
    - leaderboardId
    - validationResult
    
    - **Component:** LevelManager  
**Log Level:** INFO  
**Output Format:** JSON  
**Destinations:**
    
    - Crashlytics
    
**Sampling:**
    
    - **Enabled:** False
    - **Rate:** 1.0
    
**Custom Fields:**
    
    - levelId
    - pcgSeed
    
    
  - **Metrics:**
    
    - **Custom Metrics:**
      
      
    
  - **Alert Rules:**
    
    - **Name:** High_Backend_Function_Error_Rate  
**Condition:** count(log.severity >= ERROR WHERE component='LeaderboardFunction') > 5 in 5m  
**Severity:** High  
**Actions:**
    
    - **Type:** email  
**Target:** backend-dev-alerts@patterncipher.com  
**Configuration:**
    
    
    
**Suppression Rules:**
    
    
**Escalation Path:**
    
    
    - **Name:** Save_Data_Corruption_Spike  
**Condition:** count(log.message CONTAINS 'Checksum validation failed') > 10 in 1h  
**Severity:** Critical  
**Actions:**
    
    - **Type:** pagerduty  
**Target:** on-call-team  
**Configuration:**
    
    
    
**Suppression Rules:**
    
    
**Escalation Path:**
    
    
    
  
- **Implementation Priority:**
  
  - **Component:** Firebase Crashlytics Integration  
**Priority:** high  
**Dependencies:**
    
    
**Estimated Effort:** Low  
**Risk Level:** low  
  - **Component:** Backend Cloud Function Logging  
**Priority:** high  
**Dependencies:**
    
    - Cloud Function Development
    
**Estimated Effort:** Low  
**Risk Level:** low  
  - **Component:** Client-Side Structured Logging Framework  
**Priority:** medium  
**Dependencies:**
    
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** User-Submitted Diagnostic Log Feature  
**Priority:** low  
**Dependencies:**
    
    - Client-Side Structured Logging Framework
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  
- **Risk Assessment:**
  
  - **Risk:** PII leakage in logs violates privacy regulations (GDPR/COPPA).  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Implement strict PII scrubbing policies in the logging framework. Conduct regular code reviews and use static analysis tools to find potential leaks. Pseudonymize user identifiers.  
**Contingency Plan:** If a leak is discovered, immediately patch the code, notify the DPO, purge the affected logs, and follow the incident response plan which may include user notification.  
  - **Risk:** Excessive client-side logging impacts game performance and battery life.  
**Impact:** medium  
**Probability:** medium  
**Mitigation:** Set default production log level to INFO. Avoid logging in performance-critical loops (e.g., Update()). Use performance profiling during development to measure logging overhead.  
**Contingency Plan:** Deploy a remote config update to raise the client-side logging threshold (e.g., to ERROR only) to immediately mitigate performance impact while a patch is prepared.  
  - **Risk:** Inadequate logging on the backend prevents effective troubleshooting of critical online features like cloud save or leaderboards.  
**Impact:** high  
**Probability:** low  
**Mitigation:** Enforce structured logging with correlation IDs for all backend functions. Ensure key decision points and error states in the server-side logic are logged.  
**Contingency Plan:** If an issue cannot be diagnosed, deploy an update to the relevant Cloud Function with temporary, more verbose logging to capture the necessary details for a specific user or scenario.  
  
- **Recommendations:**
  
  - **Category:** Implementation  
**Recommendation:** Implement a lightweight, structured logging wrapper for the client (Unity) that standardizes log format and automatically enriches logs with context (playerId, appVersion, correlationId).  
**Justification:** Ensures consistency across all client-side logging, simplifies log analysis, and makes it easier to enforce policies like PII scrubbing.  
**Priority:** high  
**Implementation Notes:** This can be a static class, e.g., `GameLogger.Info(category, message, customData)`, that formats the log as JSON and passes it to `Debug.Log` and `Crashlytics.Log`.  
  - **Category:** Observability  
**Recommendation:** Adopt and enforce the use of a `correlationId` for any user action that results in a backend call.  
**Justification:** This is the single most effective way to trace a distributed request from the client through backend functions and database interactions, which is critical for debugging complex online features.  
**Priority:** high  
**Implementation Notes:** The client should generate a GUID when an action starts (e.g., level complete screen appears) and pass this ID in all subsequent API/event payloads for that action.  
  - **Category:** Compliance  
**Recommendation:** Create a formal, documented 'PII in Logs' policy that explicitly lists what is considered PII and is forbidden in logs. Integrate a check against this policy into the code review checklist.  
**Justification:** Moves privacy compliance from an ad-hoc practice to a formal, auditable process, reducing the risk of accidental data leaks (REQ-11-007).  
**Priority:** medium  
**Implementation Notes:** The policy should be stored in a shared repository (e.g., Confluence) and linked in the PR template.  
  


---

