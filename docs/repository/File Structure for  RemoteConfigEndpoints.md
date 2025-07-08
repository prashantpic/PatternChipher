# Specification

# 1. Files

- **Path:** firebase/remote-config/remote_config_template.json  
**Description:** Defines the complete, version-controlled template for Firebase Remote Config. This file acts as the single source of truth for all dynamically configurable game parameters, enabling live-ops balancing, feature flagging, and tuning of gameplay mechanics without requiring client application updates. Its structure is parsed by the game client to adjust behavior in real-time.  
**Template:** JSON Configuration Template  
**Dependency Level:** 0  
**Name:** remote_config_template  
**Type:** Configuration  
**Relative Path:** remote_config_template.json  
**Repository Id:** REPO-PATT-009  
**Pattern Ids:**
    
    - ExternalConfigurationStore
    
**Members:**
    
    - **Name:** schemaVersion  
**Type:** String  
**Attributes:** public|readonly  
    - **Name:** difficultyTiers  
**Type:** JSONArray  
**Attributes:** public|readonly  
    - **Name:** scoringRules  
**Type:** JSONObject  
**Attributes:** public|readonly  
    - **Name:** pcgRules  
**Type:** JSONObject  
**Attributes:** public|readonly  
    - **Name:** featureFlags  
**Type:** JSONObject  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Dynamic Difficulty Configuration
    - Dynamic Scoring Configuration
    - Procedural Generation Parameterization
    - Feature Flag Management
    
**Requirement Ids:**
    
    - NFR-M-002
    - NFR-M-002a
    - FR-L-001
    - BR-DIFF-001
    
**Purpose:** To define all server-side configurable parameters that control game balance, progression, and feature availability. This file serves as the template for the Firebase Remote Config service.  
**Logic Description:** This JSON file contains a root object with several key properties:
- 'schemaVersion': A string for tracking the configuration structure version.
- 'difficultyTiers': An array of objects, where each object defines a difficulty tier. Each tier object contains parameters like 'tierId', 'gridSizeRange' (e.g., '3x3-4x4'), 'symbolCountRange' (e.g., '3-5'), 'minOptimalMoves', and 'specialTileTypesAllowed' (array of strings).
- 'scoringRules': An object containing parameters like 'baseScorePerLevel', 'efficiencyBonusMultiplier', and 'speedBonusThresholds'.
- 'pcgRules': An object defining parameters for the procedural content generator. This includes 'controlledIntroduction' which is an array of objects specifying at which player level or progression milestone a new mechanic, puzzle type, or special tile is introduced.
- 'featureFlags': An object containing boolean flags for major features, such as 'isCloudSaveEnabled', 'isLeaderboardsEnabled', 'isDailyChallengeActive'.

All changes to this file in production must follow the review and approval process outlined in NFR-M-002a.  
**Documentation:**
    
    - **Summary:** This file is the master template for Firebase Remote Config. It is not code but configuration data. The game client fetches this configuration at startup to tailor the gameplay experience, allowing for dynamic adjustments to difficulty, scoring, and features without shipping a new client version. Its structure must be strictly followed by the client-side parsing logic.
    
**Namespace:** PatternCipher.Services.RemoteConfig  
**Metadata:**
    
    - **Category:** Configuration
    


---

# 2. Configuration

- **Feature Toggles:**
  
  
- **Database Configs:**
  
  


---

