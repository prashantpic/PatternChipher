# Specification

# 1. Entities

## 1.1. Player
Represents a player account, storing authentication details, core progress, and links to settings and consent. REQ-9-001, REQ-SRP-006, REQ-GS-009. Transactional updates are required for denormalized fields like totalStars to maintain consistency with PlayerLevelProgress.

### 1.1.3. Attributes

### 1.1.3.1. playerId
Unique identifier for the player, generated locally and used as the primary key.

#### 1.1.3.1.2. Type
Guid

#### 1.1.3.1.3. Is Required
True

#### 1.1.3.1.4. Is Primary Key
True

#### 1.1.3.1.5. Is Unique
True

### 1.1.3.2. firebaseUid
Firebase Authentication User ID, used for all cloud-based features like cloud save and leaderboards.

#### 1.1.3.2.2. Type
VARCHAR

#### 1.1.3.2.3. Is Required
False

#### 1.1.3.2.4. Is Unique
True

#### 1.1.3.2.5. Size
128

### 1.1.3.3. ageGateStatus
Status from the age gate check (e.g., 'UNKNOWN', 'UNDER_AGE_OF_CONSENT', 'OVER_AGE_OF_CONSENT'). REQ-UIX-016, REQ-CPS-005.

#### 1.1.3.3.2. Type
VARCHAR

#### 1.1.3.3.3. Is Required
True

#### 1.1.3.3.4. Size
50

#### 1.1.3.3.5. Default Value
UNKNOWN

### 1.1.3.4. totalStars
Aggregate total of stars collected across all levels. REQ-SRP-006.

#### 1.1.3.4.2. Type
INT

#### 1.1.3.4.3. Is Required
True

#### 1.1.3.4.4. Default Value
0

### 1.1.3.5. highestLevelPackUnlocked
The highest level pack number the player has unlocked.

#### 1.1.3.5.2. Type
INT

#### 1.1.3.5.3. Is Required
True

#### 1.1.3.5.4. Default Value
1

### 1.1.3.6. highestLevelUnlockedInPack
The highest level number within the highest unlocked pack.

#### 1.1.3.6.2. Type
INT

#### 1.1.3.6.3. Is Required
True

#### 1.1.3.6.4. Default Value
1

### 1.1.3.7. isDeleted
Flag for soft-deleting a player account.

#### 1.1.3.7.2. Type
BOOLEAN

#### 1.1.3.7.3. Is Required
True

#### 1.1.3.7.4. Default Value
false

### 1.1.3.8. lastLoginAt
Timestamp of the player's last login.

#### 1.1.3.8.2. Type
DateTime

#### 1.1.3.8.3. Is Required
False

### 1.1.3.9. createdAt
Timestamp when the player account was first created.

#### 1.1.3.9.2. Type
DateTime

#### 1.1.3.9.3. Is Required
True

### 1.1.3.10. updatedAt
Timestamp of the last update to the player record.

#### 1.1.3.10.2. Type
DateTime

#### 1.1.3.10.3. Is Required
True


### 1.1.4. Primary Keys

- playerId

### 1.1.5. Unique Constraints

### 1.1.5.1. uq_player_firebaseUid
#### 1.1.5.1.2. Columns

- firebaseUid


### 1.1.6. Indexes

### 1.1.6.1. idx_player_firebaseUid
#### 1.1.6.1.2. Columns

- firebaseUid

#### 1.1.6.1.3. Type
BTree


## 1.2. GameSettings
Stores player-specific customizable settings. REQ-GS-001 to REQ-GS-014.

### 1.2.3. Attributes

### 1.2.3.1. gameSettingsId
#### 1.2.3.1.2. Type
Guid

#### 1.2.3.1.3. Is Required
True

#### 1.2.3.1.4. Is Primary Key
True

#### 1.2.3.1.5. Is Unique
True

### 1.2.3.2. playerId
Foreign key linking to the Player entity (one-to-one relationship).

#### 1.2.3.2.2. Type
Guid

#### 1.2.3.2.3. Is Required
True

#### 1.2.3.2.4. Is Foreign Key
True

#### 1.2.3.2.5. Is Unique
True

### 1.2.3.3. bgmVolume
#### 1.2.3.3.2. Type
DECIMAL

#### 1.2.3.3.3. Is Required
True

#### 1.2.3.3.4. Precision
3

#### 1.2.3.3.5. Scale
2

#### 1.2.3.3.6. Default Value
0.8

#### 1.2.3.3.7. Constraints

- RANGE(0.0, 1.0)

### 1.2.3.4. sfxVolume
#### 1.2.3.4.2. Type
DECIMAL

#### 1.2.3.4.3. Is Required
True

#### 1.2.3.4.4. Precision
3

#### 1.2.3.4.5. Scale
2

#### 1.2.3.4.6. Default Value
1.0

#### 1.2.3.4.7. Constraints

- RANGE(0.0, 1.0)

### 1.2.3.5. colorblindMode
Selected colorblind mode (e.g., 'NONE', 'DEUTERANOPIA', 'PROTANOPIA'). REQ-GS-004.

#### 1.2.3.5.2. Type
VARCHAR

#### 1.2.3.5.3. Is Required
True

#### 1.2.3.5.4. Size
50

#### 1.2.3.5.5. Default Value
NONE

### 1.2.3.6. isReducedMotionEnabled
Flag to reduce non-essential animations. REQ-GS-006.

#### 1.2.3.6.2. Type
BOOLEAN

#### 1.2.3.6.3. Is Required
True

#### 1.2.3.6.4. Default Value
false

### 1.2.3.7. isHapticsEnabled
Flag to enable/disable haptic feedback. REQ-GS-007.

#### 1.2.3.7.2. Type
BOOLEAN

#### 1.2.3.7.3. Is Required
True

#### 1.2.3.7.4. Default Value
true

### 1.2.3.8. languageCode
Selected language code (e.g., 'en-US', 'es-ES'). REQ-GS-012.

#### 1.2.3.8.2. Type
VARCHAR

#### 1.2.3.8.3. Is Required
True

#### 1.2.3.8.4. Size
10

#### 1.2.3.8.5. Default Value
en-US

### 1.2.3.9. isCloudSaveEnabled
Player's choice to enable or disable cloud save. REQ-GS-009.

#### 1.2.3.9.2. Type
BOOLEAN

#### 1.2.3.9.3. Is Required
True

#### 1.2.3.9.4. Default Value
false

### 1.2.3.10. updatedAt
#### 1.2.3.10.2. Type
DateTime

#### 1.2.3.10.3. Is Required
True


### 1.2.4. Primary Keys

- gameSettingsId

### 1.2.5. Unique Constraints

### 1.2.5.1. uq_gamesettings_playerId
#### 1.2.5.1.2. Columns

- playerId


### 1.2.6. Indexes

### 1.2.6.1. idx_gamesettings_playerId
#### 1.2.6.1.2. Columns

- playerId

#### 1.2.6.1.3. Type
Hash


## 1.3. UserConsent
Tracks player consent for various features like analytics and terms of service. REQ-CPS-003, REQ-GS-011.

### 1.3.3. Attributes

### 1.3.3.1. userConsentId
#### 1.3.3.1.2. Type
Guid

#### 1.3.3.1.3. Is Required
True

#### 1.3.3.1.4. Is Primary Key
True

#### 1.3.3.1.5. Is Unique
True

### 1.3.3.2. playerId
#### 1.3.3.2.2. Type
Guid

#### 1.3.3.2.3. Is Required
True

#### 1.3.3.2.4. Is Foreign Key
True

### 1.3.3.3. consentType
Type of consent (e.g., 'ANALYTICS', 'TERMS_OF_SERVICE', 'PRIVACY_POLICY').

#### 1.3.3.3.2. Type
VARCHAR

#### 1.3.3.3.3. Is Required
True

#### 1.3.3.3.4. Size
50

### 1.3.3.4. isGiven
#### 1.3.3.4.2. Type
BOOLEAN

#### 1.3.3.4.3. Is Required
True

#### 1.3.3.4.4. Default Value
false

### 1.3.3.5. version
The version of the policy or terms the consent was given for.

#### 1.3.3.5.2. Type
VARCHAR

#### 1.3.3.5.3. Is Required
True

#### 1.3.3.5.4. Size
20

### 1.3.3.6. grantedAt
Timestamp when the consent status was last updated.

#### 1.3.3.6.2. Type
DateTime

#### 1.3.3.6.3. Is Required
True


### 1.3.4. Primary Keys

- userConsentId

### 1.3.5. Unique Constraints

### 1.3.5.1. uq_userconsent_player_type
#### 1.3.5.1.2. Columns

- playerId
- consentType


### 1.3.6. Indexes

### 1.3.6.1. idx_userconsent_player_type
#### 1.3.6.1.2. Columns

- playerId
- consentType

#### 1.3.6.1.3. Type
BTree


## 1.4. LevelDefinition
Stores the definition and metadata for a single, procedurally generated puzzle level. REQ-PCGDS-001, REQ-PCGDS-002.

### 1.4.3. Attributes

### 1.4.3.1. levelDefinitionId
#### 1.4.3.1.2. Type
Guid

#### 1.4.3.1.3. Is Required
True

#### 1.4.3.1.4. Is Primary Key
True

#### 1.4.3.1.5. Is Unique
True

### 1.4.3.2. levelPack
The pack or world this level belongs to.

#### 1.4.3.2.2. Type
INT

#### 1.4.3.2.3. Is Required
True

### 1.4.3.3. levelNumber
The number of the level within its pack.

#### 1.4.3.3.2. Type
INT

#### 1.4.3.3.3. Is Required
True

### 1.4.3.4. puzzleType
The type of puzzle (e.g., 'DirectMatch', 'RuleBased', 'Symmetry'). REQ-APD-001 to REQ-APD-004.

#### 1.4.3.4.2. Type
VARCHAR

#### 1.4.3.4.3. Is Required
True

#### 1.4.3.4.4. Size
50

### 1.4.3.5. generationSeed
The seed used for procedural generation to allow for reproducibility.

#### 1.4.3.5.2. Type
BIGINT

#### 1.4.3.5.3. Is Required
True

### 1.4.3.6. gridConfiguration
JSON object describing the initial state of the game grid.

#### 1.4.3.6.2. Type
JSON

#### 1.4.3.6.3. Is Required
True

### 1.4.3.7. goalConfiguration
JSON object describing the target pattern or rules for level completion. REQ-CGMI-004.

#### 1.4.3.7.2. Type
JSON

#### 1.4.3.7.3. Is Required
True

### 1.4.3.8. parMoves
The target 'par' move count for optimal completion. REQ-SRP-002.

#### 1.4.3.8.2. Type
INT

#### 1.4.3.8.3. Is Required
True

### 1.4.3.9. solutionPath
A valid solution path generated by the solver, used for hints and solvability proof. REQ-PCGDS-002.

#### 1.4.3.9.2. Type
JSON

#### 1.4.3.9.3. Is Required
True

### 1.4.3.10. difficultyScore
A calculated difficulty score for balancing. REQ-PCGDS-003.

#### 1.4.3.10.2. Type
DECIMAL

#### 1.4.3.10.3. Is Required
True

#### 1.4.3.10.4. Precision
5

#### 1.4.3.10.5. Scale
2

### 1.4.3.11. createdAt
#### 1.4.3.11.2. Type
DateTime

#### 1.4.3.11.3. Is Required
True


### 1.4.4. Primary Keys

- levelDefinitionId

### 1.4.5. Unique Constraints

### 1.4.5.1. uq_leveldefinition_pack_number
#### 1.4.5.1.2. Columns

- levelPack
- levelNumber


### 1.4.6. Indexes

### 1.4.6.1. idx_leveldefinition_pack_number
#### 1.4.6.1.2. Columns

- levelPack
- levelNumber

#### 1.4.6.1.3. Type
BTree


### 1.4.7. Caching Strategy

- **Type:** Distributed Cache (Redis)
- **Key:** levelDefinitionId or 'pack:level'
- **Ttl:** Long / Permanent
- **Notes:** Level definitions are immutable, making them ideal candidates for caching to reduce database load and level load times.

## 1.5. PlayerLevelProgress
Tracks a player's progress and best performance for each level. REQ-SRP-006, REQ-SRP-007.

### 1.5.3. Attributes

### 1.5.3.1. playerLevelProgressId
#### 1.5.3.1.2. Type
Guid

#### 1.5.3.1.3. Is Required
True

#### 1.5.3.1.4. Is Primary Key
True

#### 1.5.3.1.5. Is Unique
True

### 1.5.3.2. playerId
#### 1.5.3.2.2. Type
Guid

#### 1.5.3.2.3. Is Required
True

#### 1.5.3.2.4. Is Foreign Key
True

### 1.5.3.3. levelDefinitionId
#### 1.5.3.3.2. Type
Guid

#### 1.5.3.3.3. Is Required
True

#### 1.5.3.3.4. Is Foreign Key
True

### 1.5.3.4. isUnlocked
#### 1.5.3.4.2. Type
BOOLEAN

#### 1.5.3.4.3. Is Required
True

#### 1.5.3.4.4. Default Value
false

### 1.5.3.5. isCompleted
#### 1.5.3.5.2. Type
BOOLEAN

#### 1.5.3.5.3. Is Required
True

#### 1.5.3.5.4. Default Value
false

### 1.5.3.6. bestScore
#### 1.5.3.6.2. Type
INT

#### 1.5.3.6.3. Is Required
False

### 1.5.3.7. bestMoves
#### 1.5.3.7.2. Type
INT

#### 1.5.3.7.3. Is Required
False

### 1.5.3.8. bestTimeSeconds
#### 1.5.3.8.2. Type
INT

#### 1.5.3.8.3. Is Required
False

### 1.5.3.9. starsAwarded
#### 1.5.3.9.2. Type
INT

#### 1.5.3.9.3. Is Required
True

#### 1.5.3.9.4. Default Value
0

#### 1.5.3.9.5. Constraints

- RANGE(0, 3)

### 1.5.3.10. firstCompletedAt
#### 1.5.3.10.2. Type
DateTime

#### 1.5.3.10.3. Is Required
False

### 1.5.3.11. lastPlayedAt
#### 1.5.3.11.2. Type
DateTime

#### 1.5.3.11.3. Is Required
False


### 1.5.4. Primary Keys

- playerLevelProgressId

### 1.5.5. Unique Constraints

### 1.5.5.1. uq_playerlevelprogress_player_level
#### 1.5.5.1.2. Columns

- playerId
- levelDefinitionId


### 1.5.6. Indexes

### 1.5.6.1. idx_playerlevelprogress_player_level
#### 1.5.6.1.2. Columns

- playerId
- levelDefinitionId

#### 1.5.6.1.3. Type
BTree


## 1.6. Leaderboard
Defines a specific leaderboard for tracking player scores. REQ-SRP-008.

### 1.6.3. Attributes

### 1.6.3.1. leaderboardId
#### 1.6.3.1.2. Type
Guid

#### 1.6.3.1.3. Is Required
True

#### 1.6.3.1.4. Is Primary Key
True

#### 1.6.3.1.5. Is Unique
True

### 1.6.3.2. name
Unique name for the leaderboard (e.g., 'Global_Stars', 'Level_5_Time_Attack').

#### 1.6.3.2.2. Type
VARCHAR

#### 1.6.3.2.3. Is Required
True

#### 1.6.3.2.4. Is Unique
True

#### 1.6.3.2.5. Size
100

### 1.6.3.3. scoreType
The metric being ranked ('SCORE', 'MOVES', 'TIME').

#### 1.6.3.3.2. Type
VARCHAR

#### 1.6.3.3.3. Is Required
True

#### 1.6.3.3.4. Size
20

### 1.6.3.4. sortOrder
Sort order for scores ('ASC' for time/moves, 'DESC' for score).

#### 1.6.3.4.2. Type
VARCHAR

#### 1.6.3.4.3. Is Required
True

#### 1.6.3.4.4. Size
4

#### 1.6.3.4.5. Default Value
DESC

### 1.6.3.5. resetFrequency
How often the leaderboard resets ('NEVER', 'DAILY', 'WEEKLY').

#### 1.6.3.5.2. Type
VARCHAR

#### 1.6.3.5.3. Is Required
True

#### 1.6.3.5.4. Size
20

#### 1.6.3.5.5. Default Value
NEVER

### 1.6.3.6. isActive
#### 1.6.3.6.2. Type
BOOLEAN

#### 1.6.3.6.3. Is Required
True

#### 1.6.3.6.4. Default Value
true


### 1.6.4. Primary Keys

- leaderboardId

### 1.6.5. Unique Constraints

### 1.6.5.1. uq_leaderboard_name
#### 1.6.5.1.2. Columns

- name


### 1.6.6. Indexes


### 1.6.7. Caching Strategy

- **Type:** Distributed Cache (Redis)
- **Key:** leaderboardId
- **Ttl:** Short (1-5 minutes)
- **Notes:** Cache top N (e.g., 100) entries for each active leaderboard to vastly reduce read load for the most common queries.

## 1.7. LeaderboardEntry
Represents a single player's entry on a leaderboard. REQ-SRP-008, REQ-SRP-009.

### 1.7.3. Attributes

### 1.7.3.1. leaderboardEntryId
#### 1.7.3.1.2. Type
Guid

#### 1.7.3.1.3. Is Required
True

#### 1.7.3.1.4. Is Primary Key
True

#### 1.7.3.1.5. Is Unique
True

### 1.7.3.2. leaderboardId
#### 1.7.3.2.2. Type
Guid

#### 1.7.3.2.3. Is Required
True

#### 1.7.3.2.4. Is Foreign Key
True

### 1.7.3.3. playerId
#### 1.7.3.3.2. Type
Guid

#### 1.7.3.3.3. Is Required
True

#### 1.7.3.3.4. Is Foreign Key
True

### 1.7.3.4. playerName
Denormalized player name to avoid joins on leaderboard fetch. Must be kept in sync with the Player table.

#### 1.7.3.4.2. Type
VARCHAR

#### 1.7.3.4.3. Is Required
False

#### 1.7.3.4.4. Size
100

### 1.7.3.5. scoreValue
The player's score. Stored as BIGINT to accommodate time in milliseconds or large scores.

#### 1.7.3.5.2. Type
BIGINT

#### 1.7.3.5.3. Is Required
True

### 1.7.3.6. rank
The calculated rank of the player. Can be updated periodically by a background job.

#### 1.7.3.6.2. Type
INT

#### 1.7.3.6.3. Is Required
False

### 1.7.3.7. isDeleted
Flag for soft-deleting an entry, e.g., for cheating.

#### 1.7.3.7.2. Type
BOOLEAN

#### 1.7.3.7.3. Is Required
True

#### 1.7.3.7.4. Default Value
false

### 1.7.3.8. submittedAt
Timestamp of when the score was submitted.

#### 1.7.3.8.2. Type
DateTime

#### 1.7.3.8.3. Is Required
True


### 1.7.4. Primary Keys

- leaderboardEntryId

### 1.7.5. Unique Constraints

### 1.7.5.1. uq_leaderboardentry_leaderboard_player
#### 1.7.5.1.2. Columns

- leaderboardId
- playerId


### 1.7.6. Indexes

### 1.7.6.1. idx_leaderboardentry_ranking
#### 1.7.6.1.2. Columns

- leaderboardId
- scoreValue DESC
- submittedAt ASC

#### 1.7.6.1.3. Type
BTree


## 1.8. Achievement
Defines an achievement that players can unlock. REQ-SRP-010.

### 1.8.3. Attributes

### 1.8.3.1. achievementId
#### 1.8.3.1.2. Type
Guid

#### 1.8.3.1.3. Is Required
True

#### 1.8.3.1.4. Is Primary Key
True

#### 1.8.3.1.5. Is Unique
True

### 1.8.3.2. name
#### 1.8.3.2.2. Type
VARCHAR

#### 1.8.3.2.3. Is Required
True

#### 1.8.3.2.4. Size
100

### 1.8.3.3. description
#### 1.8.3.3.2. Type
TEXT

#### 1.8.3.3.3. Is Required
True

### 1.8.3.4. iconUrl
#### 1.8.3.4.2. Type
VARCHAR

#### 1.8.3.4.3. Is Required
False

#### 1.8.3.4.4. Size
255

### 1.8.3.5. triggerCondition
JSON object defining the rules to unlock the achievement (e.g., '{"type": "STARS_COLLECTED", "value": 100}').

#### 1.8.3.5.2. Type
JSON

#### 1.8.3.5.3. Is Required
True

### 1.8.3.6. isHidden
Whether the achievement is hidden until unlocked.

#### 1.8.3.6.2. Type
BOOLEAN

#### 1.8.3.6.3. Is Required
True

#### 1.8.3.6.4. Default Value
false

### 1.8.3.7. isActive
#### 1.8.3.7.2. Type
BOOLEAN

#### 1.8.3.7.3. Is Required
True

#### 1.8.3.7.4. Default Value
true


### 1.8.4. Primary Keys

- achievementId

### 1.8.5. Unique Constraints


### 1.8.6. Indexes


## 1.9. PlayerAchievement
A join table linking players to the achievements they have unlocked. REQ-SRP-010.

### 1.9.3. Attributes

### 1.9.3.1. playerAchievementId
#### 1.9.3.1.2. Type
Guid

#### 1.9.3.1.3. Is Required
True

#### 1.9.3.1.4. Is Primary Key
True

#### 1.9.3.1.5. Is Unique
True

### 1.9.3.2. playerId
#### 1.9.3.2.2. Type
Guid

#### 1.9.3.2.3. Is Required
True

#### 1.9.3.2.4. Is Foreign Key
True

### 1.9.3.3. achievementId
#### 1.9.3.3.2. Type
Guid

#### 1.9.3.3.3. Is Required
True

#### 1.9.3.3.4. Is Foreign Key
True

### 1.9.3.4. progress
Current progress towards multi-step achievements.

#### 1.9.3.4.2. Type
INT

#### 1.9.3.4.3. Is Required
True

#### 1.9.3.4.4. Default Value
0

### 1.9.3.5. isUnlocked
#### 1.9.3.5.2. Type
BOOLEAN

#### 1.9.3.5.3. Is Required
True

#### 1.9.3.5.4. Default Value
false

### 1.9.3.6. unlockedAt
Timestamp of when the achievement was unlocked.

#### 1.9.3.6.2. Type
DateTime

#### 1.9.3.6.3. Is Required
False


### 1.9.4. Primary Keys

- playerAchievementId

### 1.9.5. Unique Constraints

### 1.9.5.1. uq_playerachievement_player_achievement
#### 1.9.5.1.2. Columns

- playerId
- achievementId


### 1.9.6. Indexes

### 1.9.6.1. idx_playerachievement_player_achievement
#### 1.9.6.1.2. Columns

- playerId
- achievementId

#### 1.9.6.1.3. Type
BTree


## 1.10. RemoteConfig
Stores key-value pairs for remote configuration of game parameters, enabling dynamic balancing. REQ-8-006, REQ-PCGDS-007.

### 1.10.3. Attributes

### 1.10.3.1. remoteConfigId
#### 1.10.3.1.2. Type
Guid

#### 1.10.3.1.3. Is Required
True

#### 1.10.3.1.4. Is Primary Key
True

#### 1.10.3.1.5. Is Unique
True

### 1.10.3.2. configKey
The unique key for the configuration parameter.

#### 1.10.3.2.2. Type
VARCHAR

#### 1.10.3.2.3. Is Required
True

#### 1.10.3.2.4. Is Unique
True

#### 1.10.3.2.5. Size
100

### 1.10.3.3. configValue
The value of the configuration parameter, stored as JSON to support complex types.

#### 1.10.3.3.2. Type
JSON

#### 1.10.3.3.3. Is Required
True

### 1.10.3.4. description
Explanation of what this configuration parameter controls.

#### 1.10.3.4.2. Type
TEXT

#### 1.10.3.4.3. Is Required
False

### 1.10.3.5. isActive
#### 1.10.3.5.2. Type
BOOLEAN

#### 1.10.3.5.3. Is Required
True

#### 1.10.3.5.4. Default Value
true

### 1.10.3.6. updatedAt
#### 1.10.3.6.2. Type
DateTime

#### 1.10.3.6.3. Is Required
True


### 1.10.4. Primary Keys

- remoteConfigId

### 1.10.5. Unique Constraints

### 1.10.5.1. uq_remoteconfig_key
#### 1.10.5.1.2. Columns

- configKey


### 1.10.6. Indexes


### 1.10.7. Caching Strategy

- **Type:** Client-side & CDN
- **Key:** configKey or version identifier
- **Ttl:** Managed via ETag/versioning
- **Notes:** Strong caching reduces startup latency and backend load. Clients should only fetch updates.

## 1.11. AnalyticsEvent
A log of significant, anonymized player events for analysis and game balancing. REQ-8-001, REQ-8-002. To optimize analytics, frequently queried fields like levelId, moves, and timeInSeconds have been promoted from the JSON payload to dedicated columns.

### 1.11.3. Attributes

### 1.11.3.1. analyticsEventId
#### 1.11.3.1.2. Type
Guid

#### 1.11.3.1.3. Is Required
True

#### 1.11.3.1.4. Is Primary Key
True

#### 1.11.3.1.5. Is Unique
True

### 1.11.3.2. playerId
Anonymized or pseudonymized player identifier.

#### 1.11.3.2.2. Type
Guid

#### 1.11.3.2.3. Is Required
True

#### 1.11.3.2.4. Is Foreign Key
True

### 1.11.3.3. sessionId
Identifier for the game session in which the event occurred.

#### 1.11.3.3.2. Type
Guid

#### 1.11.3.3.3. Is Required
True

### 1.11.3.4. eventName
Name of the event (e.g., 'LEVEL_START', 'LEVEL_COMPLETE', 'HINT_USED').

#### 1.11.3.4.2. Type
VARCHAR

#### 1.11.3.4.3. Is Required
True

#### 1.11.3.4.4. Size
100

### 1.11.3.5. eventPayload
JSON object containing event-specific data (e.g., levelId, moves, time).

#### 1.11.3.5.2. Type
JSON

#### 1.11.3.5.3. Is Required
False

### 1.11.3.6. levelId
Promoted from payload: The level associated with the event.

#### 1.11.3.6.2. Type
Guid

#### 1.11.3.6.3. Is Required
False

#### 1.11.3.6.4. Is Foreign Key
True

### 1.11.3.7. moves
Promoted from payload: The move count at the time of the event.

#### 1.11.3.7.2. Type
INT

#### 1.11.3.7.3. Is Required
False

### 1.11.3.8. timeInSeconds
Promoted from payload: The time elapsed in the level at the time of the event.

#### 1.11.3.8.2. Type
INT

#### 1.11.3.8.3. Is Required
False

### 1.11.3.9. eventTimestamp
#### 1.11.3.9.2. Type
DateTime

#### 1.11.3.9.3. Is Required
True


### 1.11.4. Primary Keys

- analyticsEventId

### 1.11.5. Unique Constraints


### 1.11.6. Indexes

### 1.11.6.1. idx_analyticsevent_name_timestamp
#### 1.11.6.1.2. Columns

- eventName
- eventTimestamp

#### 1.11.6.1.3. Type
BTree

### 1.11.6.2. idx_analyticsevent_player_timestamp
#### 1.11.6.2.2. Columns

- playerId
- eventTimestamp

#### 1.11.6.2.3. Type
BTree


### 1.11.7. Partitioning

- **Type:** Range
- **Column:** eventTimestamp
- **Strategy:** Monthly or Weekly
- **Notes:** Improves time-based queries and allows for efficient archival/deletion of old data.



---

# 2. Relations

## 2.1. PlayerGameSettings
### 2.1.3. Source Entity
Player

### 2.1.4. Target Entity
GameSettings

### 2.1.5. Type
OneToOne

### 2.1.6. Source Multiplicity
1

### 2.1.7. Target Multiplicity
1

### 2.1.8. Cascade Delete
True

### 2.1.9. Is Identifying
True

### 2.1.10. On Delete
Cascade

### 2.1.11. On Update
Cascade

## 2.2. PlayerConsents
### 2.2.3. Source Entity
Player

### 2.2.4. Target Entity
UserConsent

### 2.2.5. Type
OneToMany

### 2.2.6. Source Multiplicity
1

### 2.2.7. Target Multiplicity
0..*

### 2.2.8. Cascade Delete
True

### 2.2.9. Is Identifying
True

### 2.2.10. On Delete
Cascade

### 2.2.11. On Update
Cascade

## 2.3. PlayerProgressRecords
### 2.3.3. Source Entity
Player

### 2.3.4. Target Entity
PlayerLevelProgress

### 2.3.5. Type
OneToMany

### 2.3.6. Source Multiplicity
1

### 2.3.7. Target Multiplicity
0..*

### 2.3.8. Cascade Delete
True

### 2.3.9. Is Identifying
True

### 2.3.10. On Delete
Cascade

### 2.3.11. On Update
Cascade

## 2.4. LevelProgressEntries
### 2.4.3. Source Entity
LevelDefinition

### 2.4.4. Target Entity
PlayerLevelProgress

### 2.4.5. Type
OneToMany

### 2.4.6. Source Multiplicity
1

### 2.4.7. Target Multiplicity
0..*

### 2.4.8. Cascade Delete
False

### 2.4.9. Is Identifying
False

### 2.4.10. On Delete
Restrict

### 2.4.11. On Update
Cascade

## 2.5. LeaderboardHasEntries
### 2.5.3. Source Entity
Leaderboard

### 2.5.4. Target Entity
LeaderboardEntry

### 2.5.5. Type
OneToMany

### 2.5.6. Source Multiplicity
1

### 2.5.7. Target Multiplicity
0..*

### 2.5.8. Cascade Delete
True

### 2.5.9. Is Identifying
True

### 2.5.10. On Delete
Cascade

### 2.5.11. On Update
Cascade

## 2.6. PlayerLeaderboardEntries
### 2.6.3. Source Entity
Player

### 2.6.4. Target Entity
LeaderboardEntry

### 2.6.5. Type
OneToMany

### 2.6.6. Source Multiplicity
1

### 2.6.7. Target Multiplicity
0..*

### 2.6.8. Cascade Delete
True

### 2.6.9. Is Identifying
True

### 2.6.10. On Delete
Cascade

### 2.6.11. On Update
Cascade

## 2.7. AchievementUnlocks
### 2.7.3. Source Entity
Achievement

### 2.7.4. Target Entity
PlayerAchievement

### 2.7.5. Type
OneToMany

### 2.7.6. Source Multiplicity
1

### 2.7.7. Target Multiplicity
0..*

### 2.7.8. Cascade Delete
True

### 2.7.9. Is Identifying
True

### 2.7.10. On Delete
Cascade

### 2.7.11. On Update
Cascade

## 2.8. PlayerUnlockedAchievements
### 2.8.3. Source Entity
Player

### 2.8.4. Target Entity
PlayerAchievement

### 2.8.5. Type
OneToMany

### 2.8.6. Source Multiplicity
1

### 2.8.7. Target Multiplicity
0..*

### 2.8.8. Cascade Delete
True

### 2.8.9. Is Identifying
True

### 2.8.10. On Delete
Cascade

### 2.8.11. On Update
Cascade

## 2.9. PlayerAnalyticsEvents
### 2.9.3. Source Entity
Player

### 2.9.4. Target Entity
AnalyticsEvent

### 2.9.5. Type
OneToMany

### 2.9.6. Source Multiplicity
1

### 2.9.7. Target Multiplicity
0..*

### 2.9.8. Cascade Delete
False

### 2.9.9. Is Identifying
False

### 2.9.10. On Delete
SetNull

### 2.9.11. On Update
Cascade



---

