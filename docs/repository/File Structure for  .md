# Specification

# 1. Files

- **Path:** EventDefinitions/SessionStartEvent.json  
**Description:** Defines the data contract for an event logged at the beginning of a new game session. Captures core device and application context. This event helps in understanding user base technology, session frequency, and adoption of new app versions.  
**Template:** JSON Schema  
**Dependency Level:** 0  
**Name:** SessionStartEvent  
**Type:** EventDefinition  
**Relative Path:** EventDefinitions/SessionStartEvent  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - EventDriven
    - PubSub
    
**Members:**
    
    - **Name:** eventName  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** eventTimestamp  
**Type:** datetime  
**Attributes:** public|readonly  
    - **Name:** playerId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** sessionId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** appVersion  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** platform  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** deviceModel  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** osVersion  
**Type:** string  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Session Tracking
    - Device Information Collection
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-AT-003
    
**Purpose:** To log the start of a user's game session with essential, anonymized device context for segmentation and technical analysis.  
**Logic Description:** This is a data contract definition. The client application is responsible for populating and sending this event to the Firebase Analytics endpoint upon app launch or resume after a significant period of inactivity. The eventName property should be a constant, e.g., 'session_start'.  
**Documentation:**
    
    - **Summary:** Describes the schema for the 'session_start' analytics event. This event provides foundational context for all subsequent events within the same session. All fields are required to ensure complete session context.
    
**Namespace:** PatternCipher.Analytics.Events  
**Metadata:**
    
    - **Category:** Analytics
    
- **Path:** EventDefinitions/LevelStartEvent.json  
**Description:** Defines the data contract for an event logged when a player starts a new level. This is crucial for creating funnels to analyze player progression and identify where players begin their engagement.  
**Template:** JSON Schema  
**Dependency Level:** 0  
**Name:** LevelStartEvent  
**Type:** EventDefinition  
**Relative Path:** EventDefinitions/LevelStartEvent  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - EventDriven
    - PubSub
    
**Members:**
    
    - **Name:** eventName  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** eventTimestamp  
**Type:** datetime  
**Attributes:** public|readonly  
    - **Name:** playerId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** sessionId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** levelId  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** levelType  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** difficulty  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** gridSize  
**Type:** string  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Level Progression Tracking
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-B-006
    
**Purpose:** To log the initiation of a level attempt, capturing the level's identity and core difficulty parameters.  
**Logic Description:** This is a data contract definition. The client application sends this event when a player transitions to the active game screen for a specific level. The eventName property should be a constant, e.g., 'level_start'.  
**Documentation:**
    
    - **Summary:** Describes the schema for the 'level_start' analytics event. This data is used to measure level engagement and serves as the starting point for calculating level completion times and rates.
    
**Namespace:** PatternCipher.Analytics.Events  
**Metadata:**
    
    - **Category:** Analytics
    
- **Path:** EventDefinitions/LevelOutcomeEvent.json  
**Description:** Defines the data contract for an event logged when a level concludes, either by completion, failure, or being quit by the player. This is a primary event for balancing and understanding player success and frustration points.  
**Template:** JSON Schema  
**Dependency Level:** 0  
**Name:** LevelOutcomeEvent  
**Type:** EventDefinition  
**Relative Path:** EventDefinitions/LevelOutcomeEvent  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - EventDriven
    - PubSub
    
**Members:**
    
    - **Name:** eventName  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** eventTimestamp  
**Type:** datetime  
**Attributes:** public|readonly  
    - **Name:** playerId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** sessionId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** levelId  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** outcome  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** timeTakenSeconds  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** movesTaken  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** score  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** starsAwarded  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** failureReason  
**Type:** string  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Level Performance Tracking
    - Player Progression Analysis
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-B-006
    
**Purpose:** To capture the result of a level attempt, including key performance metrics used for difficulty tuning and player experience analysis.  
**Logic Description:** This is a data contract definition. The client sends this event when a level ends. The 'outcome' field should be an enum-like string: 'Complete', 'Fail', 'Quit'. Fields like 'score' and 'starsAwarded' are only relevant for 'Complete'. 'failureReason' is only relevant for 'Fail'. eventName should be 'level_outcome'.  
**Documentation:**
    
    - **Summary:** Describes the schema for the 'level_outcome' analytics event. This single event captures all possible level end-states, simplifying analysis by keeping related metrics together.
    
**Namespace:** PatternCipher.Analytics.Events  
**Metadata:**
    
    - **Category:** Analytics
    
- **Path:** EventDefinitions/PlayerActionUsedEvent.json  
**Description:** Defines the data contract for an event logged when a player uses a specific in-game action like 'Hint' or 'Undo'. This helps measure the reliance on assistance mechanics, which is a key indicator of level difficulty and player friction.  
**Template:** JSON Schema  
**Dependency Level:** 0  
**Name:** PlayerActionUsedEvent  
**Type:** EventDefinition  
**Relative Path:** EventDefinitions/PlayerActionUsedEvent  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - EventDriven
    - PubSub
    
**Members:**
    
    - **Name:** eventName  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** eventTimestamp  
**Type:** datetime  
**Attributes:** public|readonly  
    - **Name:** playerId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** sessionId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** levelId  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** actionType  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** movesAtAction  
**Type:** int  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Feature Usage Tracking
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-B-006
    
**Purpose:** To log the usage of significant, discrete player actions within a level, such as using a hint or an undo move.  
**Logic Description:** This is a data contract definition. The client sends this event whenever the player successfully uses a hint or undo. The 'actionType' field should be an enum-like string: 'Hint', 'Undo'. The eventName should be 'player_action_used'.  
**Documentation:**
    
    - **Summary:** Describes the schema for the 'player_action_used' analytics event. This generic event structure can be extended to track other special actions in the future.
    
**Namespace:** PatternCipher.Analytics.Events  
**Metadata:**
    
    - **Category:** Analytics
    
- **Path:** EventDefinitions/ClientErrorEvent.json  
**Description:** Defines the data contract for an event logged when the client application encounters an unhandled exception or a significant, non-crashing error. This is vital for proactive bug fixing and improving game stability.  
**Template:** JSON Schema  
**Dependency Level:** 0  
**Name:** ClientErrorEvent  
**Type:** EventDefinition  
**Relative Path:** EventDefinitions/ClientErrorEvent  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - EventDriven
    - PubSub
    
**Members:**
    
    - **Name:** eventName  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** eventTimestamp  
**Type:** datetime  
**Attributes:** public|readonly  
    - **Name:** playerId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** sessionId  
**Type:** guid  
**Attributes:** public|readonly  
    - **Name:** errorType  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** errorMessage  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** errorContext  
**Type:** string  
**Attributes:** public|readonly  
    - **Name:** stackTraceHash  
**Type:** string  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Aggregated Error Reporting
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-AT-003
    
**Purpose:** To capture and log information about client-side errors for aggregation and analysis, helping to identify and prioritize bugs.  
**Logic Description:** This is a data contract definition. The client's global exception handler should catch unhandled exceptions and send this event. The 'stackTraceHash' is a hash of the full stack trace to allow for grouping of identical errors without sending potentially large or sensitive string data. PII must be stripped from the message and context. The eventName should be 'client_error'.  
**Documentation:**
    
    - **Summary:** Describes the schema for the 'client_error' analytics event. This provides a privacy-conscious way to monitor the stability and health of the application in the wild.
    
**Namespace:** PatternCipher.Analytics.Events  
**Metadata:**
    
    - **Category:** Analytics
    


---

# 2. Configuration

- **Feature Toggles:**
  
  
- **Database Configs:**
  
  - FirebaseAnalyticsProjectId
  - FirebaseAnalyticsApiKey
  


---

