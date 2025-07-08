# Software Design Specification (SDS) for REPO-PATT-009: RemoteConfigEndpoints

## 1. Introduction

This document outlines the design specification for the `RemoteConfigEndpoints` repository. This repository's sole purpose is to define and manage the dynamic game configuration served by Firebase Remote Config. It is not a code repository but a configuration-as-code repository.

The primary artifact is `remote_config_template.json`, which acts as the version-controlled source of truth for all game parameters that can be tuned live. This allows for dynamic game balancing, difficulty curve adjustments, feature flagging, and A/B testing without requiring a new client application release. This directly addresses requirements for maintainability (`NFR-M-002`), controlled feature introduction (`FR-L-001`), and dynamic difficulty tuning (`BR-DIFF-001`).

---

## 2. Configuration Schema: `remote_config_template.json`

This file defines the master structure for the parameters fetched by the game client. The client application must implement robust parsing logic to handle this structure, including providing sensible default values if the configuration fails to load or if a specific key is missing.

### 2.1. Root Object

The root of the JSON file is a single object containing the following top-level properties.

json
{
  "schemaVersion": "1.0.0",
  "difficultyTiers": [...],
  "scoringRules": {...},
  "pcgRules": {...},
  "featureFlags": {...}
}


### 2.2. `schemaVersion`
- **Type**: `String`
- **Description**: A semantic version string (e.g., "1.0.0") that indicates the version of the configuration schema itself. The client application will use this to ensure compatibility with its parsing logic, logging a warning if there is a major version mismatch.
- **Example**: `"1.0.0"`

### 2.3. `featureFlags`
- **Type**: `JSONObject`
- **Description**: A collection of boolean flags to remotely enable or disable major game features. This allows for phased rollouts and A/B testing.
- **Requirement Mapping**: `NFR-M-002`
- **Structure**:
  json
  "featureFlags": {
    "isCloudSaveEnabled": true,
    "isLeaderboardsEnabled": true,
    "isAchievementsEnabled": true,
    "isDailyChallengeActive": false,
    "isStoreEnabled": false
  }
  
- **Key Descriptions**:
    - `isCloudSaveEnabled` (`Boolean`): Globally enables or disables the Cloud Save feature.
    - `isLeaderboardsEnabled` (`Boolean`): Globally enables or disables the Leaderboards feature.
    - `isAchievementsEnabled` (`Boolean`): Globally enables or disables the Achievements feature.
    - `isDailyChallengeActive` (`Boolean`): Enables or disables a "Daily Challenge" mode if implemented.
    - `isStoreEnabled` (`Boolean`): Enables or disables an in-game store if implemented.

### 2.4. `scoringRules`
- **Type**: `JSONObject`
- **Description**: Defines the global rules and multipliers for score calculation.
- **Requirement Mapping**: `NFR-M-002`, `BR-STAR-001`
- **Structure**:
  json
  "scoringRules": {
    "baseScorePerLevel": 1000,
    "efficiencyBonus": {
      "multiplierPerMoveUnderPar": 50,
      "maxBonus": 2500
    },
    "speedBonus": {
      "baseTimeSeconds": 120,
      "bonusPerSecondUnder": 10,
      "maxBonus": 1200
    },
    "comboBonus": {
      "baseBonus": 100,
      "multiplierPerChain": 1.5
    },
    "starThresholds": {
      "star2": 5000,
      "star3": 8000
    }
  }
  
- **Key Descriptions**:
    - `baseScorePerLevel` (`Integer`): The base points awarded for completing any level.
    - `efficiencyBonus` (`JSONObject`): Parameters for the move efficiency bonus.
    - `speedBonus` (`JSONObject`): Parameters for the speed bonus in timed levels.
    - `comboBonus` (`JSONObject`): Parameters for combo bonuses in applicable puzzle types.
    - `starThresholds` (`JSONObject`): The score thresholds required to earn 2 and 3 stars.

### 2.5. `difficultyTiers`
- **Type**: `JSONArray` of `JSONObjects`
- **Description**: An array defining parameters for distinct difficulty tiers. This allows for a structured progression in game difficulty.
- **Requirement Mapping**: `BR-DIFF-001`, `FR-L-002`
- **Object Structure**:
  json
   {
      "tierId": "BEGINNER",
      "tierName": "Beginner Puzzles",
      "unlockCondition": { "type": "LEVEL_COUNT", "value": 0 },
      "gridSizeRange": ["3x3", "4x4"],
      "symbolCountRange": [3, 4],
      "minOptimalMovesRange": [3, 8],
      "specialTileTypesAllowed": [],
      "puzzleTypesAllowed": ["DIRECT_MATCH"]
    }
  
- **Key Descriptions**:
    - `tierId` (`String`): A unique machine-readable identifier for the tier.
    - `tierName` (`String`): A human-readable name for the tier, potentially for UI display.
    - `unlockCondition` (`JSONObject`): Defines the condition to unlock this tier (e.g., `{ "type": "STARS", "value": 50 }`).
    - `gridSizeRange` (`JSONArray` of `String`): An array of allowed grid sizes (e.g., "3x3", "4x4").
    - `symbolCountRange` (`JSONArray` of `Integer`): A `[min, max]` array for the number of unique symbols.
    - `minOptimalMovesRange` (`JSONArray` of `Integer`): A `[min, max]` array for the target optimal solution length.
    - `specialTileTypesAllowed` (`JSONArray` of `String`): An array of special tile IDs allowed in this tier.
    - `puzzleTypesAllowed` (`JSONArray` of `String`): An array of puzzle type IDs allowed in this tier.

### 2.6. `pcgRules`
- **Type**: `JSONObject`
- **Description**: Contains fine-grained rules for the Procedural Content Generator, especially for pacing the introduction of new game elements.
- **Requirement Mapping**: `FR-L-001` (Controlled Introduction)
- **Structure**:
  json
  "pcgRules": {
    "controlledIntroduction": [
      { "unlocksAtLevel": 5, "elementType": "SPECIAL_TILE", "elementId": "LOCKED" },
      { "unlocksAtLevel": 10, "elementType": "PUZZLE_TYPE", "elementId": "RULE_BASED" },
      { "unlocksAtStars": 100, "elementType": "SPECIAL_TILE", "elementId": "TRANSFORMER" }
    ]
  }
  
- **Key Descriptions**:
    - `controlledIntroduction` (`JSONArray` of `JSONObjects`): An ordered list defining when new game elements become available for procedural generation.
        - `unlocksAtLevel` / `unlocksAtStars` (`Integer`): The player progression milestone that triggers the unlock.
        - `elementType` (`String`): The type of element being unlocked (e.g., "SPECIAL_TILE", "PUZZLE_TYPE").
        - `elementId` (`String`): The unique ID of the element being unlocked.

---

## 3. Versioning and Management Process

This configuration file is a critical component for live operations. All modifications must be managed with extreme care.

- **Source Control**: `remote_config_template.json` must be version-controlled in the project's primary Git repository.
- **Change Management**: As per `NFR-M-002a`, any change to this file intended for the production environment must follow a strict change management process:
    1.  **Change Proposal**: A developer or game designer proposes changes in a dedicated feature branch.
    2.  **Pull Request (PR)**: A PR is created, detailing the changes and the justification.
    3.  **Review & Approval**: The PR must be reviewed and approved by designated stakeholders (e.g., Lead Developer, Lead Game Designer).
    4.  **Automated Validation**: A CI/CD pipeline step should validate the JSON syntax of the modified file.
    5.  **Staging Deployment**: Upon merging to the main branch, the updated configuration is automatically deployed to a non-production Firebase project (e.g., `staging`).
    6.  **QA Validation**: The changes are thoroughly tested in the staging environment by the QA team.
    7.  **Production Deployment**: After successful QA validation, the configuration is manually promoted to the production Firebase project. This step requires a final approval and should be documented in a change log.
- **Rollback Plan**: In case of a detrimental change in production, the previous known-good version of the configuration will be immediately re-published to the Firebase Remote Config console.

---

## 4. Client Integration Contract

The client application's `BackendServiceFacade` (`REPO-PATT-005`) is responsible for fetching and parsing this configuration.

- **Fetch Strategy**: The client should fetch the configuration at application startup and may implement a background refresh mechanism.
- **Default Values**: The client must contain a hardcoded set of default values matching the `remote_config_template.json` structure. These defaults will be used if the fetch fails or the user is offline.
- **Parsing**: The client will use a JSON parsing library to deserialize the fetched configuration into strongly-typed C# objects.
- **Error Handling**: If parsing fails (due to a schema mismatch or corrupted data), the client must fall back to the hardcoded default values and log an error to the analytics backend.
- **Versioning Check**: The client should compare the fetched `schemaVersion` with its expected version. A major version mismatch should trigger a non-fatal error log for monitoring purposes, indicating a potential need for a client update.