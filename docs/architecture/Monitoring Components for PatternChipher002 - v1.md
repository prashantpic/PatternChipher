# Specification

# 1. Monitoring Components

## 1.1. Client-Side APM & Crash Reporting
Monitors the Unity client's real-time performance on user devices and captures/aggregates all unhandled exceptions and crashes. Essential for ensuring reliability (NFR-R-001) and performance (NFR-P-001, NFR-P-002) targets are met across diverse mobile hardware.

### 1.1.3. Type
ApplicationPerformanceMonitoring

### 1.1.5. Provider
Firebase Crashlytics

### 1.1.6. Features

- Automatic crash and ANR (Application Not Responding) reporting.
- Stack trace collection and deobfuscation for debugging.
- Non-fatal exception logging for tracking handled errors (REQ-8-003).
- Device, OS, and app version breakdown for crash reports.
- Integration with Firebase Analytics for crash-free user metrics.

### 1.1.7. Configuration

- **Integration:** Firebase Crashlytics SDK for Unity.
- **Initialization:** Initialize with other Firebase services, respecting user consent for data collection.
- **Error Logging:** Use `Crashlytics.LogException()` for tracking significant, non-fatal errors such as save/load failures or procedural generation validation errors.
- **Custom Keys:** Attach non-PII context like `current_level_id` and `game_state` to crash reports for faster debugging.

## 1.2. Firebase Backend Service Monitoring
Monitors the health, performance, and usage of all utilized Firebase services, which constitute the system's backend infrastructure. This is critical for ensuring the availability of online features and managing operational costs. Addresses NFR-OP-002, NFR-OP-003, NFR-OP-004, and REQ-8-011.

### 1.2.3. Type
InfrastructureMonitoring

### 1.2.5. Provider
Google Cloud Monitoring & Alerting

### 1.2.6. Features

- Cloud Function Performance: Invocations, execution time, memory usage, and error rates.
- Firestore Performance: Read/write/delete operations per second, latency, and cache hit ratio.
- Authentication Monitoring: Sign-in success/failure rates by provider.
- Usage Quotas: Tracking usage against Firebase and Google Cloud quotas.
- Cost Management: Budget tracking and alerting.

### 1.2.7. Configuration

- **Dashboards:** Create custom dashboards in Google Cloud Monitoring to visualize key metrics for Cloud Functions (e.g., error rate > 5%) and Firestore (e.g., p99 latency > 500ms).
- **Alerting:** Configure alerts for critical thresholds: budget alerts, high error rates on functions, high latency on Firestore, and approaching usage quotas.
- **Logs:** Utilize Google Cloud Logging for detailed investigation of function errors or performance spikes.

## 1.3. Gameplay & Business Analytics
Collects anonymized gameplay telemetry to provide actionable insights for game balancing, difficulty curve tuning, and player engagement analysis. This is the primary tool for fulfilling data-driven design requirements REQ-8-001, REQ-8-002, and REQ-8-010.

### 1.3.3. Type
BusinessMetricsTracking

### 1.3.5. Provider
Firebase Analytics

### 1.3.6. Features

- Custom Event Tracking: Logging specific gameplay events like `level_start`, `level_complete`, `level_fail`, `hint_used`, `undo_used`.
- Event Parameters: Attaching rich, non-PII context to events (e.g., `level_id`, `moves_taken`, `time_seconds`, `par_moves`).
- Funnel Analysis: Tracking player progression through tutorials and level packs to identify drop-off points.
- Audience Segmentation: Creating player segments based on progression or behavior for targeted analysis or Remote Config rollouts.
- User Property Tracking: Setting user properties like `highest_level_unlocked` or `total_stars`.

### 1.3.7. Configuration

- **Event Schema:** Define a strict event schema in the GDD, mapping events from REQ-8-002 to Firebase Analytics event names and parameters.
- **Consent Management:** Analytics collection must be strictly tied to the user's consent status (REQ-8-005) and age-gate status (REQ-8-004). The SDK is only initialized if consent is given.
- **Child Directed:** For users identified as under the age of consent, the analytics SDK must be configured for child-directed treatment, disabling advertising ID collection.

## 1.4. Centralized & Diagnostic Logging
Provides logging for both backend Cloud Functions (for debugging server-side logic) and a mechanism for client-side diagnostics. Directly addresses the need for server-side observability and client-side logging specified in REQ-8-009 (NFR-M-005).

### 1.4.3. Type
LogAggregation

### 1.4.5. Provider
Google Cloud Logging (for backend) & Custom Client-Side Service

### 1.4.6. Features

- Backend Logging: Automatic capture of `console.log`, `console.error`, etc., from Cloud Functions into Google Cloud Logging.
- Client-Side Logging: A lightweight, in-game logging service that writes to a local file, categorized by severity (Info, Warning, Error).
- User-Triggered Submission: A mechanism for the player to package and submit their local diagnostic logs with explicit consent, uploading them to a secure Firebase Cloud Storage bucket for support analysis.
- Structured Logging: For backend logs, allowing for powerful filtering and analysis in the Google Cloud Console.
- PII Scrubbing: Client-side log submission logic should scrub potential PII before upload.

### 1.4.7. Configuration

- **Backend:** No special configuration needed; Cloud Functions log to Google Cloud Logging by default. Set up log-based metrics and alerts in Google Cloud Monitoring for specific error strings.
- **Client:** Implement a `LogService` in Unity. In production builds, set the default logging level to `Error` to minimize performance impact. The log submission feature will be accessible via a hidden debug menu or a support contact flow.



---

