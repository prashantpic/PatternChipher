# Specification

# 1. Files

- **Path:** services/orchestration/package.json  
**Description:** Defines project dependencies, scripts, and metadata for the Node.js package.  
**Template:** Node.js package.json template  
**Dependancy Level:** 0  
**Code File Definition:**
    
    - **Name:** package.json
    - **Type:** Configuration
    - **Relative Path:** ./
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Manages Node.js project dependencies (temporalio, rxjs, joi, typescript, etc.) and defines scripts for building, running, and testing the orchestrator service.
    - **Updated At:** 2024-07-15T10:00:00Z
    - **Template Id:** node-package-json-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/tsconfig.json  
**Description:** TypeScript compiler configuration for the project.  
**Template:** TypeScript tsconfig.json template  
**Dependancy Level:** 0  
**Code File Definition:**
    
    - **Name:** tsconfig.json
    - **Type:** Configuration
    - **Relative Path:** ./
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Configures the TypeScript compiler options (e.g., target ECMAScript version, module system, strict type checking, paths for module resolution) for the orchestrator service.
    - **Updated At:** 2024-07-15T10:01:00Z
    - **Template Id:** typescript-tsconfig-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/.env.example  
**Description:** Example environment variables file.  
**Template:** dotenv example template  
**Dependancy Level:** 0  
**Code File Definition:**
    
    - **Name:** .env.example
    - **Type:** Configuration
    - **Relative Path:** ./
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Provides a template for required environment variables such as Temporal connection details, service endpoints, and API keys needed for the orchestrator to function.
    - **Updated At:** 2024-07-15T10:02:00Z
    - **Template Id:** dotenv-example-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/index.ts  
**Description:** Main entry point for the service orchestrator application. Initializes and starts Temporal workers.  
**Template:** TypeScript Application Entry Point  
**Dependancy Level:** 1  
**Code File Definition:**
    
    - **Name:** index
    - **Type:** ApplicationEntry
    - **Relative Path:** src/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Initializes application-wide configurations (logging, environment variables), sets up and starts all Temporal workers responsible for executing workflows and activities.
    - **Logic Description:** Imports and invokes worker initialization modules (e.g., for IAP and score processing). Handles graceful shutdown and process signals.
    - **Updated At:** 2024-07-15T10:05:00Z
    - **Namespace:** GlyphPuzzle.Orchestration
    - **Template Id:** ts-app-entry-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/temporal_client.ts  
**Description:** Configures and exports a shared Temporal client instance.  
**Template:** TypeScript Temporal Client Configuration  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** temporalClient
    - **Type:** Configuration
    - **Relative Path:** src/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Provides a singleton, configured Temporal Client instance for interacting with the Temporal cluster (e.g., starting new workflow executions, sending signals, querying workflows).
    - **Logic Description:** Initializes a Temporal Connection and Client object based on environment configuration (e.g., Temporal server address, namespace). Exports the configured client.
    - **Updated At:** 2024-07-15T10:06:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Temporal
    - **Template Id:** ts-temporal-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/config/index.ts  
**Description:** Barrel file for exporting configuration modules.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** config.index
    - **Type:** Barrel
    - **Relative Path:** src/config/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Aggregates and exports all configuration modules (environment, logger) for simplified importing across the application.
    - **Updated At:** 2024-07-15T10:07:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Config
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/config/environment.ts  
**Description:** Manages and validates environment variables.  
**Template:** TypeScript Environment Configuration  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** environment
    - **Type:** Configuration
    - **Relative Path:** src/config/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Loads, validates (using Joi or similar), and exports environment variables in a type-safe manner, ensuring all required configurations are present and correctly formatted.
    - **Logic Description:** Defines a schema for expected environment variables (e.g., TEMPORAL_ADDRESS, API_KEYS), loads them from process.env, performs validation, and provides a typed configuration object.
    - **Updated At:** 2024-07-15T10:08:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Config
    - **Template Id:** ts-env-config-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/config/logger.ts  
**Description:** Configures the application logger.  
**Template:** TypeScript Logger Configuration  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** logger
    - **Type:** Configuration
    - **Relative Path:** src/config/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Sets up a standardized and configurable logger (e.g., Winston) for consistent application-wide logging.
    - **Logic Description:** Configures log levels (debug, info, error), formats (JSON, pretty-print), and transports (console, file, or external logging services) based on environment settings.
    - **Updated At:** 2024-07-15T10:09:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Config
    - **Template Id:** ts-logger-config-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/workflows/index.ts  
**Description:** Barrel file for exporting all workflow definitions.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** workflows.index
    - **Type:** Barrel
    - **Relative Path:** src/workflows/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Aggregates and exports all Temporal workflow interface definitions and implementations, making them available for registration with Temporal workers.
    - **Updated At:** 2024-07-15T10:10:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Workflows
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/workflows/iap_processing_workflow.ts  
**Description:** Temporal workflow definition for orchestrating In-App Purchase processing.  
**Template:** TypeScript Temporal Workflow  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** iapProcessingWorkflow
    - **Type:** WorkflowDefinition
    - **Relative Path:** src/workflows/
    - **Component Id:** iap-orchestrator
    - **Pattern Ids:**
      
      - Saga
      
    - **Implemented Features:**
      
      - IAP Validation Flow
      - Distributed Transaction Management for IAP
      - Compensation Logic for IAP Failures
      
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Orchestrates the end-to-end In-App Purchase validation and fulfillment process as a Saga. Ensures atomicity and consistency across multiple service calls.
    - **Logic Description:** Defines the sequence of activities for IAP: 1. VerifyIAPReceipt, 2. GrantEntitlement, 3. UpdatePlayerInventory, 4. TrackIAPEvent. Implements compensation activities (e.g., CompensateGrantEntitlement) to roll back changes in case of failures at any step, adhering to the Saga pattern. Uses Temporal's Saga primitives if applicable.
    - **Updated At:** 2024-07-15T10:11:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Workflows.IAP
    - **Template Id:** ts-temporal-workflow-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/workflows/score_submission_workflow.ts  
**Description:** Temporal workflow definition for orchestrating score submission processing.  
**Template:** TypeScript Temporal Workflow  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** scoreSubmissionWorkflow
    - **Type:** WorkflowDefinition
    - **Relative Path:** src/workflows/
    - **Component Id:** score-processing-saga
    - **Pattern Ids:**
      
      - Saga
      
    - **Implemented Features:**
      
      - Score Submission Process
      - Cheat Detection Orchestration
      - Leaderboard Update Orchestration
      - Cloud Save Synchronization Trigger
      - Secure Score Logging Orchestration
      - Compensation Logic for Score Processing
      
    - **Requirement Ids:**
      
      - REQ-8-014
      - REQ-SCF-014
      
    - **Purpose:** Orchestrates the score submission process as a Saga, including validation, cheat detection, leaderboard updates, cloud save synchronization, and secure audit logging.
    - **Logic Description:** Defines the activity sequence: 1. ValidateScoreIntegrity, 2. RunCheatDetection, 3. UpdateLeaderboard (if valid and not cheating), 4. SynchronizeCloudSave, 5. LogScoreSubmissionAudit. Implements compensation logic for steps like UpdateLeaderboard. Uses Temporal's Saga primitives if applicable.
    - **Updated At:** 2024-07-15T10:12:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Workflows.Score
    - **Template Id:** ts-temporal-workflow-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/index.ts  
**Description:** Barrel file for exporting all activity modules.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** activities.index
    - **Type:** Barrel
    - **Relative Path:** src/activities/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Aggregates and exports all Temporal activity interface definitions and implementations, making them available for registration with Temporal workers.
    - **Updated At:** 2024-07-15T10:13:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/iap/index.ts  
**Description:** Barrel file for IAP related activities.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** iap.activities.index
    - **Type:** Barrel
    - **Relative Path:** src/activities/iap/
    - **Component Id:** iap-orchestrator
    - **Purpose:** Exports all activity functions and interfaces related to the In-App Purchase processing workflow.
    - **Updated At:** 2024-07-15T10:14:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.IAP
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/iap/verify_iap_receipt_activity.ts  
**Description:** Temporal activity to verify an IAP receipt with the platform.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** verifyIapReceiptActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/iap/
    - **Component Id:** iap-orchestrator
    - **Implemented Features:**
      
      - IAP Receipt Verification
      
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Performs server-to-server validation of an In-App Purchase receipt by communicating with the appropriate platform validation service (Apple/Google via REPO-SECURITY).
    - **Logic Description:** Accepts IAP receipt data as input. Uses the `IAPValidationPlatformClient` to send the receipt to the REPO-SECURITY service for validation. Returns the validation result (success/failure, transaction details). Handles retries and platform-specific errors as configured by Temporal.
    - **Updated At:** 2024-07-15T10:15:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.IAP
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/iap/grant_entitlement_activity.ts  
**Description:** Temporal activity to grant entitlement/item to the player.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** grantEntitlementActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/iap/
    - **Component Id:** iap-orchestrator
    - **Implemented Features:**
      
      - Player Entitlement Granting
      
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Grants the purchased item or virtual currency to the player after successful IAP receipt validation.
    - **Logic Description:** Accepts player ID and item/currency details. Uses the `PlayerInventoryClient` to communicate with REPO-BACKEND-API to update the player's inventory or balance in REPO-DOMAIN-DATA. This is a critical step in the IAP saga.
    - **Updated At:** 2024-07-15T10:16:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.IAP
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/iap/update_player_inventory_activity.ts  
**Description:** Temporal activity to confirm or finalize player's inventory update, possibly redundant if grant_entitlement is comprehensive.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** updatePlayerInventoryActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/iap/
    - **Component Id:** iap-orchestrator
    - **Implemented Features:**
      
      - Inventory Finalization
      
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Ensures player's inventory reflects the granted entitlement. This might involve confirming the update or handling specific inventory logic not covered by grantEntitlement. If grantEntitlement is fully comprehensive, this might be merged or removed.
    - **Logic Description:** Interacts with `PlayerInventoryClient` to finalize or verify inventory changes. May be used for more complex inventory scenarios or as a separate auditable step.
    - **Updated At:** 2024-07-15T10:17:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.IAP
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/iap/track_iap_event_activity.ts  
**Description:** Temporal activity to track IAP related analytics events.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** trackIapEventActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/iap/
    - **Component Id:** iap-orchestrator
    - **Implemented Features:**
      
      - IAP Analytics Tracking
      
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Sends IAP success or failure events to the analytics service for tracking and analysis.
    - **Logic Description:** Accepts IAP transaction details and event type (e.g., 'iapSuccess', 'iapFailure'). Uses the `AnalyticsClient` to send this data to the analytics backend (REPO-ANALYTICS-MON or via REPO-BACKEND-API).
    - **Updated At:** 2024-07-15T10:18:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.IAP
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/iap/compensate_grant_entitlement_activity.ts  
**Description:** Temporal activity to compensate a failed grant entitlement.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** compensateGrantEntitlementActivity
    - **Type:** CompensationActivity
    - **Relative Path:** src/activities/iap/
    - **Component Id:** iap-orchestrator
    - **Implemented Features:**
      
      - IAP Entitlement Compensation
      
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Rolls back the granting of an item or virtual currency if a subsequent step in the IAP processing saga fails, ensuring data consistency.
    - **Logic Description:** Accepts player ID and item/currency details of the original grant. Uses the `PlayerInventoryClient` to revert the inventory update in REPO-DOMAIN-DATA (via REPO-BACKEND-API). This is a key compensation step in the IAP Saga.
    - **Updated At:** 2024-07-15T10:19:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.IAP
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/index.ts  
**Description:** Barrel file for score processing related activities.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** score.activities.index
    - **Type:** Barrel
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Purpose:** Exports all activity functions and interfaces related to the score submission processing workflow.
    - **Updated At:** 2024-07-15T10:20:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/validate_score_integrity_activity.ts  
**Description:** Temporal activity to validate the integrity of a submitted score.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** validateScoreIntegrityActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Implemented Features:**
      
      - Score Data Validation
      
    - **Requirement Ids:**
      
      - REQ-8-014
      
    - **Purpose:** Performs initial validation on the submitted score data to check for structural correctness, valid ranges, and consistency before more intensive processing.
    - **Logic Description:** Accepts score submission data. Applies validation rules, potentially using Joi schemas defined in `src/utils/validation_schemas.ts`. Returns validation status (valid/invalid) and any error details.
    - **Updated At:** 2024-07-15T10:21:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/run_cheat_detection_activity.ts  
**Description:** Temporal activity to run cheat detection algorithms on a score.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** runCheatDetectionActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Implemented Features:**
      
      - Cheat Detection Integration
      
    - **Requirement Ids:**
      
      - REQ-8-014
      
    - **Purpose:** Invokes the cheat detection service to analyze the submitted score and associated gameplay data for signs of cheating.
    - **Logic Description:** Accepts score data and relevant player context. Uses the `CheatDetectionClient` to send this information to the cheat detection module in REPO-BACKEND-API. Returns the cheat analysis result (e.g., clean, suspicious, cheated).
    - **Updated At:** 2024-07-15T10:22:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/update_leaderboard_activity.ts  
**Description:** Temporal activity to update the leaderboard with a new score.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** updateLeaderboardActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Implemented Features:**
      
      - Leaderboard Update
      
    - **Requirement Ids:**
      
      - REQ-8-014
      
    - **Purpose:** Submits a validated and cheat-checked score to the leaderboard service for ranking.
    - **Logic Description:** Accepts player ID, score details, level ID, etc. Uses the `LeaderboardClient` to communicate with REPO-BACKEND-API to update the leaderboard in REPO-DOMAIN-DATA. This activity is a step in the score submission saga.
    - **Updated At:** 2024-07-15T10:23:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/compensate_update_leaderboard_activity.ts  
**Description:** Temporal activity to compensate a leaderboard update.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** compensateUpdateLeaderboardActivity
    - **Type:** CompensationActivity
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Implemented Features:**
      
      - Leaderboard Update Compensation
      
    - **Requirement Ids:**
      
      - REQ-8-014
      
    - **Purpose:** Reverts or flags a score on the leaderboard if a subsequent step in the score processing saga fails.
    - **Logic Description:** Accepts details of the original leaderboard update (e.g., score ID, player ID). Uses the `LeaderboardClient` to request removal or invalidation of the score. This is a compensation step in the Score Submission Saga.
    - **Updated At:** 2024-07-15T10:23:30Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/synchronize_cloud_save_activity.ts  
**Description:** Temporal activity to synchronize player progress with cloud save.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** synchronizeCloudSaveActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Implemented Features:**
      
      - Cloud Save Synchronization Trigger
      
    - **Purpose:** Triggers the cloud save mechanism to synchronize the player's latest progress after a significant event like a score update.
    - **Logic Description:** Accepts player ID and potentially a summary of new progress. Uses the `CloudSaveClient` to signal REPO-BACKEND-API to initiate the cloud save process for the player.
    - **Updated At:** 2024-07-15T10:24:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/score/log_score_submission_audit_activity.ts  
**Description:** Temporal activity to log score submission details for auditing.  
**Template:** TypeScript Temporal Activity  
**Dependancy Level:** 3  
**Code File Definition:**
    
    - **Name:** logScoreSubmissionAuditActivity
    - **Type:** ActivityImplementation
    - **Relative Path:** src/activities/score/
    - **Component Id:** score-processing-saga
    - **Implemented Features:**
      
      - Secure Score Submission Logging
      
    - **Requirement Ids:**
      
      - REQ-SCF-014
      
    - **Purpose:** Logs comprehensive details about a score submission (including validation and cheat detection results) for auditing, support, and cheat analysis purposes.
    - **Logic Description:** Accepts Player ID, submitted score, Level ID/Zone ID, server-generated timestamp, validation results, cheat detection results, etc. Uses the `AuditLoggerClient` to persist this log securely via REPO-BACKEND-API to REPO-DOMAIN-DATA.
    - **Updated At:** 2024-07-15T10:25:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Score
    - **Template Id:** ts-temporal-activity-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/activities/shared/activity_context.interface.ts  
**Description:** Defines shared interfaces for activity contexts or common parameters.  
**Template:** TypeScript Interface Definition  
**Dependancy Level:** 4  
**Code File Definition:**
    
    - **Name:** ActivityContext
    - **Type:** Interface
    - **Relative Path:** src/activities/shared/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Provides common TypeScript interface definitions for activity inputs, outputs, or shared context data (e.g., player identifiers, session information) to ensure consistency.
    - **Updated At:** 2024-07-15T10:26:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Activities.Shared
    - **Template Id:** ts-interface-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/index.ts  
**Description:** Barrel file for exporting all external service clients.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** services.index
    - **Type:** Barrel
    - **Relative Path:** src/services/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Aggregates and exports all client modules used by activities to communicate with other microservices or external platforms.
    - **Updated At:** 2024-07-15T10:27:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/iap_validation_platform_client.ts  
**Description:** Client to communicate with the IAP validation platform service (REPO-SECURITY).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** iapValidationPlatformClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** iap-orchestrator
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Provides methods to interact with the REPO-SECURITY service for server-to-server IAP receipt validation with Apple/Google platforms.
    - **Logic Description:** Implements HTTP client logic (e.g., using axios or fetch) to call the specific IAP validation endpoints exposed by REPO-SECURITY. Handles request/response DTOs, authentication, and error mapping.
    - **Updated At:** 2024-07-15T10:28:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/player_inventory_client.ts  
**Description:** Client to communicate with the player inventory service (via REPO-BACKEND-API to REPO-DOMAIN-DATA).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** playerInventoryClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** iap-orchestrator
    - **Requirement Ids:**
      
      - REQ-8-013
      
    - **Purpose:** Provides methods to update a player's inventory (items, virtual currency) by calling the REPO-BACKEND-API.
    - **Logic Description:** Implements HTTP client logic to call player inventory management endpoints on REPO-BACKEND-API. Manages request/response DTOs for inventory updates.
    - **Updated At:** 2024-07-15T10:29:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/leaderboard_client.ts  
**Description:** Client to communicate with the leaderboard service (via REPO-BACKEND-API to REPO-DOMAIN-DATA).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** leaderboardClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** score-processing-saga
    - **Requirement Ids:**
      
      - REQ-8-014
      
    - **Purpose:** Provides methods to update leaderboards by calling the REPO-BACKEND-API.
    - **Logic Description:** Implements HTTP client logic to call leaderboard management endpoints (e.g., submit score) on REPO-BACKEND-API. Manages request/response DTOs for leaderboard interactions.
    - **Updated At:** 2024-07-15T10:30:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/analytics_client.ts  
**Description:** Client to send events to the analytics service (REPO-ANALYTICS-MON or via REPO-BACKEND-API).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** analyticsClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** iap-orchestrator
    - **Purpose:** Provides methods to send analytics events to the designated analytics ingestion endpoint.
    - **Logic Description:** Implements HTTP client logic to call analytics event ingestion endpoints (either directly to REPO-ANALYTICS-MON or via REPO-BACKEND-API). Manages request/response DTOs for analytics events.
    - **Updated At:** 2024-07-15T10:31:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/cheat_detection_client.ts  
**Description:** Client to communicate with the cheat detection service (likely part of REPO-BACKEND-API).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** cheatDetectionClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** score-processing-saga
    - **Requirement Ids:**
      
      - REQ-8-014
      
    - **Purpose:** Provides methods to submit scores or gameplay data for cheat analysis by calling the REPO-BACKEND-API.
    - **Logic Description:** Implements HTTP client logic to call cheat detection service endpoints on REPO-BACKEND-API. Manages DTOs for submitting data and receiving analysis results.
    - **Updated At:** 2024-07-15T10:32:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/cloud_save_client.ts  
**Description:** Client to communicate with the cloud save service (likely part of REPO-BACKEND-API).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** cloudSaveClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** score-processing-saga
    - **Purpose:** Provides methods to trigger cloud save operations for player data by calling the REPO-BACKEND-API.
    - **Logic Description:** Implements HTTP client logic to call cloud save management endpoints on REPO-BACKEND-API, signaling it to synchronize player data.
    - **Updated At:** 2024-07-15T10:33:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/services/audit_logger_client.ts  
**Description:** Client for logging audit events (e.g., score submissions to REPO-BACKEND-API).  
**Template:** TypeScript HTTP Client  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** auditLoggerClient
    - **Type:** ServiceClient
    - **Relative Path:** src/services/
    - **Component Id:** score-processing-saga
    - **Requirement Ids:**
      
      - REQ-SCF-014
      
    - **Purpose:** Provides methods to send detailed audit logs to a central logging or data storage service via REPO-BACKEND-API.
    - **Logic Description:** Implements HTTP client logic to call audit log ingestion endpoints on REPO-BACKEND-API. Ensures secure and reliable logging of critical events like score submissions.
    - **Updated At:** 2024-07-15T10:34:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Services
    - **Template Id:** ts-http-client-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/error_handling/index.ts  
**Description:** Barrel file for error handling modules.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** errorHandling.index
    - **Type:** Barrel
    - **Relative Path:** src/error_handling/
    - **Component Id:** recovery-manager
    - **Purpose:** Aggregates and exports error handling utilities, custom error types, and recovery management logic.
    - **Updated At:** 2024-07-15T10:35:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.ErrorHandling
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/error_handling/recovery_manager.ts  
**Description:** Defines unified error recovery strategies and policies for workflows.  
**Template:** TypeScript Module  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** recoveryManager
    - **Type:** Utility
    - **Relative Path:** src/error_handling/
    - **Component Id:** recovery-manager
    - **Purpose:** Provides mechanisms and configurations for implementing retry policies, error classification (retryable vs. non-retryable), and defining compensation strategies within Temporal workflows and activities.
    - **Logic Description:** Contains helper functions for configuring Temporal retry options (e.g., exponential backoff, maximum attempts), classifying specific errors from service clients, and potentially utilities for standardizing how compensation logic is invoked or managed in workflows. This supports the robust error recovery aspect of Sagas.
    - **Updated At:** 2024-07-15T10:36:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.ErrorHandling
    - **Template Id:** ts-module-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/error_handling/custom_errors.ts  
**Description:** Defines custom error types specific to the orchestration domain.  
**Template:** TypeScript Custom Error Definitions  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** customErrors
    - **Type:** ErrorDefinitions
    - **Relative Path:** src/error_handling/
    - **Component Id:** recovery-manager
    - **Purpose:** Provides specific, typed error classes for different failure scenarios encountered during workflow orchestration, enabling more precise error handling and reporting.
    - **Logic Description:** Extends the base `Error` class or Temporal's error classes (e.g., `ApplicationFailure`) to create meaningful error types such as `IAPValidationFailedError`, `ScoreProcessingError`, `CheatDetectedError`, `CompensationFailedError`. These errors can carry additional contextual information.
    - **Updated At:** 2024-07-15T10:37:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.ErrorHandling
    - **Template Id:** ts-custom-errors-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/utils/index.ts  
**Description:** Barrel file for utility modules.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** utils.index
    - **Type:** Barrel
    - **Relative Path:** src/utils/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Aggregates and exports common utility functions and modules, such as validation schemas.
    - **Updated At:** 2024-07-15T10:38:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Utils
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/utils/validation_schemas.ts  
**Description:** Contains Joi validation schemas for workflow and activity inputs.  
**Template:** TypeScript Joi Schemas  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** validationSchemas
    - **Type:** ValidationSchemas
    - **Relative Path:** src/utils/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Defines Joi schemas (or similar validation library schemas) to validate the structure, types, and constraints of inputs to workflows and activities, ensuring data integrity before processing.
    - **Logic Description:** Exports various Joi schema objects for different data structures, such as IAP receipt payloads, score submission data, and other DTOs used in orchestration processes. These can be used in activities or workflow input validation.
    - **Updated At:** 2024-07-15T10:39:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Utils
    - **Template Id:** ts-joi-schemas-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/interfaces/index.ts  
**Description:** Barrel file for shared TypeScript interfaces.  
**Template:** TypeScript Barrel File  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** interfaces.index
    - **Type:** Barrel
    - **Relative Path:** src/interfaces/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Aggregates and exports all shared TypeScript interface definitions (DTOs, value objects) for the orchestrator module.
    - **Updated At:** 2024-07-15T10:40:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Interfaces
    - **Template Id:** ts-barrel-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/interfaces/iap.interfaces.ts  
**Description:** TypeScript interfaces related to IAP processing.  
**Template:** TypeScript Interface Definition  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** iap.interfaces
    - **Type:** Interface
    - **Relative Path:** src/interfaces/
    - **Component Id:** iap-orchestrator
    - **Purpose:** Defines data structures, DTOs, and type definitions specifically for In-App Purchase workflow inputs, activity parameters, service client requests/responses, and outputs.
    - **Updated At:** 2024-07-15T10:41:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Interfaces
    - **Template Id:** ts-interface-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/interfaces/score.interfaces.ts  
**Description:** TypeScript interfaces related to score processing.  
**Template:** TypeScript Interface Definition  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** score.interfaces
    - **Type:** Interface
    - **Relative Path:** src/interfaces/
    - **Component Id:** score-processing-saga
    - **Purpose:** Defines data structures, DTOs, and type definitions specifically for score submission workflow inputs, activity parameters, service client requests/responses, and outputs.
    - **Updated At:** 2024-07-15T10:42:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Interfaces
    - **Template Id:** ts-interface-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/interfaces/common.interfaces.ts  
**Description:** Common TypeScript interfaces used across the orchestrator.  
**Template:** TypeScript Interface Definition  
**Dependancy Level:** 2  
**Code File Definition:**
    
    - **Name:** common.interfaces
    - **Type:** Interface
    - **Relative Path:** src/interfaces/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Defines generic data structures, types, or enums (e.g., PlayerID, ItemID, Status codes, standard error responses) that are used by multiple workflows, activities, or services within the orchestrator.
    - **Updated At:** 2024-07-15T10:43:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Interfaces
    - **Template Id:** ts-interface-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/workers/index.ts  
**Description:** Initializes and starts all Temporal workers for the orchestrator.  
**Template:** TypeScript Temporal Worker Initializer  
**Dependancy Level:** 1  
**Code File Definition:**
    
    - **Name:** workers.index
    - **Type:** WorkerInitializer
    - **Relative Path:** src/workers/
    - **Component Id:** REPO-SERVICE-ORCHESTRATOR
    - **Purpose:** Serves as the central point for creating, configuring, and launching all Temporal workers required by the orchestrator service.
    - **Logic Description:** Imports individual worker setup functions (e.g., for IAP processing, score submission). Iterates through them, initializing and starting each worker to listen on its designated task queue(s). Manages shared worker configurations if any.
    - **Updated At:** 2024-07-15T10:44:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Workers
    - **Template Id:** ts-temporal-worker-main-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/workers/iap_processing_worker.ts  
**Description:** Sets up and runs the Temporal worker for IAP processing workflows and activities.  
**Template:** TypeScript Temporal Worker  
**Dependancy Level:** 1  
**Code File Definition:**
    
    - **Name:** iapProcessingWorker
    - **Type:** Worker
    - **Relative Path:** src/workers/
    - **Component Id:** iap-orchestrator
    - **Purpose:** Hosts and executes In-App Purchase processing workflows and their associated activities by listening to a specific Temporal task queue.
    - **Logic Description:** Creates a Temporal Worker instance connected to the Temporal cluster. Registers all IAP-related workflow implementations (e.g., `iapProcessingWorkflow`) and activity implementations (e.g., `verifyIapReceiptActivity`, `grantEntitlementActivity`) with this worker. Starts the worker to begin polling its task queue.
    - **Updated At:** 2024-07-15T10:45:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Workers
    - **Template Id:** ts-temporal-worker-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    
- **Path:** services/orchestration/src/workers/score_submission_worker.ts  
**Description:** Sets up and runs the Temporal worker for score submission workflows and activities.  
**Template:** TypeScript Temporal Worker  
**Dependancy Level:** 1  
**Code File Definition:**
    
    - **Name:** scoreSubmissionWorker
    - **Type:** Worker
    - **Relative Path:** src/workers/
    - **Component Id:** score-processing-saga
    - **Purpose:** Hosts and executes score submission processing workflows and their associated activities by listening to a specific Temporal task queue.
    - **Logic Description:** Creates a Temporal Worker instance. Registers all score submission-related workflow implementations (e.g., `scoreSubmissionWorkflow`) and activity implementations (e.g., `runCheatDetectionActivity`, `updateLeaderboardActivity`) with this worker. Starts the worker to begin polling its task queue.
    - **Updated At:** 2024-07-15T10:46:00Z
    - **Namespace:** GlyphPuzzle.Orchestration.Workers
    - **Template Id:** ts-temporal-worker-v1
    
**Metadata:**
    
    - **Domain:** GlyphPuzzle.Orchestration
    


---

# 2. Configuration

- **Feature Flags:**
  
  - enableDetailedAuditLoggingForScore
  - enableAdvancedCheatDetectionHeuristicsToggle
  - enableIAPSagaVerboseLogging
  
- **Security Configs:**
  
  - TEMPORAL_CLUSTER_ADDRESS
  - TEMPORAL_NAMESPACE
  - TEMPORAL_CLIENT_CERT_PATH
  - TEMPORAL_CLIENT_KEY_PATH
  - IAP_VALIDATION_SERVICE_API_KEY
  - BACKEND_API_AUTH_TOKEN
  
- **Service Endpoints:**
  
  - IAP_VALIDATION_SERVICE_ENDPOINT (REPO-SECURITY)
  - BACKEND_API_ENDPOINT (REPO-BACKEND-API for inventory, leaderboard, cheat detection, cloud save, audit)
  - ANALYTICS_SERVICE_ENDPOINT
  
- **Retry Policies:**
  
  - DefaultActivityRetryPolicy
  - CriticalActivityRetryPolicy
  - IAPValidationActivityRetryPolicy
  
- **Queue Names:**
  
  - IAP_PROCESSING_TASK_QUEUE
  - SCORE_SUBMISSION_TASK_QUEUE
  


---

