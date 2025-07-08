# Software Design Specification (SDS) for REPO-PATT-010: AnalyticsCollectorEndpoints

## 1. Introduction

This document provides the detailed software design specification for the **AnalyticsCollectorEndpoints** repository (`REPO-PATT-010`). This repository represents the backend data ingestion service provided by **Firebase Analytics**. It does not contain executable code itself; instead, this specification defines the **data contract**—the structure and format of custom events—that the client application (`REPO-PATT-001`) must adhere to when sending telemetry data.

The primary purpose of this analytics system is to collect anonymized or pseudonymized player behavior data to inform game balancing, identify player friction points, measure feature adoption, and monitor application stability, as outlined in requirements `FR-AT-001`, `FR-AT-002`, `FR-AT-003`, and `FR-B-006`.

## 2. Architectural Overview

The `AnalyticsCollectorEndpoints` service functions as a high-throughput, "fire-and-forget" event ingestion endpoint within the overall system architecture.

-   **Pattern:** It adheres to an **Event-Driven, Publish-Subscribe** model. The game client publishes events without expecting a direct response.
-   **Interaction:** The client-side `BackendServiceFacade` (`REPO-PATT-005`) is the sole component responsible for formatting and sending events to this service via the Firebase Analytics SDK.
-   **Technology:** The service is fully managed by **Firebase Analytics**.

This design decouples the game client from the complexities of data collection, storage, and processing, ensuring that analytics logging has a minimal performance impact on the user's gameplay experience.

## 3. Core Principles & Compliance

All analytics event collection must adhere to the following principles:

-   **User Consent (`FR-AT-002`):** The client application is responsible for enforcing user consent. No analytics events shall be sent for users who have opted out or have not yet opted in (as required by regional regulations). The `BackendServiceFacade` must check the consent status before logging any event.
-   **Data Anonymization (`FR-AT-001`, `FR-AT-003`):** All data sent to the analytics service must be anonymized or pseudonymized. No Personal Identifiable Information (PII) should be included in event payloads. The `playerId` must be a non-personally-identifiable GUID.
-   **Child Privacy (COPPA/GDPR-K):** For users identified as children via the age gate, the Firebase Analytics SDK must be configured for "child-directed treatment," which further restricts data collection to ensure compliance.
-   **Secure Transmission (`FR-AT-003`):** All data transmission is handled by the Firebase SDK, which uses HTTPS/TLS to ensure data is encrypted in transit.

## 4. Event Definitions (Data Contracts)

The following custom events define the data contract for the analytics service. The `BackendServiceFacade` will be responsible for constructing these events with the specified parameters and logging them using the `FirebaseAnalytics.LogEvent(string name, params Parameter[] parameters)` method.

---

### 4.1. Event: `session_start`

-   **Purpose:** To log the beginning of a new game session. Provides foundational context for user engagement and technical segmentation.
-   **Trigger:** Fired once when the application is launched or resumes from the background after a significant period of inactivity (as defined by the Firebase SDK's session management).
-   **Requirement ID:** `FR-AT-001`
-   **Parameters:**

| Parameter Name  | Data Type | Required | Description                                                                          |
| :-------------- | :-------- | :------- | :----------------------------------------------------------------------------------- |
| `app_version`   | `string`  | Yes      | The client application version (e.g., "1.0.0").                                      |
| `platform`      | `string`  | Yes      | The client platform (e.g., "iOS", "Android").                                        |
| `device_model`  | `string`  | Yes      | The client device model (e.g., "iPhone14,2", "Pixel 6"). Provided by `SystemInfo`. |
| `os_version`    | `string`  | Yes      | The operating system version of the client device.                                   |

---

### 4.2. Event: `level_start`

-   **Purpose:** To track the initiation of a level attempt. This is the starting point for creating progression funnels.
-   **Trigger:** Fired when the player enters the active game screen for a specific level.
-   **Requirement ID:** `FR-AT-001`, `FR-B-006`
-   **Parameters:**

| Parameter Name | Data Type | Required | Description                                                               |
| :------------- | :-------- | :------- | :------------------------------------------------------------------------ |
| `level_id`     | `string`  | Yes      | A unique identifier for the level being played (e.g., a GUID or seed).      |
| `level_type`   | `string`  | Yes      | The type of puzzle (e.g., "DirectMatch", "RuleBased").                    |
| `difficulty`   | `long`    | Yes      | An integer representing the difficulty tier or value of the level.        |
| `grid_size`    | `string`  | Yes      | The dimensions of the grid for this level (e.g., "3x3", "8x8").           |

---

### 4.3. Event: `level_outcome`

-   **Purpose:** To capture the result of a level attempt, providing critical data for balancing difficulty and analyzing player friction.
-   **Trigger:** Fired when a level ends for any reason (completion, failure, or the player quitting).
-   **Requirement ID:** `FR-AT-001`, `FR-B-006`
-   **Parameters:**

| Parameter Name       | Data Type | Required | Description                                                                                                   |
| :------------------- | :-------- | :------- | :------------------------------------------------------------------------------------------------------------ |
| `level_id`           | `string`  | Yes      | A unique identifier for the level that was played.                                                            |
| `outcome`            | `string`  | Yes      | The result of the level attempt. Enum: `complete`, `fail`, `quit`.                                            |
| `time_taken_seconds` | `long`    | Yes      | Total time in seconds the player spent in the level.                                                          |
| `moves_taken`        | `long`    | Yes      | Total number of valid moves the player made.                                                                  |
| `score`              | `long`    | No       | The final score achieved. Required only if `outcome` is `complete`.                                           |
| `stars_awarded`      | `long`    | No       | The number of stars awarded (1-3). Required only if `outcome` is `complete`.                                  |
| `failure_reason`     | `string`  | No       | The reason for failure (e.g., "OutOfMoves", "TimerExpired"). Required only if `outcome` is `fail`. |

---

### 4.4. Event: `player_action_used`

-   **Purpose:** To track the usage of key assistance mechanics, indicating points where players may be struggling.
-   **Trigger:** Fired each time the player successfully uses an Undo or Hint action.
-   **Requirement ID:** `FR-AT-001`
-   **Parameters:**

| Parameter Name  | Data Type | Required | Description                                                                |
| :-------------- | :-------- | :------- | :------------------------------------------------------------------------- |
| `level_id`      | `string`  | Yes      | The identifier of the level where the action was used.                     |
| `action_type`   | `string`  | Yes      | The type of action used. Enum: `hint`, `undo`.                             |
| `moves_at_action` | `long`    | Yes      | The move count of the player at the moment the action was used.          |

---

### 4.5. Event: `client_error`

-   **Purpose:** To capture client-side errors for stability monitoring and bug prioritization.
-   **Trigger:** Fired by a global exception handler in the client application when an unhandled exception or significant non-fatal error occurs.
-   **Requirement ID:** `FR-AT-001`, `FR-AT-003`
-   **Parameters:**

| Parameter Name     | Data Type | Required | Description                                                                                           |
| :----------------- | :-------- | :------- | :---------------------------------------------------------------------------------------------------- |
| `error_type`       | `string`  | Yes      | The type or class of the error (e.g., "NullReferenceException", "PCG_Unsolvable").                    |
| `error_context`    | `string`  | Yes      | A brief, non-PII description of the context (e.g., "DuringLevelLoad", "OnTileSwap").                  |
| `stack_trace_hash` | `string`  | Yes      | An SHA256 hash of the full, sanitized stack trace. Used to group identical errors in the backend.     |

## 5. Integration and Configuration

-   **Client Integration:** The `BackendServiceFacade` (`REPO-PATT-005`) will encapsulate all calls to the Firebase Analytics SDK. It will expose a single method, such as `public void LogAnalyticsEvent(string eventName, Dictionary<string, object> parameters)`, which will be responsible for converting the dictionary into the `Parameter[]` array expected by Firebase and making the `FirebaseAnalytics.LogEvent()` call.
-   **Configuration:** The client application must be configured with the correct Firebase Project ID, App ID, and API keys via the `google-services.json` (Android) and `GoogleService-Info.plist` (iOS) files.
-   **Batching:** The system will rely on the Firebase SDK's default event batching mechanism to conserve network resources and battery life, aligning with `FR-AT-001`. No custom batching logic is required in the client.