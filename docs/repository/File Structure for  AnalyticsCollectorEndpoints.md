# Specification

# 1. Files

- **Path:** config/analytics_configuration.yaml  
**Description:** A YAML configuration file defining the high-level policies and settings for the Firebase Analytics service. This includes data retention schedules, BigQuery export configuration, and default consent management states based on regional regulations.  
**Template:** YAML Configuration Template  
**Dependency Level:** 0  
**Name:** analytics_configuration  
**Type:** Configuration  
**Relative Path:** config/analytics_configuration.yaml  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - ConfigurationManagement
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Analytics Service Configuration
    - Data Retention Policy Definition
    - Consent Management Defaults
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-AT-002
    - FR-AT-003
    
**Purpose:** To serve as the single source of truth for how the Firebase Analytics project should be configured. This file guides the operational setup in the Firebase console.  
**Logic Description:** This is a declarative configuration file, not executable code. It will contain key-value pairs for settings like data_retention_months: 14, bigquery_export_enabled: true, bigquery_dataset_name: 'patterncipher_analytics_v1', and a section for default_consent_settings mapping regions (e.g., 'GDPR_REGION') to default states (e.g., 'opt-in').  
**Documentation:**
    
    - **Summary:** Defines the operational parameters for the Firebase Analytics service. It ensures consistent setup across different environments and provides a clear reference for compliance and data management policies.
    
**Namespace:** PatternCipher.Services.Analytics.Config  
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** schemas/custom_events.json  
**Description:** A structured JSON file that formally defines all custom analytics events and their parameters. This schema acts as the strict contract between the game client, which sends the events, and the data analysis team, which consumes the data from BigQuery.  
**Template:** JSON Schema Template  
**Dependency Level:** 1  
**Name:** custom_events  
**Type:** SchemaDefinition  
**Relative Path:** schemas/custom_events.json  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - DataContract
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Gameplay Event Tracking Schema
    - Player Progression Tracking Schema
    - UI Interaction Tracking Schema
    
**Requirement Ids:**
    
    - FR-AT-001
    - FR-B-006
    
**Purpose:** To provide a single, authoritative definition for all analytics events, ensuring data consistency and quality. This prevents data drift and misinterpretation.  
**Logic Description:** The file will contain a JSON array of event definitions. Each event will have a unique 'eventName' (e.g., 'level_complete') and a 'parameters' array. Each parameter object will specify its 'paramName' (e.g., 'level_id', 'moves_taken', 'score'), 'dataType' (e.g., 'string', 'long', 'double'), and a 'description'. This covers all events mentioned in FR-AT-001, such as level starts, completions, failures, hint/undo usage, and feature interactions.  
**Documentation:**
    
    - **Summary:** This file is the master schema for all custom events tracked in the game. It defines the structure and data types for the payload of each event, serving as a critical reference for client-side implementation and backend data analysis.
    
**Namespace:** PatternCipher.Services.Analytics.Schemas  
**Metadata:**
    
    - **Category:** Schema
    
- **Path:** schemas/user_properties.json  
**Description:** A JSON schema file that defines all custom user properties to be set in Firebase Analytics. These properties describe segments of the user base rather than single events.  
**Template:** JSON Schema Template  
**Dependency Level:** 1  
**Name:** user_properties  
**Type:** SchemaDefinition  
**Relative Path:** schemas/user_properties.json  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - DataContract
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Player Segmentation Properties
    
**Requirement Ids:**
    
    - FR-AT-001
    
**Purpose:** To define persistent user attributes for audience segmentation and cohort analysis, enabling deeper understanding of player groups over time.  
**Logic Description:** This file will contain a JSON array of user property definitions. Each property will have a 'propertyName' (e.g., 'player_progression_pack', 'total_stars_collected'), 'dataType' (string or number), and a 'description'. These properties are updated at key moments, such as after completing a level pack or changing a significant setting.  
**Documentation:**
    
    - **Summary:** Defines the schema for custom user properties. These properties are attached to a user's analytics profile and are used to create user segments for targeted analysis and understanding long-term engagement patterns.
    
**Namespace:** PatternCipher.Services.Analytics.Schemas  
**Metadata:**
    
    - **Category:** Schema
    
- **Path:** schemas/privacy_compliance_map.json  
**Description:** A JSON file that maps every event parameter and user property to its privacy classification. It dictates the handling rules for data based on user consent status and age, ensuring compliance with regulations like GDPR and COPPA.  
**Template:** JSON Schema Template  
**Dependency Level:** 2  
**Name:** privacy_compliance_map  
**Type:** ComplianceMap  
**Relative Path:** schemas/privacy_compliance_map.json  
**Repository Id:** REPO-PATT-010  
**Pattern Ids:**
    
    - DataContract
    - Policy
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Data Privacy Classification
    - Consent-Based Data Handling Rules
    - COPPA Compliance Logic Definition
    
**Requirement Ids:**
    
    - FR-AT-002
    - FR-AT-003
    
**Purpose:** To provide an auditable and machine-readable definition of the data privacy rules, guiding both client-side implementation and compliance reviews.  
**Logic Description:** This file will be a JSON object mapping parameter names from 'custom_events.json' and 'user_properties.json' to a compliance object. This object will include a 'privacy_category' (e.g., 'GameplayBehavior', 'DeviceTechnical', 'PseudonymizedIdentifier') and a 'compliance_action' field. The action field will specify what to do for unconsented or child users (e.g., 'OMIT', 'TRUNCATE', 'HASH', 'ALLOW'). For example, 'deviceModel' might be 'ALLOW', while a more specific ID might be 'OMIT' for children.  
**Documentation:**
    
    - **Summary:** This file is a critical compliance artifact that explicitly defines the privacy treatment for every piece of analytics data collected. It serves as a blueprint for developers to implement privacy-respecting analytics and for auditors to verify compliance.
    
**Namespace:** PatternCipher.Services.Analytics.Schemas  
**Metadata:**
    
    - **Category:** Compliance
    


---

# 2. Configuration

- **Feature Toggles:**
  
  - enableAnalytics
  - enableBigQueryExport
  - enableUserPropertyCollection
  
- **Database Configs:**
  
  


---

