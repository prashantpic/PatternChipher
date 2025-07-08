# Specification

# 1. Event Driven Architecture Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Architecture Type:** Hybrid Client-Server (BaaS)
  - **Technology Stack:**
    
    - Unity (C#)
    - Firebase Authentication
    - Cloud Firestore
    - Firebase Cloud Functions (TypeScript/Node.js)
    - Firebase Remote Config
    - Firebase Analytics
    
  - **Bounded Contexts:**
    
    - Core Gameplay & Progression
    - Online Services (Authentication, Leaderboards, Cloud Save)
    - Game Balancing & Configuration
    
  
- **Project Specific Events:**
  
  - **Event Id:** EVT-001  
**Event Name:** LevelCompleted  
**Event Type:** domain  
**Category:** Core Gameplay & Progression  
**Description:** Fired when a player successfully completes a game level. This is the primary trigger for updating player progress, scores, achievements, and leaderboards.  
**Trigger Condition:** The GoalEvaluator component in the client's Domain Layer confirms the puzzle's win condition is met.  
**Source Context:** Client: Domain Layer  
**Target Contexts:**
    
    - Client: Application Layer
    - Backend: Firebase Services
    
**Payload:**
    
    - **Schema:**
      
      - **Player Id:** Guid
      - **Level Definition Id:** Guid
      - **Final Score:** INT
      - **Moves Taken:** INT
      - **Time Seconds:** INT
      - **Stars Awarded:** INT
      
    - **Required Fields:**
      
      - playerId
      - levelDefinitionId
      - finalScore
      - movesTaken
      - starsAwarded
      
    - **Optional Fields:**
      
      - timeSeconds
      
    
**Frequency:** high  
**Business Criticality:** critical  
**Data Source:**
    
    - **Database:** N/A (In-memory game state)
    - **Table:** N/A
    - **Operation:** read
    - **Notes:** Data is sourced from the active game session state
    
**Routing:**
    
    - **Routing Key:** player_progress.update, leaderboard.submit, achievement.check
    - **Exchange:** Client: Application Layer (In-memory event bus or direct method calls)
    - **Queue:** N/A
    
**Consumers:**
    
    - **Service:** ProgressionManager  
**Handler:** OnLevelCompleted  
**Processing Type:** async  
    - **Service:** FirebaseService  
**Handler:** SubmitScoreAndProgress  
**Processing Type:** async  
    
**Dependencies:**
    
    - REQ-CGMI-005
    
**Error Handling:**
    
    - **Retry Strategy:** Local retry queue with exponential backoff for failed network calls.
    - **Dead Letter Queue:** Log failed event to local device log for diagnostics.
    - **Timeout Ms:** 30000
    
  - **Event Id:** EVT-002  
**Event Name:** AchievementUnlocked  
**Event Type:** domain  
**Category:** Core Gameplay & Progression  
**Description:** Fired when a player meets the criteria for a specific achievement.  
**Trigger Condition:** A check against Achievement definitions after a relevant event (e.g., LevelCompleted, total stars updated).  
**Source Context:** Client: Application Layer (ProgressionManager)  
**Target Contexts:**
    
    - Client: Presentation Layer
    - Backend: Firebase Services
    
**Payload:**
    
    - **Schema:**
      
      - **Player Id:** Guid
      - **Achievement Id:** Guid
      - **Unlocked At:** DateTime
      
    - **Required Fields:**
      
      - playerId
      - achievementId
      - unlockedAt
      
    - **Optional Fields:**
      
      
    
**Frequency:** low  
**Business Criticality:** normal  
**Data Source:**
    
    - **Database:** Local DB
    - **Table:** PlayerAchievement
    - **Operation:** update
    
**Routing:**
    
    - **Routing Key:** achievement.unlocked
    - **Exchange:** Client: Application Layer
    - **Queue:** N/A
    
**Consumers:**
    
    - **Service:** UIManager  
**Handler:** DisplayAchievementToast  
**Processing Type:** async  
    - **Service:** FirebaseService  
**Handler:** SyncAchievement  
**Processing Type:** async  
    
**Dependencies:**
    
    - REQ-SRP-010
    
**Error Handling:**
    
    - **Retry Strategy:** Local retry queue for failed network sync.
    - **Dead Letter Queue:** Log sync failure locally.
    - **Timeout Ms:** 15000
    
  - **Event Id:** EVT-003  
**Event Name:** ScoreSubmittedForValidation  
**Event Type:** integration  
**Category:** Online Services  
**Description:** Represents the client's request to post a score, which triggers server-side validation. The true 'event' is the Firestore document write.  
**Trigger Condition:** A LevelCompleted event occurs and the player is authenticated for online services.  
**Source Context:** Client: Infrastructure Layer (FirebaseService)  
**Target Contexts:**
    
    - Backend: Firebase Services (Cloud Functions)
    
**Payload:**
    
    - **Schema:**
      
      - **Leaderboard Name:** VARCHAR
      - **Score Value:** BIGINT
      - **Level Definition Id:** Guid
      - **Client Timestamp:** DateTime
      
    - **Required Fields:**
      
      - leaderboardName
      - scoreValue
      
    - **Optional Fields:**
      
      - levelDefinitionId
      
    
**Frequency:** high  
**Business Criticality:** important  
**Data Source:**
    
    - **Database:** Cloud Firestore
    - **Table:** LeaderboardEntry (or a temporary submission collection)
    - **Operation:** create
    
**Routing:**
    
    - **Routing Key:** firestore.documents.create
    - **Exchange:** Cloud Firestore
    - **Queue:** Cloud Function Trigger (onScoreSubmit)
    
**Consumers:**
    
    - **Service:** LeaderboardValidationFunction  
**Handler:** validateAndPostScore  
**Processing Type:** async  
    
**Dependencies:**
    
    - REQ-9-002
    - REQ-SRP-008
    
**Error Handling:**
    
    - **Retry Strategy:** Managed by Firebase Cloud Functions built-in retry mechanism.
    - **Dead Letter Queue:** Log invalid submission and payload to a 'failed_submissions' collection in Firestore or Cloud Logging.
    - **Timeout Ms:** 60000
    
  - **Event Id:** EVT-004  
**Event Name:** RemoteConfigFetched  
**Event Type:** system  
**Category:** Game Balancing & Configuration  
**Description:** Fired when the client successfully fetches updated configuration parameters from Firebase Remote Config.  
**Trigger Condition:** Successful completion of a fetch and activate call to the Firebase Remote Config SDK.  
**Source Context:** Client: Infrastructure Layer (RemoteConfigService)  
**Target Contexts:**
    
    - Client: Application Layer
    - Client: Domain Layer
    
**Payload:**
    
    - **Schema:**
      
      - **Updated Keys:** Array<string>
      
    - **Required Fields:**
      
      
    - **Optional Fields:**
      
      - updatedKeys
      
    
**Frequency:** low  
**Business Criticality:** normal  
**Data Source:**
    
    - **Database:** Firebase Remote Config
    - **Table:** N/A
    - **Operation:** read
    
**Routing:**
    
    - **Routing Key:** config.updated
    - **Exchange:** Client: Application Layer
    - **Queue:** N/A
    
**Consumers:**
    
    - **Service:** GameManager  
**Handler:** ApplyRemoteConfig  
**Processing Type:** async  
    
**Dependencies:**
    
    - REQ-8-006
    
**Error Handling:**
    
    - **Retry Strategy:** Client-side scheduled retry on next app start.
    - **Dead Letter Queue:** N/A, fallback to cached or default values.
    - **Timeout Ms:** 10000
    
  
- **Event Types And Schema Design:**
  
  - **Essential Event Types:**
    
    - **Event Name:** LevelCompleted  
**Category:** domain  
**Description:** The primary event driving all player progression logic.  
**Priority:** high  
    - **Event Name:** AchievementUnlocked  
**Category:** domain  
**Description:** Notifies the system of a significant player milestone.  
**Priority:** medium  
    - **Event Name:** ScoreSubmittedForValidation  
**Category:** integration  
**Description:** Initiates the server-side process for leaderboard updates, crucial for online features.  
**Priority:** high  
    - **Event Name:** RemoteConfigFetched  
**Category:** system  
**Description:** Enables dynamic game balancing, a key NFR.  
**Priority:** medium  
    
  - **Schema Design:**
    
    - **Format:** JSON
    - **Reasoning:** JSON is the native format for Firebase services (Firestore, Cloud Functions) and is easily serializable from C# POCOs in Unity. It offers the best combination of readability and performance for this technology stack.
    - **Consistency Approach:** Define event schemas as C# classes (POCOs) within the Client's Domain/Infrastructure layer. These classes are the single source of truth and are serialized to JSON for transmission.
    
  - **Schema Evolution:**
    
    - **Backward Compatibility:** True
    - **Forward Compatibility:** False
    - **Strategy:** Additive changes only for minor updates (new optional fields). For breaking changes, a new event version will be introduced via a 'eventVersion' field in the payload.
    
  - **Event Structure:**
    
    - **Standard Fields:**
      
      - eventId (UUID)
      - eventTimestamp (UTC ISO 8601)
      - eventName
      - eventVersion (e.g., 1.0)
      - correlationId (UUID)
      
    - **Metadata Requirements:**
      
      - playerId or firebaseUid for user context.
      - clientAppVersion to correlate events with specific builds.
      
    
  
- **Event Routing And Processing:**
  
  - **Routing Mechanisms:**
    
    - **Type:** In-Client Notification/Delegate  
**Description:** For events originating and consumed within the Unity client (e.g., Domain event handled by Application layer), a simple C# delegate, event, or a lightweight in-memory bus is sufficient.  
**Use Case:** Notifying the UIManager to show a toast when the ProgressionManager unlocks an achievement.  
    - **Type:** Firebase Cloud Function Triggers  
**Description:** The primary mechanism for backend event processing. Functions are triggered by changes in other Firebase services (e.g., a write to a Firestore collection, a new user authentication). This is not a traditional message broker but serves the same purpose in this BaaS architecture.  
**Use Case:** A client writing a score submission document to Firestore, which triggers a Cloud Function to validate the score.  
    
  - **Processing Patterns:**
    
    - **Pattern:** sequential  
**Applicable Scenarios:**
    
    - Validating and posting a single leaderboard score.
    - Updating a player's cloud save data.
    
**Implementation:** A single Firebase Cloud Function handles the entire logic flow for a given trigger.  
    - **Pattern:** parallel (Fan-out)  
**Applicable Scenarios:**
    
    - The 'LevelCompleted' event from the client results in multiple, independent backend API calls: update player progress, submit to leaderboard, check achievements. These can be fired in parallel from the client.
    - A single Firestore write could potentially trigger multiple independent Cloud Functions if subscribed to the same document path.
    
**Implementation:** Client application makes multiple independent async calls to the backend. Alternatively, multiple Cloud Functions can be configured to trigger from the same Firestore document write.  
    
  - **Filtering And Subscription:**
    
    - **Filtering Mechanism:** Cloud Function document path subscription.
    - **Subscription Model:** Producer-unaware. The client writes to a specific Firestore path, and is unaware of which functions, if any, are subscribed to that path.
    - **Routing Keys:**
      
      - /leaderboard_submissions/{submissionId}
      - /player_achievements/{playerId}/{achievementId}
      - /user_profiles/{playerId}/cloud_save
      
    
  - **Handler Isolation:**
    
    - **Required:** True
    - **Approach:** Firebase Cloud Functions
    - **Reasoning:** Firebase Cloud Functions are inherently isolated, serverless execution environments. Each function invocation is independent, providing natural isolation without additional configuration.
    
  - **Delivery Guarantees:**
    
    - **Level:** at-least-once
    - **Justification:** This is the native guarantee for most Firebase Cloud Function triggers (e.g., Firestore, Pub/Sub). It ensures that no event is lost, which is critical for progression and scoring.
    - **Implementation:** All Cloud Function handlers must be designed to be idempotent. For example, when processing a score submission, the function should check if that score has already been processed for that player/level to avoid duplicate entries.
    
  
- **Event Storage And Replay:**
  
  - **Persistence Requirements:**
    
    - **Required:** False
    - **Duration:** N/A
    - **Reasoning:** The architecture is state-driven, not event-sourced. The source of truth is the state stored in Firestore (e.g., PlayerLevelProgress), not a log of events. Events are ephemeral triggers for state transitions, not the state itself.
    
  - **Event Sourcing:**
    
    - **Necessary:** False
    - **Justification:** Event Sourcing is an unnecessary complexity for this system. The requirements can be fully met by storing and managing the current state of entities. State recovery is handled via backups of the state store (Firestore), not by replaying events.
    - **Scope:**
      
      
    
  - **Technology Options:**
    
    - **Technology:** None (No Event Store)  
**Suitability:** high  
**Reasoning:** Aligns with the principle of simplicity and avoids over-engineering. The existing Firestore database serves as the state store, which is sufficient.  
    
  - **Replay Capabilities:**
    
    - **Required:** False
    - **Scenarios:**
      
      
    - **Implementation:** Not required. System recovery relies on restoring state from Firestore backups, not replaying events.
    
  - **Retention Policy:**
    
    - **Strategy:** No Retention of Events
    - **Duration:** Ephemeral
    - **Archiving Approach:** Events are not stored. State is stored in Firestore and backed up. Analytics events are stored separately with their own retention policy (REQ-11-014).
    
  
- **Dead Letter Queue And Error Handling:**
  
  - **Dead Letter Strategy:**
    
    - **Approach:** Use a dedicated Firestore collection as a Dead Letter Queue (DLQ).
    - **Queue Configuration:** A 'failed_event_submissions' collection in Firestore.
    - **Processing Logic:** A Cloud Function that fails after all retries will write the original event payload, error message, and timestamp to the DLQ collection for manual review and reprocessing.
    
  - **Retry Policies:**
    
    - **Error Type:** Transient Network/Service Errors  
**Max Retries:** 5  
**Backoff Strategy:** exponential  
**Delay Configuration:** Utilize the default Firebase Cloud Functions automatic retry configuration for background-triggered functions.  
    - **Error Type:** Data Validation/Logic Errors  
**Max Retries:** 0  
**Backoff Strategy:** fixed  
**Delay Configuration:** No retry. The event is immediately sent to the DLQ as it will never succeed without a code or data fix.  
    
  - **Poison Message Handling:**
    
    - **Detection Mechanism:** Firebase Cloud Function retry exhaustion.
    - **Handling Strategy:** Move the message to the 'failed_event_submissions' DLQ collection in Firestore.
    - **Alerting Required:** True
    
  - **Error Notification:**
    
    - **Channels:**
      
      - Google Cloud Monitoring (Stackdriver) Alerts
      - Email
      
    - **Severity:** critical
    - **Recipients:**
      
      - Backend Development Team
      - On-call Engineer
      
    
  - **Recovery Procedures:**
    
    - **Scenario:** Leaderboard score validation fails due to a temporary bug.  
**Procedure:** 1. Deploy a fix for the Cloud Function. 2. Manually inspect the failed event in the DLQ. 3. Trigger a manual reprocessing of the failed event.  
**Automation Level:** semi-automated  
    
  
- **Event Versioning Strategy:**
  
  - **Schema Evolution Approach:**
    
    - **Strategy:** Additive changes for backward compatibility.
    - **Versioning Scheme:** Semantic versioning (e.g., 1.0, 1.1, 2.0) stored in the event payload.
    - **Migration Strategy:** Consumer (Cloud Function) is responsible for handling multiple event versions during a transition period.
    
  - **Compatibility Requirements:**
    
    - **Backward Compatible:** True
    - **Forward Compatible:** False
    - **Reasoning:** Consumers must be able to process older event versions to support clients that have not yet updated. Consumers are not expected to handle future, unknown event versions.
    
  - **Version Identification:**
    
    - **Mechanism:** Field in payload
    - **Location:** payload
    - **Format:** eventVersion: "1.0"
    
  - **Consumer Upgrade Strategy:**
    
    - **Approach:** Update consumers (Cloud Functions) before producers (client app) are released.
    - **Rollout Strategy:** Deploy updated functions capable of handling both old and new event versions. Once the client update is fully rolled out, legacy handling code can be deprecated.
    - **Rollback Procedure:** Revert the Cloud Function to the previous version that handles the older event schema.
    
  - **Schema Registry:**
    
    - **Required:** False
    - **Technology:** N/A
    - **Governance:** Event schemas are documented as C# classes in the shared client-side codebase and in the project's technical documentation (e.g., Confluence/Wiki).
    
  
- **Event Monitoring And Observability:**
  
  - **Monitoring Capabilities:**
    
    - **Capability:** Cloud Function Performance Monitoring  
**Justification:** Essential to track execution time, invocation count, and error rates of all event handlers.  
**Implementation:** Firebase Console and Google Cloud Monitoring.  
    - **Capability:** DLQ Monitoring  
**Justification:** Essential for identifying and reacting to events that permanently fail processing.  
**Implementation:** Google Cloud Alerting policy on the number of documents in the 'failed_event_submissions' collection.  
    
  - **Tracing And Correlation:**
    
    - **Tracing Required:** True
    - **Correlation Strategy:** Correlation ID
    - **Trace Id Propagation:** The client generates a unique 'correlationId' (UUID) for each user action that initiates a chain of events (e.g., 'LevelCompleted'). This ID is included in all related event payloads and logged by all processing Cloud Functions.
    
  - **Performance Metrics:**
    
    - **Metric:** Function Execution Time (P95)  
**Threshold:** < 500ms  
**Alerting:** True  
    - **Metric:** Function Error Rate  
**Threshold:** > 1% over 5 minutes  
**Alerting:** True  
    - **Metric:** DLQ Size  
**Threshold:** > 0  
**Alerting:** True  
    
  - **Event Flow Visualization:**
    
    - **Required:** False
    - **Tooling:** N/A
    - **Scope:** The event flows are simple enough (Client -> Firestore -> Function) that visualization tools are not essential. Correlation IDs in logs are sufficient for tracing.
    
  - **Alerting Requirements:**
    
    - **Condition:** Spike in leaderboard submission validation errors.  
**Severity:** critical  
**Response Time:** 1 hour  
**Escalation Path:**
    
    - On-call Backend Engineer
    - Lead Backend Engineer
    
    - **Condition:** Cloud Function invocation timeout.  
**Severity:** warning  
**Response Time:** 4 business hours  
**Escalation Path:**
    
    - Backend Development Team
    
    
  
- **Implementation Priority:**
  
  - **Component:** LevelCompleted event publishing and backend processing (Score/Progress Update)  
**Priority:** high  
**Dependencies:**
    
    - PlayerLevelProgress table
    - FirebaseService client component
    
**Estimated Effort:** Medium  
  - **Component:** ScoreSubmittedForValidation Cloud Function trigger and handler  
**Priority:** high  
**Dependencies:**
    
    - Leaderboard tables
    - BR-LEAD-001 documentation
    
**Estimated Effort:** Medium  
  - **Component:** DLQ strategy for Cloud Functions  
**Priority:** medium  
**Dependencies:**
    
    - All Cloud Function handlers
    
**Estimated Effort:** Low  
  - **Component:** RemoteConfigFetched client-side event handling  
**Priority:** medium  
**Dependencies:**
    
    - RemoteConfigService
    - GameManager
    
**Estimated Effort:** Low  
  
- **Risk Assessment:**
  
  - **Risk:** Idempotency is not correctly implemented in Cloud Functions, leading to duplicate data (e.g., multiple leaderboard entries for one level completion).  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Implement strict unit and integration tests for all Cloud Functions to verify idempotent behavior. Use Firestore transactions to atomically check for existence before writing.  
  - **Risk:** Incorrect event versioning leads to processing failures after a client or backend update.  
**Impact:** high  
**Probability:** low  
**Mitigation:** Enforce a strict policy of deploying consumer updates before producer updates. Consumers must gracefully handle older, known event versions.  
  - **Risk:** Cascading failures if a downstream service (e.g., Firestore) is slow or unavailable, causing Cloud Functions to time out and fill the DLQ.  
**Impact:** medium  
**Probability:** low  
**Mitigation:** Leverage Firebase's inherent resilience. Implement circuit breaker patterns in the client for calls to the backend if direct API calls are used. Ensure core gameplay is not blocked by backend unavailability.  
  
- **Recommendations:**
  
  - **Category:** Processing Logic  
**Recommendation:** Strictly enforce idempotency for all Cloud Function handlers that modify state.  
**Justification:** Firebase triggers provide 'at-least-once' delivery, making idempotency essential to prevent data corruption and duplication from retries.  
**Priority:** high  
  - **Category:** Architecture  
**Recommendation:** Do not introduce a dedicated message broker (e.g., Kafka, RabbitMQ). Continue to leverage Firebase's native service triggers.  
**Justification:** A message broker adds significant operational complexity and cost, which is not justified by the current requirements. The existing BaaS architecture provides sufficient event-handling capabilities.  
**Priority:** high  
  - **Category:** Observability  
**Recommendation:** Implement correlation IDs for all user-initiated event chains from day one.  
**Justification:** This is a low-effort, high-reward practice that is invaluable for debugging distributed transactions between the client and multiple backend functions.  
**Priority:** high  
  - **Category:** Schema Management  
**Recommendation:** Maintain a single, version-controlled markdown document in the repository that defines all event schemas.  
**Justification:** Avoids the overhead of a full schema registry while providing a single source of truth for developers on both the client and backend.  
**Priority:** medium  
  


---

