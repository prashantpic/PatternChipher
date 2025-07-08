# Specification

# 1. Data Transformation Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - Unity (C#)
    - Firebase Firestore
    - Firebase Cloud Functions
    - Firebase Remote Config
    - JSON
    
  - **Service Interfaces:**
    
    - Firebase SDKs (Firestore, Auth, Remote Config, Analytics)
    - Custom Cloud Functions (HTTPS/JSON)
    
  - **Data Models:**
    
    - PlayerProfile (Client Domain)
    - UserProfile (Firestore)
    - LeaderboardEntry
    - AnalyticsEvent
    
  
- **Data Mapping Strategy:**
  
  - **Essential Mappings:**
    
    - **Mapping Id:** M-001  
**Source:** Client-Domain-PlayerProfile  
**Target:** Backend-Firestore-UserProfile  
**Transformation:** nested  
**Configuration:**
    
    
**Mapping Technique:** Object-to-JSON Serialization  
**Justification:** Core transformation for the optional cloud save feature, synchronizing local player progress to the backend. REQ-10-007, REQ-10-008.  
**Complexity:** medium  
    - **Mapping Id:** M-002  
**Source:** Client-Domain-GameCompletionState  
**Target:** Backend-Firestore-LeaderboardEntry  
**Transformation:** flattened  
**Configuration:**
    
    
**Mapping Technique:** Direct field mapping  
**Justification:** Transforms in-memory game results into a structured format for leaderboard submission. REQ-9-002, REQ-SRP-008.  
**Complexity:** simple  
    - **Mapping Id:** M-003  
**Source:** Client-Application-GameEvent  
**Target:** Backend-Analytics-Event  
**Transformation:** flattened  
**Configuration:**
    
    
**Mapping Technique:** Direct field mapping with a nested JSON payload  
**Justification:** Formats gameplay events for collection by the Firebase Analytics service for balancing and analysis. REQ-8-001, REQ-8-002.  
**Complexity:** simple  
    - **Mapping Id:** M-004  
**Source:** Backend-RemoteConfig  
**Target:** Client-Domain-GameConfiguration  
**Transformation:** direct  
**Configuration:**
    
    
**Mapping Technique:** Key-value mapping  
**Justification:** Applies dynamically fetched game parameters to the client's internal configuration models for live game balancing. REQ-8-006.  
**Complexity:** simple  
    
  - **Object To Object Mappings:**
    
    - **Source Object:** Client-Side PlayerProfile C# Object  
**Target Object:** Firestore UserProfile.cloud_save_data_object Map  
**Field Mappings:**
    
    - **Source Field:** TotalStars  
**Target Field:** totalStars  
**Transformation:** Direct  
**Data Type Conversion:** C# int to Firestore Number  
    - **Source Field:** PlayerLevelProgress  
**Target Field:** levelProgress  
**Transformation:** Object to Map/JSON serialization  
**Data Type Conversion:** C# Dictionary to Firestore Map  
    - **Source Field:** GameSettings  
**Target Field:** settings  
**Transformation:** Object to Map/JSON serialization  
**Data Type Conversion:** C# Object to Firestore Map  
    
    
  - **Data Type Conversions:**
    
    - **From:** C# System.DateTime  
**To:** Firestore Server Timestamp  
**Conversion Method:** Firebase SDK automatic conversion using FieldValue.serverTimestamp()  
**Validation Required:** False  
    - **From:** C# Guid  
**To:** Firestore String  
**Conversion Method:** ToString() method  
**Validation Required:** True  
    
  - **Bidirectional Mappings:**
    
    - **Entity:** Player Progress (Cloud Save)  
**Forward Mapping:** M-001 (Client-to-Backend)  
**Reverse Mapping:** M-001R (Backend-to-Client)  
**Consistency Strategy:** Last Write Wins based on server-side timestamp comparison as per REQ-10-010.  
    
  
- **Schema Validation Requirements:**
  
  - **Field Level Validations:**
    
    - **Field:** LeaderboardEntry.scoreValue  
**Rules:**
    
    - IsInteger
    - IsNonNegative
    
**Priority:** critical  
**Error Message:** Invalid score submitted.  
    - **Field:** Player.ageGateStatus  
**Rules:**
    
    - IsIn(UNKNOWN, UNDER_AGE_OF_CONSENT, OVER_AGE_OF_CONSENT)
    
**Priority:** critical  
**Error Message:** Invalid age gate status.  
    - **Field:** GameSettings.bgmVolume  
**Rules:**
    
    - IsDecimal
    - IsInRange(0.0, 1.0)
    
**Priority:** high  
**Error Message:** Volume must be between 0.0 and 1.0.  
    
  - **Cross Field Validations:**
    
    - **Validation Id:** V-CF-001  
**Fields:**
    
    - LeaderboardEntry.scoreValue
    - LeaderboardEntry.levelDefinitionId
    
**Rule:** Submitted score must be plausible for the given level. This involves fetching the LevelDefinition's parMoves and max possible score and comparing against the submitted scoreValue.  
**Condition:** On all LeaderboardEntry submissions.  
**Error Handling:** Reject submission and log to a 'failed_submissions' collection.  
    
  - **Business Rule Validations:**
    
    - **Rule Id:** BRV-001  
**Description:** Validate leaderboard score against level par and maximum possible score.  
**Fields:**
    
    - LeaderboardEntry.scoreValue
    - LevelDefinition.parMoves
    
**Logic:** scoreValue must be less than a calculated theoretical maximum for the level and greater than a theoretical minimum. Implemented in a Cloud Function as per REQ-CPS-012.  
**Priority:** critical  
    
  - **Conditional Validations:**
    
    
  - **Validation Groups:**
    
    - **Group Name:** LeaderboardSubmission  
**Validations:**
    
    - fieldLevelValidations.LeaderboardEntry
    - BRV-001
    
**Execution Order:** 1  
**Stop On First Failure:** True  
    
  
- **Transformation Pattern Evaluation:**
  
  - **Selected Patterns:**
    
    - **Pattern:** adapter  
**Use Case:** The FirebaseService class in the client's Infrastructure layer adapts application-layer requests into Firebase SDK-specific calls, decoupling the game logic from the Firebase implementation.  
**Implementation:** C# class implementing a domain-specific interface (e.g., ICloudStorage) with Firebase SDKs.  
**Justification:** Follows the Layered Architecture design to isolate external dependencies.  
    - **Pattern:** converter  
**Use Case:** Serializing and deserializing C# PlayerProfile objects to and from JSON for local persistence and Firestore storage.  
**Implementation:** Newtonsoft.Json or a similar library to handle object-JSON conversion.  
**Justification:** Essential for data persistence and cloud synchronization as per REQ-PDP-001 and REQ-10-008.  
    - **Pattern:** pipeline  
**Use Case:** Leaderboard score submission involves a series of steps: client submission, Firestore trigger, Cloud Function validation, and final write to the leaderboard collection.  
**Implementation:** Client SDK call -> Firestore Write -> Cloud Function Trigger -> Function Execution.  
**Justification:** Provides a robust, server-authoritative way to process and validate user-submitted data, fulfilling REQ-CPS-012.  
    
  - **Pipeline Processing:**
    
    - **Required:** True
    - **Stages:**
      
      - **Stage:** Client Submission  
**Transformation:** GameResultToLeaderboardEntry  
**Dependencies:**
    
    
      - **Stage:** Server-Side Validation  
**Transformation:** LeaderboardScorePlausibilityCheck  
**Dependencies:**
    
    - Client Submission
    
      - **Stage:** Final Persistence  
**Transformation:** WriteToLeaderboard  
**Dependencies:**
    
    - Server-Side Validation
    
      
    - **Parallelization:** False
    
  - **Processing Mode:**
    
    - **Real Time:**
      
      - **Required:** True
      - **Scenarios:**
        
        - Cloud Save synchronization
        - Leaderboard score submission
        
      - **Latency Requirements:** < 2 seconds for user-facing operations
      
    - **Batch:**
      
      - **Required:** True
      - **Batch Size:** 25
      - **Frequency:** On app background or every 10 minutes
      - **Scenarios:**
        
        - Analytics event transmission
        
      
    - **Streaming:**
      
      - **Required:** False
      - **Streaming Framework:** N/A
      - **Windowing Strategy:** N/A
      
    
  - **Canonical Data Model:**
    
    - **Applicable:** True
    - **Scope:**
      
      - PlayerProfile
      
    - **Benefits:**
      
      - Ensures consistency between local save data and cloud save data.
      - Decouples client-side domain logic from the specific schema of the backend database (Firestore).
      
    
  
- **Version Handling Strategy:**
  
  - **Schema Evolution:**
    
    - **Strategy:** Schema-on-Read with explicit migration logic.
    - **Versioning Scheme:** Semantic versioning (e.g., 1.0, 1.1) stored within the save file.
    - **Compatibility:**
      
      - **Backward:** True
      - **Forward:** False
      - **Reasoning:** The application must be able to read and migrate older save file versions to the current version. It is not expected to handle future, unknown versions.
      
    
  - **Transformation Versioning:**
    
    - **Mechanism:** Application code versioning.
    - **Version Identification:** The migration logic is tied to the `save_schema_version` field in the save data.
    - **Migration Strategy:** Run a sequence of migration scripts upon detecting an older save version at app launch.
    
  - **Data Model Changes:**
    
    - **Migration Path:** Sequential transformation scripts (v1.0 -> v1.1, v1.1 -> v2.0, etc.).
    - **Rollback Strategy:** Not supported for local save migration. The original file is backed up before migration is attempted. On failure, the backup is restored.
    - **Validation Strategy:** Run post-migration checks to ensure key progress data (e.g., total stars, unlocked levels) is consistent.
    
  - **Schema Registry:**
    
    - **Required:** False
    - **Technology:** N/A
    - **Governance:** Schemas are documented as C# classes and in a markdown file within the source code repository.
    
  
- **Performance Optimization:**
  
  - **Critical Requirements:**
    
    - **Operation:** Cloud Save Synchronization  
**Max Latency:** 2000ms  
**Throughput Target:** N/A  
**Justification:** Must complete quickly during app close to not frustrate the user or risk being terminated by the OS.  
    - **Operation:** Remote Config Fetch  
**Max Latency:** 1500ms  
**Throughput Target:** N/A  
**Justification:** Must complete during initial app load without significantly delaying access to the main menu.  
    
  - **Parallelization Opportunities:**
    
    - **Transformation:** LevelCompleted event processing  
**Parallelization Strategy:** Asynchronous API calls from the client  
**Expected Gain:** Reduces total time to update multiple backend systems (progress, leaderboards, achievements) by running them concurrently instead of sequentially.  
    
  - **Caching Strategies:**
    
    - **Cache Type:** Client-side (in-memory and disk)  
**Cache Scope:** Remote Config values  
**Eviction Policy:** Time-based (e.g., 12 hours)  
**Applicable Transformations:**
    
    - M-004
    
    - **Cache Type:** Client-side (in-memory)  
**Cache Scope:** Frequently accessed game settings  
**Eviction Policy:** Session-based  
**Applicable Transformations:**
    
    
    
  - **Memory Optimization:**
    
    - **Techniques:**
      
      - Use of struct for simple data models where applicable.
      - Efficient JSON serialization settings to ignore null values.
      
    - **Thresholds:** N/A
    - **Monitoring Required:** False
    
  - **Lazy Evaluation:**
    
    - **Applicable:** False
    - **Scenarios:**
      
      
    - **Implementation:** N/A
    
  - **Bulk Processing:**
    
    - **Required:** True
    - **Batch Sizes:**
      
      - **Optimal:** 25
      - **Maximum:** 100
      
    - **Parallelism:** 1
    - **Scenarios:**
      
      - Uploading analytics events to Firebase.
      
    
  
- **Error Handling And Recovery:**
  
  - **Error Handling Strategies:**
    
    - **Error Type:** Network Unavailability  
**Strategy:** Graceful Degradation  
**Fallback Action:** Disable online features (leaderboards, cloud save) and notify the user with a non-intrusive UI element. Queue up non-critical data (analytics) for later submission.  
**Escalation Path:**
    
    
    - **Error Type:** Data Validation Failure (Server)  
**Strategy:** Reject and Log  
**Fallback Action:** The Cloud Function rejects the write operation. The failed data and error reason are written to a Dead Letter Queue (DLQ) collection in Firestore.  
**Escalation Path:**
    
    - DLQ Alert
    - On-call Engineer
    
    - **Error Type:** Local Save Data Migration Failure  
**Strategy:** Restore and Notify  
**Fallback Action:** Restore the pre-migration backup of the save file. Notify the user that progress from the old version could not be loaded and offer a choice to reset or contact support.  
**Escalation Path:**
    
    - Log Error to Analytics
    
    
  - **Logging Requirements:**
    
    - **Log Level:** error
    - **Included Data:**
      
      - correlationId
      - errorMessage
      - stackTrace (server-side)
      - failedEventPayload
      
    - **Retention Period:** 30 days
    - **Alerting:** True
    
  - **Partial Success Handling:**
    
    - **Strategy:** Allowed for independent operations.
    - **Reporting Mechanism:** Log warnings for failed optional operations.
    - **Recovery Actions:**
      
      - Example: If a player completes a level, saving local progress is critical and must succeed. The subsequent leaderboard submission is optional; if it fails, the local progress is not rolled back. The submission is retried locally.
      
    
  - **Circuit Breaking:**
    
    - **Required:** True
    - **Dependency:** Firebase Backend Services
    - **Threshold:** 5 consecutive failures
    - **Timeout:** 60 seconds
    - **Fallback Strategy:** Temporarily disable all online features and use local data only. Display a 'Connecting...' or 'Offline' message to the user.
    
  - **Retry Strategies:**
    
    - **Operation:** Cloud Save Sync / Leaderboard Submission  
**Max Retries:** 3  
**Backoff Strategy:** exponential  
**Retry Conditions:**
    
    - 5xx server errors
    - Network timeout
    
    
  - **Error Notifications:**
    
    - **Condition:** DLQ size > 0  
**Recipients:**
    
    - backend-dev-alerts@patterncipher.com
    
**Severity:** high  
**Channel:** Email, PagerDuty  
    
  
- **Project Specific Transformations:**
  
  ### .1. LocalPlayerStateToCloudSave
  Transforms the client-side C# player profile object into a JSON/Map format suitable for storage in a Firestore document for cloud synchronization.

  #### .1.1. Transformation Id
  PST-001

  #### .1.4. Source
  
  - **Service:** Client: Domain Layer
  - **Model:** PlayerProfile
  - **Fields:**
    
    - TotalStars
    - PlayerLevelProgress
    - GameSettings
    
  
  #### .1.5. Target
  
  - **Service:** Backend: Firebase Services
  - **Model:** UserProfile.cloud_save_data_object
  - **Fields:**
    
    - totalStars
    - levelProgress
    - settings
    
  
  #### .1.6. Transformation
  
  - **Type:** nested
  - **Logic:** Serializes the C# PlayerProfile object and its nested objects into a single JSON object. Also includes schema version and timestamps.
  - **Configuration:**
    
    
  
  #### .1.7. Frequency
  on-demand

  #### .1.8. Criticality
  high

  #### .1.9. Dependencies
  
  - REQ-10-007
  - REQ-10-008
  
  #### .1.10. Validation
  
  - **Pre Transformation:**
    
    
  - **Post Transformation:**
    
    - Schema conformance check against DM-002
    
  
  #### .1.11. Performance
  
  - **Expected Volume:** Low (per user)
  - **Latency Requirement:** < 500ms
  - **Optimization Strategy:** Only transform and sync changed data where possible.
  
  ### .2. GameResultToLeaderboardEntry
  Transforms the results of a completed level into a standardized LeaderboardEntry object for server-side submission and validation.

  #### .2.1. Transformation Id
  PST-002

  #### .2.4. Source
  
  - **Service:** Client: Application Layer
  - **Model:** GameCompletionState (In-memory)
  - **Fields:**
    
    - FinalScore
    - MovesTaken
    - TimeSeconds
    
  
  #### .2.5. Target
  
  - **Service:** Backend: Firebase Services
  - **Model:** LeaderboardEntry (Submission Payload)
  - **Fields:**
    
    - scoreValue
    - levelDefinitionId
    - clientTimestamp
    
  
  #### .2.6. Transformation
  
  - **Type:** flattened
  - **Logic:** Directly maps game result metrics to the leaderboard submission payload fields.
  - **Configuration:**
    
    
  
  #### .2.7. Frequency
  real-time

  #### .2.8. Criticality
  high

  #### .2.9. Dependencies
  
  - REQ-9-002
  - REQ-CPS-012
  
  #### .2.10. Validation
  
  - **Pre Transformation:**
    
    - Check for valid user authentication
    
  - **Post Transformation:**
    
    - Server-side plausibility validation (BRV-001)
    
  
  #### .2.11. Performance
  
  - **Expected Volume:** Medium
  - **Latency Requirement:** < 200ms for transformation, < 1000ms for full round trip
  - **Optimization Strategy:** Minimal client-side logic.
  
  ### .3. RemoteConfigToClientSettings
  Transforms the key-value data fetched from Firebase Remote Config into strongly-typed client-side configuration objects used by the game.

  #### .3.1. Transformation Id
  PST-003

  #### .3.4. Source
  
  - **Service:** Backend: Firebase Services
  - **Model:** RemoteConfig Payload (Dictionary/JSON)
  - **Fields:**
    
    - difficulty_scaling_factor
    - new_feature_enabled
    
  
  #### .3.5. Target
  
  - **Service:** Client: Domain Layer
  - **Model:** GameConfiguration (C# Object)
  - **Fields:**
    
    - DifficultyScalingFactor
    - IsNewFeatureEnabled
    
  
  #### .3.6. Transformation
  
  - **Type:** direct
  - **Logic:** Parses the fetched remote config data and maps values to the corresponding fields in the C# GameConfiguration object, applying defaults for missing keys.
  - **Configuration:**
    
    
  
  #### .3.7. Frequency
  on-demand

  #### .3.8. Criticality
  medium

  #### .3.9. Dependencies
  
  - REQ-8-006
  - REQ-PCGDS-007
  
  #### .3.10. Validation
  
  - **Pre Transformation:**
    
    
  - **Post Transformation:**
    
    - Type checking and range validation on parsed values
    
  
  #### .3.11. Performance
  
  - **Expected Volume:** Low (once per session)
  - **Latency Requirement:** < 100ms for transformation after fetch
  - **Optimization Strategy:** Cache fetched values locally to avoid re-fetching on every app start.
  
  
- **Implementation Priority:**
  
  - **Component:** PST-001: LocalPlayerStateToCloudSave (and reverse)  
**Priority:** high  
**Dependencies:**
    
    - PlayerProfile data model
    - FirebaseService
    
**Estimated Effort:** Medium  
**Risk Level:** high  
  - **Component:** PST-002: GameResultToLeaderboardEntry  
**Priority:** high  
**Dependencies:**
    
    - ScoringService
    - FirebaseService
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** Server-side Leaderboard Validation  
**Priority:** high  
**Dependencies:**
    
    - PST-002
    - Cloud Function environment setup
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** Local Save Data Migration Logic  
**Priority:** medium  
**Dependencies:**
    
    - PlayerProfile data model
    - PersistenceService
    
**Estimated Effort:** High  
**Risk Level:** high  
  
- **Risk Assessment:**
  
  - **Risk:** Data loss or corruption during cloud save conflict resolution.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Implement a robust 'last-write-wins' strategy based on server timestamps. For significant conflicts (e.g., large progress difference), prompt the user with a clear choice. Heavily test conflict scenarios.  
**Contingency Plan:** Log conflict details to the server. Provide a support channel for users to report data loss for manual investigation.  
  - **Risk:** Failure to correctly migrate local save data across app updates, leading to player progress loss.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Implement comprehensive unit and integration tests for all migration paths. Always back up the old save file before attempting migration. Use an idempotent design for migration scripts.  
**Contingency Plan:** If migration fails, restore the backup and notify the user with instructions to contact support, providing diagnostic info.  
  - **Risk:** Incomplete or incorrect server-side validation allows fraudulent leaderboard scores.  
**Impact:** medium  
**Probability:** high  
**Mitigation:** Implement multi-layered validation in Cloud Functions: data format, plausibility against game rules (BRV-001), and rate limiting. Regularly review top scores for anomalies.  
**Contingency Plan:** Develop an admin tool or script to manually remove fraudulent scores from the leaderboard.  
  
- **Recommendations:**
  
  - **Category:** Data Model  
**Recommendation:** Strictly separate the client-side domain model (C# POCOs) from the backend Firestore schema. Use the Infrastructure Layer's services as the explicit boundary for transformation.  
**Justification:** This maintains the separation of concerns outlined in the Layered Architecture, allowing the client and backend to evolve independently and improves testability of the core domain logic.  
**Priority:** high  
**Implementation Notes:** Create distinct C# classes for DTOs (Data Transfer Objects) used in Firebase communication if they differ significantly from the domain models.  
  - **Category:** Testing  
**Recommendation:** Automate testing for data migration paths (REQ-10-005) and cloud save conflict resolution (REQ-10-010) as part of the CI/CD pipeline.  
**Justification:** These are high-risk, complex transformations where manual testing is error-prone and time-consuming. Automation ensures reliability across releases.  
**Priority:** high  
**Implementation Notes:** Use the Firebase Local Emulator Suite to test Firestore and Cloud Function interactions without incurring costs or affecting live data.  
  - **Category:** Performance  
**Recommendation:** Ensure all transformations involving network I/O (cloud save, leaderboard submissions) are performed asynchronously to prevent blocking the main Unity game thread.  
**Justification:** Main thread blocking leads to a frozen UI and poor user experience, violating performance NFRs.  
**Priority:** high  
**Implementation Notes:** Use C#'s async/await pattern with Unity's async operation handling for all network calls.  
  


---

