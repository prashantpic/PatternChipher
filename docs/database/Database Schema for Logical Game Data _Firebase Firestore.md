# Specification

# 1. Database Design

## 1.1. Player
Represents a player account, storing authentication details, core progress, and links to settings and consent. Implemented as a Firestore Collection with documents keyed by `firebaseUid` or `playerId`. REQ-9-001, REQ-SRP-006, REQ-GS-009. Denormalized fields like totalStars may require Cloud Functions for consistent updates in Firestore.

### 1.1.3. Attributes

### 1.1.3.1. playerId
Unique identifier for the player, generated locally or upon initial account creation. Used internally.

#### 1.1.3.1.2. Type
Guid

#### 1.1.3.1.3. Is Required
True

#### 1.1.3.1.4. Is Primary Key
True

#### 1.1.3.1.5. Is Unique
True

### 1.1.3.2. firebaseUid
Firebase Authentication User ID. Used as the primary key for documents in the Firestore 'players' collection. Required for cloud-based features.

#### 1.1.3.2.2. Type
VARCHAR

#### 1.1.3.2.3. Is Required
False

#### 1.1.3.2.4. Is Unique
True

#### 1.1.3.2.5. Size
128

### 1.1.3.3. ageGateStatus
Status from the age gate check ('UNKNOWN', 'UNDER_AGE_OF_CONSENT', 'OVER_AGE_OF_CONSENT'). REQ-UIX-016, REQ-CPS-005.

#### 1.1.3.3.2. Type
VARCHAR

#### 1.1.3.3.3. Is Required
True

#### 1.1.3.3.4. Size
50

#### 1.1.3.3.5. Default Value
UNKNOWN

### 1.1.3.4. totalStars
Aggregate total of stars collected across all levels. Denormalized sum from PlayerLevelProgress. Maintained via transactional updates or Cloud Functions. REQ-SRP-006.

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
False

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

- firebaseUid

### 1.1.5. Unique Constraints

### 1.1.5.1. uq_player_playerId
#### 1.1.5.1.2. Columns

- playerId


### 1.1.6. Indexes

### 1.1.6.1. idx_player_playerId
#### 1.1.6.1.2. Columns

- playerId

#### 1.1.6.1.3. Type
BTree


### 1.1.7. Caching Strategy

- **Type:** Client-side
- **Key:** player data by UID
- **Ttl:** Session lifetime
- **Notes:** Player profile and progress data are loaded on login and kept in memory, minimizing frequent database reads.

## 1.2. GameSettings
Stores player-specific customizable settings. Implemented as a subcollection or linked document under the Player document in Firestore. REQ-GS-001 to REQ-GS-014.

### 1.2.3. Attributes

### 1.2.3.1. gameSettingsId
Unique identifier for the settings document.

#### 1.2.3.1.2. Type
Guid

#### 1.2.3.1.3. Is Required
True

#### 1.2.3.1.4. Is Primary Key
True

#### 1.2.3.1.5. Is Unique
True

### 1.2.3.2. playerId
Foreign key linking to the Player entity (one-to-one relationship via `playerId`). Used as part of the document path or field for lookup if not a subcollection.

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
1

#### 1.2.3.4.7. Constraints

- RANGE(0.0, 1.0)

### 1.2.3.5. colorblindMode
Selected colorblind mode ('NONE', 'DEUTERANOPIA', 'PROTANOPIA', 'TRITANOPIA'). REQ-GS-004.

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
False

### 1.2.3.7. isHapticsEnabled
Flag to enable/disable haptic feedback. REQ-GS-007.

#### 1.2.3.7.2. Type
BOOLEAN

#### 1.2.3.7.3. Is Required
True

#### 1.2.3.7.4. Default Value
True

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
False

### 1.2.3.10. createdAt
Timestamp when the settings record was first created.

#### 1.2.3.10.2. Type
DateTime

#### 1.2.3.10.3. Is Required
True

### 1.2.3.11. updatedAt
Timestamp of the last update to the settings record.

#### 1.2.3.11.2. Type
DateTime

#### 1.2.3.11.3. Is Required
True


### 1.2.4. Primary Keys

- gameSettingsId

### 1.2.5. Unique Constraints


### 1.2.6. Indexes

### 1.2.6.1. idx_gamesettings_playerId
#### 1.2.6.1.2. Columns

- playerId

#### 1.2.6.1.3. Type
Hash


### 1.2.7. Caching Strategy

- **Type:** Client-side
- **Key:** player settings data
- **Ttl:** Session lifetime
- **Notes:** Settings are loaded on app start and kept in memory. Updates are synced periodically or on demand.

## 1.3. UserConsent
Tracks player consent for various features like analytics and terms of service. Implemented as a subcollection under the Player document or linked documents in Firestore. REQ-CPS-003, REQ-GS-011, FR-AT-002, NFR-LC-002.

### 1.3.3. Attributes

### 1.3.3.1. userConsentId
Unique identifier for the consent document.

#### 1.3.3.1.2. Type
Guid

#### 1.3.3.1.3. Is Required
True

#### 1.3.3.1.4. Is Primary Key
True

#### 1.3.3.1.5. Is Unique
True

### 1.3.3.2. playerId
Foreign key linking to the Player entity. Used as part of the document path or field for lookup.

#### 1.3.3.2.2. Type
Guid

#### 1.3.3.2.3. Is Required
True

#### 1.3.3.2.4. Is Foreign Key
True

### 1.3.3.3. consentType
Type of consent ('ANALYTICS', 'TERMS_OF_SERVICE', 'PRIVACY_POLICY', 'CLOUD_SAVE').

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
False

### 1.3.3.5. version
The version of the policy or terms the consent was given for.

#### 1.3.3.5.2. Type
VARCHAR

#### 1.3.3.5.3. Is Required
True

#### 1.3.3.5.4. Size
20

### 1.3.3.6. grantedAt
Timestamp when the consent status was last updated/granted.

#### 1.3.3.6.2. Type
DateTime

#### 1.3.3.6.3. Is Required
True

### 1.3.3.7. createdAt
Timestamp when the consent record was first created.

#### 1.3.3.7.2. Type
DateTime

#### 1.3.3.7.3. Is Required
True


### 1.3.4. Primary Keys

- userConsentId

### 1.3.5. Unique Constraints


### 1.3.6. Indexes

### 1.3.6.1. idx_userconsent_player_type
#### 1.3.6.1.2. Columns

- playerId
- consentType

#### 1.3.6.1.3. Type
BTree


### 1.3.7. Caching Strategy

- **Type:** Client-side
- **Key:** player consent data
- **Ttl:** Session lifetime
- **Notes:** Consent status is checked and cached on app start.

## 1.4. LevelDefinition
Stores the definition and metadata for a single, procedurally generated puzzle level. Implemented as a Firestore Collection of static level documents (potentially separate collections per pack). REQ-PCGDS-001, REQ-PCGDS-002, FR-L-001.

### 1.4.3. Attributes

### 1.4.3.1. levelDefinitionId
Unique identifier for the level definition document.

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
The type of puzzle ('DirectMatch', 'RuleBased', 'Symmetry', 'ChainReaction'). REQ-APD-001 to REQ-APD-004, FR-L-003.

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
JSON object describing the target pattern or rules for level completion. REQ-CGMI-004, FR-G-005.

#### 1.4.3.7.2. Type
JSON

#### 1.4.3.7.3. Is Required
True

### 1.4.3.8. parMoves
The target 'par' move count for optimal completion, determined by the solver. REQ-SRP-002, FR-S-002.

#### 1.4.3.8.2. Type
INT

#### 1.4.3.8.3. Is Required
True

### 1.4.3.9. solutionPath
A valid, optimal or near-optimal solution path generated by the solver, used for hints and solvability proof. Stored as a sequence of moves. REQ-PCGDS-002, FR-S-002, FR-U-010.

#### 1.4.3.9.2. Type
JSON

#### 1.4.3.9.3. Is Required
True

### 1.4.3.10. difficultyScore
A calculated difficulty score for balancing. REQ-PCGDS-003, FR-L-002.

#### 1.4.3.10.2. Type
DECIMAL

#### 1.4.3.10.3. Is Required
True

#### 1.4.3.10.4. Precision
5

#### 1.4.3.10.5. Scale
2

### 1.4.3.11. createdAt
Timestamp when the level definition was first created/generated.

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

### 1.4.6.2. idx_leveldefinition_difficultyScore
#### 1.4.6.2.2. Columns

- difficultyScore

#### 1.4.6.2.3. Type
BTree


### 1.4.7. Caching Strategy

- **Type:** Distributed Cache (Firebase Cache) / Client-side Asset Loading
- **Key:** levelDefinitionId or 'pack:level'
- **Ttl:** Long / Permanent (as definitions are static)
- **Notes:** Level definitions are loaded via client asset bundles or directly from a Firestore collection. Client caching (e.g., via Addressables) is crucial for fast level load times.

## 1.5. PlayerLevelProgress
Tracks a player's progress and best performance for each level. Implemented as a subcollection under the Player document or linked documents in Firestore. REQ-SRP-006, REQ-SRP-007, FR-S-006, FR-S-007, FR-L-005.

### 1.5.3. Attributes

### 1.5.3.1. playerLevelProgressId
Unique identifier for the progress document.

#### 1.5.3.1.2. Type
Guid

#### 1.5.3.1.3. Is Required
True

#### 1.5.3.1.4. Is Primary Key
True

#### 1.5.3.1.5. Is Unique
True

### 1.5.3.2. playerId
Foreign key linking to the Player entity. Used as part of the document path or field for lookup.

#### 1.5.3.2.2. Type
Guid

#### 1.5.3.2.3. Is Required
True

#### 1.5.3.2.4. Is Foreign Key
True

### 1.5.3.3. levelDefinitionId
Foreign key linking to the LevelDefinition entity.

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
False

### 1.5.3.5. isCompleted
#### 1.5.3.5.2. Type
BOOLEAN

#### 1.5.3.5.3. Is Required
True

#### 1.5.3.5.4. Default Value
False

### 1.5.3.6. bestScore
Best score achieved for this level.

#### 1.5.3.6.2. Type
INT

#### 1.5.3.6.3. Is Required
False

### 1.5.3.7. bestMoves
Fewest moves taken to complete this level.

#### 1.5.3.7.2. Type
INT

#### 1.5.3.7.3. Is Required
False

### 1.5.3.8. bestTimeSeconds
Fastest time to complete this level (if timed).

#### 1.5.3.8.2. Type
INT

#### 1.5.3.8.3. Is Required
False

### 1.5.3.9. starsAwarded
Number of stars earned for the best completion of this level. REQ-SRP-006, FR-S-005.

#### 1.5.3.9.2. Type
INT

#### 1.5.3.9.3. Is Required
True

#### 1.5.3.9.4. Default Value
0

#### 1.5.3.9.5. Constraints

- RANGE(0, 3)

### 1.5.3.10. firstCompletedAt
Timestamp of the first successful completion.

#### 1.5.3.10.2. Type
DateTime

#### 1.5.3.10.3. Is Required
False

### 1.5.3.11. lastPlayedAt
Timestamp of the most recent play session for this level.

#### 1.5.3.11.2. Type
DateTime

#### 1.5.3.11.3. Is Required
False

### 1.5.3.12. createdAt
Timestamp when the progress record was first created.

#### 1.5.3.12.2. Type
DateTime

#### 1.5.3.12.3. Is Required
True

### 1.5.3.13. updatedAt
Timestamp of the last update to the progress record.

#### 1.5.3.13.2. Type
DateTime

#### 1.5.3.13.3. Is Required
True


### 1.5.4. Primary Keys

- playerLevelProgressId

### 1.5.5. Unique Constraints


### 1.5.6. Indexes

### 1.5.6.1. idx_playerlevelprogress_player_level
#### 1.5.6.1.2. Columns

- playerId
- levelDefinitionId

#### 1.5.6.1.3. Type
BTree

### 1.5.6.2. idx_playerlevelprogress_isCompleted
#### 1.5.6.2.2. Columns

- isCompleted

#### 1.5.6.2.3. Type
BTree

### 1.5.6.3. idx_playerlevelprogress_starsAwarded
#### 1.5.6.3.2. Columns

- starsAwarded

#### 1.5.6.3.3. Type
BTree


### 1.5.7. Caching Strategy

- **Type:** Client-side
- **Key:** player progress data
- **Ttl:** Session lifetime / Periodic sync
- **Notes:** Player progress for unlocked levels is loaded and updated in memory. Changes are synced to cloud save periodically or on explicit triggers.

## 1.6. Leaderboard
Defines a specific leaderboard for tracking player scores. Implemented as a Firestore Collection of static leaderboard definition documents. REQ-SRP-008, FR-ONL-001.

### 1.6.3. Attributes

### 1.6.3.1. leaderboardId
Unique identifier for the leaderboard definition document.

#### 1.6.3.1.2. Type
Guid

#### 1.6.3.1.3. Is Required
True

#### 1.6.3.1.4. Is Primary Key
True

#### 1.6.3.1.5. Is Unique
True

### 1.6.3.2. name
Unique name for the leaderboard (e.g., 'Global_Stars', 'Level_5_Time_Attack'). Used as the document ID in Firestore.

#### 1.6.3.2.2. Type
VARCHAR

#### 1.6.3.2.3. Is Required
True

#### 1.6.3.2.4. Is Unique
True

#### 1.6.3.2.5. Size
100

### 1.6.3.3. scoreType
The metric being ranked ('SCORE', 'MOVES', 'TIME'). FR-ONL-001.

#### 1.6.3.3.2. Type
VARCHAR

#### 1.6.3.3.3. Is Required
True

#### 1.6.3.3.4. Size
20

### 1.6.3.4. sortOrder
Sort order for scores ('ASC' for time/moves, 'DESC' for score). FR-ONL-001.

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
True

### 1.6.3.7. createdAt
Timestamp when the leaderboard definition was created.

#### 1.6.3.7.2. Type
DateTime

#### 1.6.3.7.3. Is Required
True

### 1.6.3.8. updatedAt
Timestamp of the last update to the leaderboard definition.

#### 1.6.3.8.2. Type
DateTime

#### 1.6.3.8.3. Is Required
True


### 1.6.4. Primary Keys

- name

### 1.6.5. Unique Constraints


### 1.6.6. Indexes


## 1.7. LeaderboardEntry
Represents a single player's best entry on a specific leaderboard. Implemented as a Firestore Collection ('leaderboardEntries'). Each entry is linked to a leaderboard and a player. REQ-SRP-008, REQ-SRP-009, FR-ONL-001, NFR-SEC-003.

### 1.7.3. Attributes

### 1.7.3.1. leaderboardEntryId
Unique identifier for the leaderboard entry document.

#### 1.7.3.1.2. Type
Guid

#### 1.7.3.1.3. Is Required
True

#### 1.7.3.1.4. Is Primary Key
True

#### 1.7.3.1.5. Is Unique
True

### 1.7.3.2. leaderboardId
Foreign key linking to the Leaderboard entity (via Leaderboard.leaderboardId).

#### 1.7.3.2.2. Type
Guid

#### 1.7.3.2.3. Is Required
True

#### 1.7.3.2.4. Is Foreign Key
True

### 1.7.3.3. playerId
Foreign key linking to the Player entity (via Player.playerId).

#### 1.7.3.3.2. Type
Guid

#### 1.7.3.3.3. Is Required
True

#### 1.7.3.3.4. Is Foreign Key
True

### 1.7.3.4. firebaseUid
Firebase UID of the player. Used for querying and security rules. Redundant if playerId is sufficient for lookups.

#### 1.7.3.4.2. Type
VARCHAR

#### 1.7.3.4.3. Is Required
True

#### 1.7.3.4.4. Size
128

### 1.7.3.5. playerName
Denormalized player name to avoid joins on leaderboard fetch. Must be kept in sync (e.g., via Cloud Function triggered by Player name changes). FR-ONL-001.

#### 1.7.3.5.2. Type
VARCHAR

#### 1.7.3.5.3. Is Required
False

#### 1.7.3.5.4. Size
100

### 1.7.3.6. scoreValue
The player's score. Stored as BIGINT to accommodate time in milliseconds or large scores.

#### 1.7.3.6.2. Type
BIGINT

#### 1.7.3.6.3. Is Required
True

### 1.7.3.7. levelDefinitionId
Optional link to the specific level if the leaderboard is level-specific. Improves querying for per-level leaderboards.

#### 1.7.3.7.2. Type
Guid

#### 1.7.3.7.3. Is Required
False

#### 1.7.3.7.4. Is Foreign Key
True

### 1.7.3.8. componentsOfScore
Optional JSON payload storing components contributing to the score (e.g., moves, time, bonuses) for validation and display.

#### 1.7.3.8.2. Type
JSON

#### 1.7.3.8.3. Is Required
False

### 1.7.3.9. rank
The calculated rank of the player. Updated periodically by a background job or Cloud Function triggered by new submissions. FR-ONL-001.

#### 1.7.3.9.2. Type
INT

#### 1.7.3.9.3. Is Required
False

### 1.7.3.10. isDeleted
Flag for soft-deleting an entry, e.g., for cheating. NFR-SEC-003.

#### 1.7.3.10.2. Type
BOOLEAN

#### 1.7.3.10.3. Is Required
True

#### 1.7.3.10.4. Default Value
False

### 1.7.3.11. submittedAt
Server timestamp of when the score was submitted. Used for Tie-breaking. FR-ONL-001.

#### 1.7.3.11.2. Type
DateTime

#### 1.7.3.11.3. Is Required
True

### 1.7.3.12. createdAt
Timestamp when the entry document was created.

#### 1.7.3.12.2. Type
DateTime

#### 1.7.3.12.3. Is Required
True

### 1.7.3.13. updatedAt
Timestamp of the last update to the entry (e.g., rank update, soft delete).

#### 1.7.3.13.2. Type
DateTime

#### 1.7.3.13.3. Is Required
True


### 1.7.4. Primary Keys

- leaderboardEntryId

### 1.7.5. Unique Constraints


### 1.7.6. Indexes

### 1.7.6.1. uq_leaderboardentry_leaderboard_player
#### 1.7.6.1.2. Columns

- leaderboardId
- playerId

#### 1.7.6.1.3. Type
BTree

### 1.7.6.2. idx_leaderboardentry_leaderboard_firebaseUid
#### 1.7.6.2.2. Columns

- leaderboardId
- firebaseUid

#### 1.7.6.2.3. Type
BTree

#### 1.7.6.2.4. Notes
Useful for querying a player's rank on a specific leaderboard using their Firebase UID.

### 1.7.6.3. idx_leaderboardentry_ranking
#### 1.7.6.3.2. Columns

- leaderboardId
- scoreValue DESC
- submittedAt ASC

#### 1.7.6.3.3. Type
BTree

#### 1.7.6.3.4. Notes
Composite index for efficient ranking queries by score, then submission time.


### 1.7.7. Caching Strategy

- **Type:** Distributed Cache (Firebase Cache) / Server-side
- **Key:** leaderboardId
- **Ttl:** Short (1-5 minutes for top N entries)
- **Notes:** Caching top N entries in Cloud Functions or client-side cache can reduce Firestore reads for frequently viewed leaderboards. Queries for a specific player's rank will hit Firestore directly.

## 1.8. Achievement
Defines an achievement that players can unlock. Implemented as a Firestore Collection of static achievement definition documents. REQ-SRP-010, FR-ONL-002.

### 1.8.3. Attributes

### 1.8.3.1. achievementId
Unique identifier for the achievement definition document.

#### 1.8.3.1.2. Type
Guid

#### 1.8.3.1.3. Is Required
True

#### 1.8.3.1.4. Is Primary Key
True

#### 1.8.3.1.5. Is Unique
True

### 1.8.3.2. name
Unique name for the achievement. Used as the document ID in Firestore.

#### 1.8.3.2.2. Type
VARCHAR

#### 1.8.3.2.3. Is Required
True

#### 1.8.3.2.4. Is Unique
True

#### 1.8.3.2.5. Size
100

### 1.8.3.3. description
#### 1.8.3.3.2. Type
TEXT

#### 1.8.3.3.3. Is Required
True

### 1.8.3.4. iconUrl
URL or path to the achievement icon asset.

#### 1.8.3.4.2. Type
VARCHAR

#### 1.8.3.4.3. Is Required
False

#### 1.8.3.4.4. Size
255

### 1.8.3.5. triggerCondition
JSON object defining the rules to unlock the achievement (e.g., '{\"type\": \"STARS_COLLECTED\", \"value\": 100, \"levelPack\": 2}'). Processed by client-side logic, potentially validated server-side for critical achievements. FR-ONL-002.

#### 1.8.3.5.2. Type
JSON

#### 1.8.3.5.3. Is Required
True

### 1.8.3.6. isHidden
Whether the achievement is hidden until unlocked.

#### 1.8.3.6.2. Type
BOOLEANE

#### 1.8.3.6.3. Is Required
True

#### 1.8.3.6.4. Default Value
False

### 1.8.3.7. isActive
#### 1.8.3.7.2. Type
BOOLEAN

#### 1.8.3.7.3. Is Required
True

#### 1.8.3.7.4. Default Value
True

### 1.8.3.8. createdAt
Timestamp when the achievement definition was created.

#### 1.8.3.8.2. Type
DateTime

#### 1.8.3.8.3. Is Required
True

### 1.8.3.9. updatedAt
Timestamp of the last update to the achievement definition.

#### 1.8.3.9.2. Type
DateTime

#### 1.8.3.9.3. Is Required
True


### 1.8.4. Primary Keys

- name

### 1.8.5. Unique Constraints


### 1.8.6. Indexes


## 1.9. PlayerAchievement
A link table tracking player's progress and unlock status for achievements. Implemented as a subcollection under the Player document or linked documents in Firestore. REQ-SRP-010, FR-ONL-002.

### 1.9.3. Attributes

### 1.9.3.1. playerAchievementId
Unique identifier for the player achievement document.

#### 1.9.3.1.2. Type
Guid

#### 1.9.3.1.3. Is Required
True

#### 1.9.3.1.4. Is Primary Key
True

#### 1.9.3.1.5. Is Unique
True

### 1.9.3.2. playerId
Foreign key linking to the Player entity. Used as part of the document path or field for lookup.

#### 1.9.3.2.2. Type
Guid

#### 1.9.3.2.3. Is Required
True

#### 1.9.3.2.4. Is Foreign Key
True

### 1.9.3.3. achievementId
Foreign key linking to the Achievement entity (via Achievement.achievementId).

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
False

### 1.9.3.6. unlockedAt
Server timestamp of when the achievement was unlocked.

#### 1.9.3.6.2. Type
DateTime

#### 1.9.3.6.3. Is Required
False

### 1.9.3.7. createdAt
Timestamp when the player achievement record was first created.

#### 1.9.3.7.2. Type
DateTime

#### 1.9.3.7.3. Is Required
True

### 1.9.3.8. updatedAt
Timestamp of the last update to the player achievement record (e.g., progress update, unlock).

#### 1.9.3.8.2. Type
DateTime

#### 1.9.3.8.3. Is Required
True


### 1.9.4. Primary Keys

- playerAchievementId

### 1.9.5. Unique Constraints


### 1.9.6. Indexes

### 1.9.6.1. uq_playerachievement_player_achievement
#### 1.9.6.1.2. Columns

- playerId
- achievementId

#### 1.9.6.1.3. Type
BTree

### 1.9.6.2. idx_playerachievement_isUnlocked
#### 1.9.6.2.2. Columns

- isUnlocked

#### 1.9.6.2.3. Type
BTree


### 1.9.7. Caching Strategy

- **Type:** Client-side
- **Key:** player achievement data
- **Ttl:** Session lifetime / Periodic sync
- **Notes:** Player achievement status is loaded and updated in memory. Changes are synced to cloud save periodically or on explicit triggers.

## 1.10. RemoteConfig
Stores key-value pairs for remote configuration of game parameters. Implemented as a Firestore Collection of static configuration documents. Enables dynamic balancing. REQ-8-006, REQ-PCGDS-007, NFR-M-002.

### 1.10.3. Attributes

### 1.10.3.1. remoteConfigId
Unique identifier for the config document.

#### 1.10.3.1.2. Type
Guid

#### 1.10.3.1.3. Is Required
True

#### 1.10.3.1.4. Is Primary Key
True

#### 1.10.3.1.5. Is Unique
True

### 1.10.3.2. configKey
The unique key for the configuration parameter. Used as the document ID in Firestore.

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
True

### 1.10.3.6. createdAt
Timestamp when the config entry was created.

#### 1.10.3.6.2. Type
DateTime

#### 1.10.3.6.3. Is Required
True

### 1.10.3.7. updatedAt
Timestamp of the last update to the config entry.

#### 1.10.3.7.2. Type
DateTime

#### 1.10.3.7.3. Is Required
True


### 1.10.4. Primary Keys

- configKey

### 1.10.5. Unique Constraints


### 1.10.6. Indexes


### 1.10.7. Caching Strategy

- **Type:** Client-side & CDN
- **Key:** configKey or version identifier
- **Ttl:** Managed via Firebase Remote Config fetch policies
- **Notes:** Firebase Remote Config handles caching, versioning, and conditional fetching efficiently.



---

# 2. Diagrams

- **Diagram_Title:** Logical Game Data ER Diagram  
**Diagram_Area:** Game Core Data  
**Explanation:** Entity-Relationship Diagram for the Logical Game Data database (Firebase Firestore). It illustrates the main entities such as Player, Game Settings, Level Definitions, Player Progress, Leaderboards, Achievements, and Remote Config. Relationships between entities are shown, indicating how data is linked across different collections or subcollections, representing concepts like a player having settings, progress on levels, leaderboard entries, and achievements. RemoteConfig is shown as a separate entity.  
**Mermaid_Text:** erDiagram
    Player {
        Guid playerId PK "Unique identifier for the player"
        VARCHAR firebaseUid "Firebase Authentication User ID"
        VARCHAR ageGateStatus "Status from age gate"
        INT totalStars "Aggregate total stars"
        INT highestLevelPackUnlocked "Highest pack unlocked"
        INT highestLevelUnlockedInPack "Highest level in pack"
        BOOLEAN isDeleted "Soft delete flag"
        DateTime lastLoginAt "Last login timestamp"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    GameSettings {
        Guid gameSettingsId PK "Unique identifier for settings"
        Guid playerId FK "Foreign key to Player"
        DECIMAL bgmVolume "Background music volume"
        DECIMAL sfxVolume "Sound effects volume"
        VARCHAR colorblindMode "Colorblind mode setting"
        BOOLEAN isReducedMotionEnabled "Reduce motion flag"
        BOOLEAN isHapticsEnabled "Haptics flag"
        VARCHAR languageCode "Language setting"
        BOOLEAN isCloudSaveEnabled "Cloud save flag"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    UserConsent {
        Guid userConsentId PK "Unique identifier for consent"
        Guid playerId FK "Foreign key to Player"
        VARCHAR consentType "Type of consent"
        BOOLEAN isGiven "Consent status"
        VARCHAR version "Policy version"
        DateTime grantedAt "Consent granted timestamp"
        DateTime createdAt "Creation timestamp"
    }

    LevelDefinition {
        Guid levelDefinitionId PK "Unique identifier for level definition"
        INT levelPack "Level pack number"
        INT levelNumber "Level number within pack"
        VARCHAR puzzleType "Type of puzzle"
        BIGINT generationSeed "Generation seed"
        JSON gridConfiguration "Initial grid state"
        JSON goalConfiguration "Target pattern/rules"
        INT parMoves "Target move count"
        JSON solutionPath "Optimal solution path"
        DECIMAL difficultyScore "Calculated difficulty"
        DateTime createdAt "Creation timestamp"
    }

    PlayerLevelProgress {
        Guid playerLevelProgressId PK "Unique identifier for progress"
        Guid playerId FK "Foreign key to Player"
        Guid levelDefinitionId FK "Foreign key to LevelDefinition"
        BOOLEAN isUnlocked "Level unlocked flag"
        BOOLEAN isCompleted "Level completed flag"
        INT bestScore "Best score achieved"
        INT bestMoves "Fewest moves taken"
        INT bestTimeSeconds "Fastest time"
        INT starsAwarded "Stars earned"
        DateTime firstCompletedAt "First completion timestamp"
        DateTime lastPlayedAt "Last played timestamp"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    Leaderboard {
        Guid leaderboardId PK "Unique identifier for leaderboard definition"
        VARCHAR name "Unique name for leaderboard"
        VARCHAR scoreType "Metric being ranked"
        VARCHAR sortOrder "Sort order"
        VARCHAR resetFrequency "Reset frequency"
        BOOLEAN isActive "Leaderboard active flag"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    LeaderboardEntry {
        Guid leaderboardEntryId PK "Unique identifier for entry"
        Guid leaderboardId FK "Foreign key to Leaderboard"
        Guid playerId FK "Foreign key to Player"
        VARCHAR firebaseUid "Firebase UID of player"
        VARCHAR playerName "Player name"
        BIGINT scoreValue "Score value"
        Guid levelDefinitionId FK "Optional level link"
        JSON componentsOfScore "Score components"
        INT rank "Calculated rank"
        BOOLEAN isDeleted "Soft delete flag"
        DateTime submittedAt "Submission timestamp"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    Achievement {
        Guid achievementId PK "Unique identifier for achievement definition"
        VARCHAR name "Unique name for achievement"
        TEXT description "Achievement description"
        VARCHAR iconUrl "Icon asset path"
        JSON triggerCondition "Unlock rules"
        BOOLEAN isHidden "Hidden until unlocked"
        BOOLEAN isActive "Achievement active flag"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    PlayerAchievement {
        Guid playerAchievementId PK "Unique identifier for player achievement"
        Guid playerId FK "Foreign key to Player"
        Guid achievementId FK "Foreign key to Achievement"
        INT progress "Current progress"
        BOOLEAN isUnlocked "Achievement unlocked flag"
        DateTime unlockedAt "Unlock timestamp"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    RemoteConfig {
        Guid remoteConfigId PK "Unique identifier for config"
        VARCHAR configKey "Unique key for parameter"
        JSON configValue "Parameter value as JSON"
        TEXT description "Config description"
        BOOLEAN isActive "Config active flag"
        DateTime createdAt "Creation timestamp"
        DateTime updatedAt "Last update timestamp"
    }

    Player ||--|| GameSettings : "has"
    Player ||--o{ UserConsent : "has"
    Player ||--o{ PlayerLevelProgress : "tracks progress for"
    LevelDefinition ||--o{ PlayerLevelProgress : "progress tracked by"
    Player ||--o{ LeaderboardEntry : "has"
    Leaderboard ||--o{ LeaderboardEntry : "includes"
    Player ||--o{ PlayerAchievement : "tracks"
    Achievement ||--o{ PlayerAchievement : "is tracked by"
  


---

