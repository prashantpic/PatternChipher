# Specification

# 1. Database Design

## 1.1. AnalyticsEvent
A log of significant, anonymized player events for analysis and game balancing. Collected via Firebase Analytics SDK and typically exported to BigQuery for querying and analysis. High volume, append-only data. REQ-8-001, REQ-8-002, FR-AT-001.

### 1.1.3. Attributes

### 1.1.3.1. analyticsEventId
Unique identifier for the event record (primary key for internal processing if needed, but BigQuery typically doesn't rely on this).

#### 1.1.3.1.2. Type
Guid

#### 1.1.3.1.3. Is Required
True

#### 1.1.3.1.4. Is Primary Key
True

#### 1.1.3.1.5. Is Unique
True

### 1.1.3.2. playerId
Pseudonymized player identifier (e.g., the local playerId if no cloud account, or the firebaseUid if logged in and consented). Must not be PII unless explicitly consented for specific purposes. Used for correlating events per player.

#### 1.1.3.2.2. Type
Guid

#### 1.1.3.2.3. Is Required
True

#### 1.1.3.2.4. Is Foreign Key
True

### 1.1.3.3. sessionId
Identifier for the game session in which the event occurred. Useful for grouping events within a single play session.

#### 1.1.3.3.2. Type
Guid

#### 1.1.3.3.3. Is Required
True

### 1.1.3.4. eventName
Name of the event (e.g., 'level_start', 'level_complete', 'hint_used').

#### 1.1.3.4.2. Type
VARCHAR

#### 1.1.3.4.3. Is Required
True

#### 1.1.3.4.4. Size
100

### 1.1.3.5. eventTimestamp
Timestamp when the event occurred (client or server timestamp, consistency is key). In BigQuery, this is often a partitioning column.

#### 1.1.3.5.2. Type
DateTime

#### 1.1.3.5.3. Is Required
True

### 1.1.3.6. eventPayload
JSON object containing event-specific data (e.g., level details, score, moves, time, hint type, tile states). Schema of payload varies by eventName.

#### 1.1.3.6.2. Type
JSON

#### 1.1.3.6.3. Is Required
False

### 1.1.3.7. levelId
Promoted from payload for common queries: The level associated with the event (if applicable).

#### 1.1.3.7.2. Type
Guid

#### 1.1.3.7.3. Is Required
False

#### 1.1.3.7.4. Is Foreign Key
True

### 1.1.3.8. moves
Promoted from payload for common queries: The move count at the time of the event (if applicable).

#### 1.1.3.8.2. Type
INT

#### 1.1.3.8.3. Is Required
False

### 1.1.3.9. timeInSeconds
Promoted from payload for common queries: The time elapsed in the level at the time of the event (if applicable).

#### 1.1.3.9.2. Type
INT

#### 1.1.3.9.3. Is Required
False

### 1.1.3.10. appVersion
Client application version at the time of the event. Crucial for analyzing data across updates.

#### 1.1.3.10.2. Type
VARCHAR

#### 1.1.3.10.3. Is Required
False

#### 1.1.3.10.4. Size
20

### 1.1.3.11. platform
Client platform (e.g., 'iOS', 'Android').

#### 1.1.3.11.2. Type
VARCHAR

#### 1.1.3.11.3. Is Required
False

#### 1.1.3.11.4. Size
20

### 1.1.3.12. deviceModel
Client device model (anonymized or generalized if necessary). NFR-AT-001.

#### 1.1.3.12.2. Type
VARCHAR

#### 1.1.3.12.3. Is Required
False

#### 1.1.3.12.4. Size
100

### 1.1.3.13. osVersion
Client OS version. NFR-AT-001.

#### 1.1.3.13.2. Type
VARCHAR

#### 1.1.3.13.3. Is Required
False

#### 1.1.3.13.4. Size
50

### 1.1.3.14. countryCode
Geo-location information (derived from IP, handled by analytics platform). Must be anonymized/aggregated for reporting. NFR-LC-002.

#### 1.1.3.14.2. Type
VARCHAR

#### 1.1.3.14.3. Is Required
False

#### 1.1.3.14.4. Size
10


### 1.1.4. Primary Keys

- analyticsEventId

### 1.1.5. Unique Constraints


### 1.1.6. Indexes

### 1.1.6.1. idx_analyticsevent_name_timestamp
#### 1.1.6.1.2. Columns

- eventName
- eventTimestamp

#### 1.1.6.1.3. Type
BTree

#### 1.1.6.1.4. Notes
Primary index for querying event occurrences over time.

### 1.1.6.2. idx_analyticsevent_player_timestamp
#### 1.1.6.2.2. Columns

- playerId
- eventTimestamp

#### 1.1.6.2.3. Type
BTree

#### 1.1.6.2.4. Notes
Index for tracking a specific player's event history.

### 1.1.6.3. idx_analyticsevent_timestamp
#### 1.1.6.3.2. Columns

- eventTimestamp

#### 1.1.6.3.3. Type
BTree

#### 1.1.6.3.4. Notes
Index for general time-based queries.


### 1.1.7. Partitioning

- **Type:** Range
- **Column:** eventTimestamp
- **Strategy:** Daily or Hourly (in BigQuery)
- **Notes:** Essential for performance and cost management in high-volume analytics platforms like BigQuery. Queries should always filter by time range.

### 1.1.8. Caching Strategy

- **Type:** Platform Managed (BigQuery)
- **Key:** Query results
- **Ttl:** Variable
- **Notes:** Caching is handled by the analytics platform itself (BigQuery).



---

# 2. Diagrams

- **Diagram_Title:** Analytics Event Stream  
**Diagram_Area:** Analytics Logging  
**Explanation:** Represents the core Analytics Event Stream table, capturing various player actions and game state.
It includes fields for tracking player sessions, event details, and context like level, moves, and time.
Foreign key relationships are indicated for Player and Level entities, although these entities are not fully defined in the provided schema.
This structure is optimized for high-volume, append-only data typical of event streams exported to platforms like BigQuery.  
**Mermaid_Text:** erDiagram
    Player {
        Guid playerId PK
    }
    Level {
        Guid levelId PK
    }
    AnalyticsEvent {
        Guid analyticsEventId PK
        Guid playerId FK
        Guid sessionId
        VARCHAR eventName
        DateTime eventTimestamp
        JSON eventPayload
        Guid levelId FK
        INT moves
        INT timeInSeconds
        VARCHAR appVersion
        VARCHAR platform
        VARCHAR deviceModel
        VARCHAR osVersion
        VARCHAR countryCode
    }

    Player ||--o{ AnalyticsEvent : generates
    Level ||--o{ AnalyticsEvent : pertains_to  


---

