# Software Design Specification (SDS) for AnalyticsCollectorEndpoints (REPO-PATT-010)

## 1. Introduction

This document provides the detailed software design specification for the `AnalyticsCollectorEndpoints` repository. This repository does not contain executable code but serves as the definitive source of configuration and data contracts for the Firebase Analytics service used by the "Pattern Cipher" game.

Its purpose is to define:
1.  The high-level configuration of the Firebase Analytics project.
2.  The precise schema for all custom events and user properties sent by the game client.
3.  The privacy compliance rules governing the collection of each data point.

This repository implements a configuration-as-code approach for the analytics backend, ensuring consistency, version control, and clear communication between the client development team and the data analysis team.

-   **Architectural Style:** MBaaS (Mobile Backend as a Service), Event-Driven, Publish-Subscribe.
-   **Core Technology:** Firebase Analytics.

## 2. Analytics Service Configuration

This section specifies the content of the `config/analytics_configuration.yaml` file. This file guides the manual or scripted setup of the Firebase Analytics project in the Firebase Console.

**File Path:** `config/analytics_configuration.yaml`

yaml
# Pattern Cipher - Firebase Analytics Configuration
# REQ-8-001, FR-AT-001, FR-AT-002

# Data Retention Settings
# Specifies how long user-level data (including user properties) is retained.
# Complies with data minimization principles.
data_retention:
  user_data_months: 14 # Standard Firebase setting, balances long-term analysis with privacy.
  event_data_months: 14 # Retention for event data.

# BigQuery Integration Configuration
# For advanced analysis and long-term storage of anonymized/aggregated data.
# REQ-8-005, FR-B-006
bigquery_export:
  enabled: true
  project_id: "pattern-cipher-prod" # To be replaced with the actual GCP project ID.
  dataset_name: "patterncipher_analytics_v1"
  # Daily export is standard. Streaming export can be enabled for real-time needs if required.
  export_frequency: "daily"

# Default Consent Management Settings
# Defines the default analytics consent state for different regions.
# The client will use this as a fallback and for initial state determination.
# REQ-11-005, FR-AT-002
default_consent_settings:
  # 'ad_storage' and 'analytics_storage' are standard keys used by Firebase.
  - region: "DEFAULT"
    ad_storage: "denied"
    analytics_storage: "denied"
  - region: "GDPR" # European Economic Area, UK
    ad_storage: "denied"
    analytics_storage: "denied"
  - region: "CCPA" # California, USA
    ad_storage: "denied"
    analytics_storage: "granted"

# COPPA/Child-Directed Treatment
# Global setting to ensure all SDK behavior is compliant for users identified as children.
# REQ-11-002, REQ-11-004
child_directed_treatment:
  # This flag must be set dynamically on the client-side SDK initialization
  # based on the age gate result, but is documented here as a core policy.
  description: "If the user is identified as being under the age of digital consent, all analytics and ad calls must be flagged as child-directed. This disables collection of advertising IDs and other PII."



## 3. Custom Event Schema

This section specifies the content of the `schemas/custom_events.json` file. This file serves as the strict data contract for all custom analytics events logged by the game client.

**File Path:** `schemas/custom_events.json`

**Structure:** A JSON array where each object represents an event definition.

**Key Requirements Covered:** `FR-AT-001`, `FR-B-006`

json
[
  {
    "eventName": "level_start",
    "description": "Fired when a player begins a level attempt.",
    "parameters": [
      { "paramName": "level_id", "dataType": "string", "description": "Unique identifier for the procedurally generated level." },
      { "paramName": "puzzle_type", "dataType": "string", "description": "The type of puzzle (e.g., 'DirectMatch', 'RuleBased')." },
      { "paramName": "difficulty_tier", "dataType": "long", "description": "The numerical difficulty tier of the level." }
    ]
  },
  {
    "eventName": "level_complete",
    "description": "Fired when a player successfully completes a level.",
    "parameters": [
      { "paramName": "level_id", "dataType": "string", "description": "Unique identifier for the procedurally generated level." },
      { "paramName": "moves_taken", "dataType": "long", "description": "Number of moves the player took to complete the level." },
      { "paramName": "time_taken_seconds", "dataType": "long", "description": "Time in seconds the player took to complete the level." },
      { "paramName": "par_moves", "dataType": "long", "description": "The target 'par' move count for the level." },
      { "paramName": "score_achieved", "dataType": "long", "description": "The final score for the level." },
      { "paramName": "stars_earned", "dataType": "long", "description": "Number of stars earned (1-3)." }
    ]
  },
  {
    "eventName": "level_fail",
    "description": "Fired when a player fails a level (e.g., runs out of moves/time).",
    "parameters": [
      { "paramName": "level_id", "dataType": "string", "description": "Unique identifier for the procedurally generated level." },
      { "paramName": "moves_taken", "dataType": "long", "description": "Number of moves the player had made before failing." },
      { "paramName": "time_taken_seconds", "dataType": "long", "description": "Time in seconds elapsed before failing." },
      { "paramName": "fail_reason", "dataType": "string", "description": "Reason for failure (e.g., 'out_of_moves', 'timeout')." }
    ]
  },
  {
    "eventName": "level_quit",
    "description": "Fired when a player manually quits a level before completion.",
    "parameters": [
      { "paramName": "level_id", "dataType": "string", "description": "Unique identifier for the procedurally generated level." },
      { "paramName": "moves_taken", "dataType": "long", "description": "Number of moves made before quitting." },
      { "paramName": "time_in_level_seconds", "dataType": "long", "description": "Time in seconds spent in the level before quitting." }
    ]
  },
  {
    "eventName": "hint_used",
    "description": "Fired when a player uses a hint.",
    "parameters": [
      { "paramName": "level_id", "dataType": "string", "description": "The level in which the hint was used." },
      { "paramName": "moves_at_hint", "dataType": "long", "description": "The move count when the hint was used." }
    ]
  },
  {
    "eventName": "undo_used",
    "description": "Fired when a player uses the undo action.",
    "parameters": [
      { "paramName": "level_id", "dataType": "string", "description": "The level in which undo was used." },
      { "paramName": "moves_at_undo", "dataType": "long", "description": "The move count when undo was used." }
    ]
  },
  {
    "eventName": "setting_changed",
    "description": "Fired when a player changes a game setting.",
    "parameters": [
      { "paramName": "setting_name", "dataType": "string", "description": "The name of the setting changed (e.g., 'music_volume', 'sfx_volume', 'colorblind_mode')." },
      { "paramName": "setting_value", "dataType": "string", "description": "The new value of the setting." }
    ]
  },
  {
    "eventName": "tutorial_step",
    "description": "Fired for each interaction with the tutorial.",
    "parameters": [
      { "paramName": "tutorial_id", "dataType": "string", "description": "Identifier for the tutorial being shown (e.g., 'core_swap_mechanic')." },
      { "paramName": "step_index", "dataType": "long", "description": "The index of the tutorial step." },
      { "paramName": "status", "dataType": "string", "description": "Status of the step ('started', 'completed', 'skipped')." }
    ]
  }
]


## 4. User Property Schema

This section specifies the content of the `schemas/user_properties.json` file. This defines custom properties that are set on a user's analytics profile for audience segmentation.

**File Path:** `schemas/user_properties.json`

**Key Requirements Covered:** `FR-AT-001`

json
[
  {
    "propertyName": "player_first_open_date",
    "dataType": "string",
    "description": "The date (YYYY-MM-DD) of the user's first session. Used for cohort analysis."
  },
  {
    "propertyName": "highest_level_pack_unlocked",
    "dataType": "string",
    "description": "The identifier of the highest level pack the user has unlocked."
  },
  {
    "propertyName": "total_stars_collected",
    "dataType": "long",
    "description": "The cumulative number of stars the player has earned."
  },
  {
    "propertyName": "player_consent_status",
    "dataType": "string",
    "description": "The current analytics consent status (e.g., 'granted', 'denied')."
  },
  {
    "propertyName": "last_played_puzzle_type",
    "dataType": "string",
    "description": "The type of puzzle the player last engaged with."
  }
]


## 5. Privacy Compliance Map

This section specifies the content of the `schemas/privacy_compliance_map.json` file. This file provides an auditable map of how each event parameter and user property should be handled based on privacy contexts (e.g., child users, unconsented users).

**File Path:** `schemas/privacy_compliance_map.json`

**Key Requirements Covered:** `FR-AT-002`, `FR-AT-003`

**Structure:** A JSON object mapping parameter/property names to compliance rule objects.

json
{
  "compliance_actions": {
    "ALLOW": "The parameter can be collected regardless of consent status (for non-PII operational data).",
    "OMIT_IF_UNCONSENTED": "The parameter must be omitted if the user has not granted analytics_storage consent.",
    "OMIT_FOR_CHILD": "The parameter must be omitted if the user is identified as a child under COPPA/GDPR-K."
  },
  "parameter_map": {
    "level_id": "ALLOW",
    "puzzle_type": "ALLOW",
    "difficulty_tier": "ALLOW",
    "moves_taken": "ALLOW",
    "time_taken_seconds": "ALLOW",
    "par_moves": "ALLOW",
    "score_achieved": "ALLOW",
    "stars_earned": "ALLOW",
    "fail_reason": "ALLOW",
    "time_in_level_seconds": "ALLOW",
    "moves_at_hint": "ALLOW",
    "moves_at_undo": "ALLOW",
    "setting_name": "ALLOW",
    "setting_value": "ALLOW",
    "tutorial_id": "ALLOW",
    "step_index": "ALLOW",
    "status": "ALLOW"
  },
  "user_property_map": {
    "player_first_open_date": "OMIT_IF_UNCONSENTED",
    "highest_level_pack_unlocked": "OMIT_IF_UNCONSENTED",
    "total_stars_collected": "OMIT_IF_UNCONSENTED",
    "player_consent_status": "ALLOW",
    "last_played_puzzle_type": "OMIT_IF_UNCONSENTED"
  }
}


**Note:** The Firebase Analytics SDK itself handles the omission of device-level identifiers (like Advertising ID) when configured for child-directed treatment. This map primarily guides the client-side implementation on which custom parameters to send based on the user's consent status.