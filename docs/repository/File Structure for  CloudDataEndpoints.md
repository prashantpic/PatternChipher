# Specification

# 1. Files

- **Path:** firebase/firestore/data/firestore.rules  
**Description:** Defines the security rules for the Cloud Firestore database. It ensures that authenticated users can only access and modify their own data, and enforces data schema validation on write operations. This is the primary mechanism for implementing NFR-SEC-004 and parts of DM-002.  
**Template:** Firestore Rules Template  
**Dependency Level:** 1  
**Name:** firestore  
**Type:** SecurityRules  
**Relative Path:** firestore.rules  
**Repository Id:** REPO-PATT-007  
**Pattern Ids:**
    
    - RoleBasedAccessControl
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - User Data Isolation
    - Schema Validation
    
**Requirement Ids:**
    
    - FR-ONL-003
    - DM-002
    - NFR-SEC-004
    
**Purpose:** To secure the Firestore database, restricting read and write access to user-specific documents based on their authentication UID and validating the structure of incoming data.  
**Logic Description:** The rules will be structured to match the 'userProfiles' collection. A 'match /userProfiles/{userId}' block will be the core of the rules. The 'allow read, update, delete' permissions will be granted only if 'request.auth.uid == userId', ensuring a user can only affect their own document. The 'allow create' permission will be granted if 'request.auth.uid == userId' and the incoming data conforms to the UserProfile schema defined in DM-002. Write validation functions will be used to check data types, presence of required fields like 'cloud_save_data_object_version' and 'timestamp_of_last_cloud_sync' (ensuring it is a server timestamp). Global read/write/list access to the 'userProfiles' collection will be denied.  
**Documentation:**
    
    - **Summary:** This file contains the security logic for the Cloud Firestore instance. It ensures data isolation between users and validates the integrity of data written to the database, which is critical for the cloud save feature.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Security
    
- **Path:** firebase/firestore/data/firestore.indexes.json  
**Description:** Configuration file for defining composite indexes in Cloud Firestore. While the primary access pattern for cloud save is direct document retrieval, this file establishes the pattern for any future administrative or feature-based queries that might require indexing across multiple fields for performance.  
**Template:** JSON Configuration Template  
**Dependency Level:** 0  
**Name:** firestore.indexes  
**Type:** DatabaseIndexConfig  
**Relative Path:** firestore.indexes.json  
**Repository Id:** REPO-PATT-007  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Database Indexing
    
**Requirement Ids:**
    
    - DM-002
    
**Purpose:** To specify composite indexes required for efficient Firestore queries, preventing performance degradation on complex data retrieval operations.  
**Logic Description:** This JSON file defines an array of index objects. Each object specifies a 'collectionGroup' (e.g., 'userProfiles'), a 'queryScope', and a list of 'fields' to be indexed, including their sort order ('ASCENDING' or 'DESCENDING'). For now, this file will contain a placeholder structure, as the initial requirements do not necessitate complex queries. It will be updated if features requiring such indexes are added. For example, an index could be defined on 'timestamp_of_last_cloud_sync' if there was a need to query for recently active users.  
**Documentation:**
    
    - **Summary:** Defines custom composite indexes for Cloud Firestore. This configuration is necessary for queries that filter or order by multiple fields, ensuring they execute efficiently at scale.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Database
    
- **Path:** firebase/firestore/data/schemas/UserProfile.md  
**Description:** Markdown documentation defining the data schema for a user's document within the 'userProfiles' collection in Firestore. This serves as the single source of truth for the structure of user data stored in the cloud, directly addressing the DM-002 requirement.  
**Template:** Markdown Documentation Template  
**Dependency Level:** 0  
**Name:** UserProfile  
**Type:** DataSchemaDocumentation  
**Relative Path:** schemas/UserProfile.md  
**Repository Id:** REPO-PATT-007  
**Pattern Ids:**
    
    - DataModel
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Schema Definition
    
**Requirement Ids:**
    
    - DM-002
    - FR-ONL-003
    - DM-006
    
**Purpose:** To formally document the structure, fields, data types, and constraints of the UserProfile document stored in Firestore, ensuring clarity for client and backend development.  
**Logic Description:** The document will be structured with tables to define each field. Key fields to be documented include: Document ID (Firebase UID from Auth), 'cloud_save_data_object' (Map/Object, detailing the nested structure mirroring the local save from DM-001), 'cloud_save_data_object_version' (String, for migration), 'timestamp_of_last_cloud_sync' (ServerTimestamp, for conflict resolution), and 'user_profile_schema_version' (String). Each field definition will include its name, data type, a description of its purpose, and whether it is required.  
**Documentation:**
    
    - **Summary:** This document provides a detailed specification for the UserProfile data model as it is stored in Cloud Firestore. It is a critical reference for implementing client-side serialization and backend validation logic.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Documentation
    
- **Path:** firebase/firestore/data/backups/backup_policy.md  
**Description:** Documents the backup and recovery policy for the Cloud Firestore database, addressing NFR-BS-004. It outlines the strategy, objectives (RPO/RTO), and procedures for ensuring data durability and disaster recovery.  
**Template:** Markdown Documentation Template  
**Dependency Level:** 0  
**Name:** backup_policy  
**Type:** PolicyDocumentation  
**Relative Path:** backups/backup_policy.md  
**Repository Id:** REPO-PATT-007  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Backup and Recovery Policy
    
**Requirement Ids:**
    
    - NFR-BS-004
    
**Purpose:** To define and document the backup and recovery strategy for all user data stored in Cloud Firestore, ensuring business continuity and data durability.  
**Logic Description:** This document will specify the use of Firebase's built-in Point-in-Time Recovery (PITR) and/or Managed Export services. It will formally state the Recovery Point Objective (RPO) as 24 hours and the Recovery Time Objective (RTO) as 4 hours. It will also outline the high-level steps required to initiate a data restore from a backup and mandate periodic (e.g., semi-annual) recovery drills to validate the procedure and ensure RTO/RPO targets are achievable.  
**Documentation:**
    
    - **Summary:** Outlines the official policy and procedures for backing up and recovering the production Firestore database. This document is essential for operational readiness and disaster recovery planning.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Documentation
    
- **Path:** firebase/firestore/data/strategies/ConflictResolution.md  
**Description:** Details the conflict resolution strategy for the cloud save feature, as required by DM-006. This document is critical for both client and backend developers to ensure consistent handling of data synchronization conflicts.  
**Template:** Markdown Documentation Template  
**Dependency Level:** 0  
**Name:** ConflictResolution  
**Type:** StrategyDocumentation  
**Relative Path:** strategies/ConflictResolution.md  
**Repository Id:** REPO-PATT-007  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Cloud Save Conflict Resolution
    
**Requirement Ids:**
    
    - DM-006
    - FR-ONL-003
    
**Purpose:** To formally document the logic for resolving conflicts that arise when synchronizing player data across multiple devices, ensuring a predictable and user-friendly experience.  
**Logic Description:** The primary strategy will be 'Last Write Wins', where the document with the most recent server-side 'timestamp_of_last_cloud_sync' is considered authoritative. The document will detail the client-side flow: before writing, the client reads the cloud timestamp; if the local save is older, it must first pull the newer cloud data. For initial sync on a new device, the client will check for existing cloud data; if found, it will prompt the user to choose between the local and cloud versions, displaying key progress metrics (e.g., total stars, last played date) to inform the decision. Data merging will be explicitly disallowed to maintain simplicity.  
**Documentation:**
    
    - **Summary:** Specifies the business rules and technical flow for handling data conflicts during cloud save synchronization. This ensures consistent implementation on the client and prevents data loss.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Documentation
    
- **Path:** firebase/firestore/data/strategies/SyncTriggers.md  
**Description:** Documents the client-side events that should trigger a cloud save synchronization, fulfilling requirement DM-005. This document serves as a specification for the client-side infrastructure layer to implement.  
**Template:** Markdown Documentation Template  
**Dependency Level:** 0  
**Name:** SyncTriggers  
**Type:** StrategyDocumentation  
**Relative Path:** strategies/SyncTriggers.md  
**Repository Id:** REPO-PATT-007  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Cloud Save Sync Strategy
    
**Requirement Ids:**
    
    - DM-005
    - FR-ONL-003
    
**Purpose:** To define the specific application events that should initiate a cloud save data synchronization from the client application to the Firebase backend.  
**Logic Description:** This document will list all events that trigger a sync. Automatic triggers include: application pause/quit (with a debounce mechanism), successful completion of a level, and unlocking a major new feature or level pack. A manual trigger must be available in the game's settings menu, allowing the user to force a sync at any time. The document will also specify that a sync should only proceed if the local data has changed since the last successful sync, to conserve network and battery resources.  
**Documentation:**
    
    - **Summary:** Defines the specific triggers for initiating the cloud save process. This ensures that player data is saved in a timely and efficient manner, balancing data freshness with performance considerations.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Documentation
    


---

# 2. Configuration

- **Feature Toggles:**
  
  
- **Database Configs:**
  
  - firestore.project.id
  - firestore.database.name
  


---

