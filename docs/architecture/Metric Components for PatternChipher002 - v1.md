# Specification

# 1. Telemetry And Metrics Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - Unity (C#)
    - Firebase Firestore
    - Firebase Cloud Functions (TypeScript/Node.js)
    - Firebase Authentication
    - Firebase Analytics
    - Firebase Crashlytics
    - Firebase Remote Config
    - Google Cloud Monitoring
    
  - **Monitoring Components:**
    
    - Client-Side APM & Crash Reporting
    - Firebase Backend Service Monitoring
    - Gameplay & Business Analytics
    - Centralized & Diagnostic Logging
    
  - **Requirements:**
    
    - NFR-P-001 (Responsiveness)
    - NFR-P-002 (Performance)
    - NFR-R-001 (Reliability)
    - REQ-8-001 (Analytics Data Collection)
    - REQ-8-010 (Balancing with Analytics)
    - NFR-OP-002 (Backend Monitoring)
    - NFR-OP-003 (Backend Alerting)
    - NFR-OP-004 (Cost Management)
    
  - **Environment:** production
  
- **Standard System Metrics Selection:**
  
  - **Hardware Utilization Metrics:**
    
    - **Name:** client.cpu.utilization  
**Type:** gauge  
**Unit:** percentage  
**Description:** Client-side CPU utilization, collected as part of performance profiling during development and for high-severity ANR/crash events. Not a real-time production metric.  
**Collection:**
    
    - **Interval:** on-demand
    - **Method:** profiling
    
**Thresholds:**
    
    - **Warning:** N/A
    - **Critical:** N/A
    
**Justification:** Required for development-time optimization to meet NFR-P-002 and diagnose performance bottlenecks causing ANRs.  
    - **Name:** client.memory.usage  
**Type:** gauge  
**Unit:** megabytes  
**Description:** Client-side memory usage, collected during development profiling and attached to crash reports.  
**Collection:**
    
    - **Interval:** on-demand
    - **Method:** profiling
    
**Thresholds:**
    
    - **Warning:** N/A
    - **Critical:** N/A
    
**Justification:** Essential for diagnosing memory leaks and optimizing asset loading to meet NFR-P-002 and prevent out-of-memory crashes (NFR-R-001).  
    
  - **Runtime Metrics:**
    
    - **Name:** backend.function.execution_time  
**Type:** histogram  
**Unit:** milliseconds  
**Description:** Execution time for each Firebase Cloud Function (e.g., LeaderboardFunction). Provided by Google Cloud Monitoring.  
**Technology:** Node.js  
**Collection:**
    
    - **Interval:** real-time
    - **Method:** provider_native
    
**Criticality:** high  
    - **Name:** backend.function.invocations  
**Type:** counter  
**Unit:** count  
**Description:** Invocation count for each Cloud Function, split by status (ok, error, timeout). Provided by Google Cloud Monitoring.  
**Technology:** Node.js  
**Collection:**
    
    - **Interval:** real-time
    - **Method:** provider_native
    
**Criticality:** high  
    - **Name:** client.anr_rate  
**Type:** gauge  
**Unit:** percentage  
**Description:** Application Not Responding (ANR) rate for the client app. Provided by Firebase Crashlytics.  
**Technology:** Unity  
**Collection:**
    
    - **Interval:** N/A
    - **Method:** provider_native
    
**Criticality:** high  
    
  - **Request Response Metrics:**
    
    - **Name:** backend.firestore.request_latency  
**Type:** histogram  
**Unit:** milliseconds  
**Description:** Latency for Firestore read, write, and delete operations. Provided by Google Cloud Monitoring.  
**Dimensions:**
    
    - operation_type
    
**Percentiles:**
    
    - p50
    - p95
    - p99
    
**Collection:**
    
    - **Interval:** real-time
    - **Method:** provider_native
    
    
  - **Availability Metrics:**
    
    - **Name:** backend.service.availability  
**Type:** gauge  
**Unit:** percentage  
**Description:** Availability of critical Firebase services (Auth, Firestore, Functions) as reported by Google Cloud status dashboards.  
**Calculation:** Uptime / (Uptime + Downtime) over a time window.  
**Sla Target:** 99.5%  
    - **Name:** client.crash_free_users_rate  
**Type:** gauge  
**Unit:** percentage  
**Description:** Percentage of users who did not experience a crash in a given period. Provided by Firebase Crashlytics.  
**Calculation:** 1 - (Crashing Users / Total Users)  
**Sla Target:** 99.8%  
    
  - **Scalability Metrics:**
    
    - **Name:** backend.firestore.usage_vs_quota  
**Type:** gauge  
**Unit:** percentage  
**Description:** Current usage (reads, writes, deletes, stored data) against Firebase Firestore quotas.  
**Capacity Threshold:** 85%  
**Auto Scaling Trigger:** True  
    - **Name:** backend.function.usage_vs_quota  
**Type:** gauge  
**Unit:** percentage  
**Description:** Current usage (invocations, compute-seconds) against Cloud Functions quotas.  
**Capacity Threshold:** 85%  
**Auto Scaling Trigger:** True  
    
  
- **Application Specific Metrics Design:**
  
  - **Transaction Metrics:**
    
    - **Name:** transaction.level.completion  
**Type:** counter  
**Unit:** count  
**Description:** Counts level completion events to track player progression and identify difficult levels. REQ-8-002.  
**Business_Context:** Player Engagement & Level Balancing  
**Dimensions:**
    
    - levelId
    - puzzleType
    - status(success|fail|quit)
    
**Collection:**
    
    - **Interval:** on_event
    - **Method:** batch
    
**Aggregation:**
    
    - **Functions:**
      
      - sum
      - rate
      
    - **Window:** 1h
    
    - **Name:** transaction.level.completion_time  
**Type:** histogram  
**Unit:** seconds  
**Description:** Distribution of time taken to complete levels, used for difficulty tuning. REQ-8-002.  
**Business_Context:** Level Balancing  
**Dimensions:**
    
    - levelId
    - puzzleType
    
**Collection:**
    
    - **Interval:** on_event
    - **Method:** batch
    
**Aggregation:**
    
    - **Functions:**
      
      - avg
      - p90
      
    - **Window:** 24h
    
    - **Name:** transaction.leaderboard.submission  
**Type:** counter  
**Unit:** count  
**Description:** Counts attempts to submit a score to the leaderboard, tracked server-side. REQ-9-002.  
**Business_Context:** Online Feature Usage  
**Dimensions:**
    
    - leaderboardId
    - status(success|validation_fail|error)
    
**Collection:**
    
    - **Interval:** on_event
    - **Method:** real-time
    
**Aggregation:**
    
    - **Functions:**
      
      - sum
      
    - **Window:** 1h
    
    - **Name:** transaction.cloudsave.sync  
**Type:** counter  
**Unit:** count  
**Description:** Counts cloud save synchronization events. REQ-10-007.  
**Business_Context:** Online Feature Usage & Reliability  
**Dimensions:**
    
    - trigger(manual|auto)
    - status(success|fail|conflict_resolved)
    
**Collection:**
    
    - **Interval:** on_event
    - **Method:** real-time
    
**Aggregation:**
    
    - **Functions:**
      
      - sum
      
    - **Window:** 1h
    
    
  - **Cache Performance Metrics:**
    
    - **Name:** client.remote_config.cache_status  
**Type:** counter  
**Unit:** count  
**Description:** Tracks whether the client used cached or freshly fetched Remote Config values at startup. REQ-8-006.  
**Cache Type:** client_side_sdk  
**Hit Ratio Target:** 95%  
    
  - **External Dependency Metrics:**
    
    - **Name:** dependency.firebase_auth.latency  
**Type:** histogram  
**Unit:** milliseconds  
**Description:** Latency for Firebase Authentication operations (e.g., sign-in).  
**Dependency:** Firebase Authentication  
**Circuit Breaker Integration:** True  
**Sla:**
    
    - **Response Time:** 1500ms
    - **Availability:** 99.9%
    
    
  - **Error Metrics:**
    
    - **Name:** error.client.crash  
**Type:** counter  
**Unit:** count  
**Description:** Counts client application crashes. Provided by Firebase Crashlytics. NFR-R-001.  
**Error Types:**
    
    - unhandled_exception
    
**Dimensions:**
    
    - os_version
    - device_model
    - app_version
    
**Alert Threshold:** Spike > 5% of users in 1hr  
    - **Name:** error.backend.function_failures  
**Type:** counter  
**Unit:** count  
**Description:** Counts server-side Cloud Function execution failures. REQ-8-003.  
**Error Types:**
    
    - uncaught_exception
    - timeout
    
**Dimensions:**
    
    - function_name
    
**Alert Threshold:** >1% error rate over 5 min  
    - **Name:** error.client.pcg_unsolvable  
**Type:** counter  
**Unit:** count  
**Description:** Counts instances where the client-side Procedural Content Generator fails to create a solvable level after retries. REQ-8-003.  
**Error Types:**
    
    - LevelGenerationUnsolvableException
    
**Dimensions:**
    
    - app_version
    
**Alert Threshold:** Spike > 10 events in 1hr  
    
  - **Throughput And Latency Metrics:**
    
    - **Name:** latency.leaderboard.submission_e2e  
**Type:** histogram  
**Unit:** milliseconds  
**Description:** End-to-end latency from client submitting a score to the server confirming the write.  
**Percentiles:**
    
    - p50
    - p95
    - p99
    
**Buckets:**
    
    - 100
    - 250
    - 500
    - 1000
    - 2000
    
**Sla Targets:**
    
    - **P95:** 1000ms
    - **P99:** 1500ms
    
    
  
- **Business Kpi Identification:**
  
  - **Critical Business Metrics:**
    
    - **Name:** kpi.retention.d1  
**Type:** gauge  
**Unit:** percentage  
**Description:** Percentage of new users who return the day after their first session. Key indicator of initial engagement.  
**Business Owner:** Product Team  
**Calculation:** (Returning Users on Day 2) / (New Users on Day 1)  
**Reporting Frequency:** daily  
**Target:** >40%  
    - **Name:** kpi.retention.d7  
**Type:** gauge  
**Unit:** percentage  
**Description:** Percentage of new users who return 7 days after their first session. Key indicator of medium-term engagement.  
**Business Owner:** Product Team  
**Calculation:** (Returning Users on Day 8) / (New Users on Day 1)  
**Reporting Frequency:** daily  
**Target:** >20%  
    
  - **User Engagement Metrics:**
    
    - **Name:** engagement.dau  
**Type:** gauge  
**Unit:** count  
**Description:** Daily Active Users. The total number of unique users who open the app each day.  
**Segmentation:**
    
    - country
    - app_version
    
**Cohort Analysis:** True  
    - **Name:** engagement.session_length  
**Type:** histogram  
**Unit:** minutes  
**Description:** Distribution of time users spend in the app per session.  
**Segmentation:**
    
    - country
    - player_progression_tier
    
**Cohort Analysis:** True  
    - **Name:** engagement.feature_adoption.cloud_save  
**Type:** gauge  
**Unit:** percentage  
**Description:** Percentage of DAU that has enabled the optional cloud save feature. REQ-10-007.  
**Segmentation:**
    
    
**Cohort Analysis:** False  
    
  - **Conversion Metrics:**
    
    - **Name:** funnel.tutorial_completion_rate  
**Type:** gauge  
**Unit:** percentage  
**Description:** Percentage of new users who complete the initial interactive tutorial. REQ-UIX-007.  
**Funnel Stage:** Onboarding  
**Conversion Target:** 95%  
    - **Name:** funnel.level_pack_1_completion_rate  
**Type:** gauge  
**Unit:** percentage  
**Description:** Percentage of users who start Level Pack 1 that go on to complete it.  
**Funnel Stage:** Early Gameplay  
**Conversion Target:** 70%  
    
  - **Operational Efficiency Kpis:**
    
    - **Name:** ops.firebase_cost_per_dau  
**Type:** gauge  
**Unit:** usd  
**Description:** Total daily Firebase cost divided by Daily Active Users. Monitors cost efficiency. NFR-OP-004.  
**Calculation:** (Daily Firebase Bill) / (DAU)  
**Benchmark Target:** <$0.001  
    
  - **Revenue And Cost Metrics:**
    
    
  - **Customer Satisfaction Indicators:**
    
    - **Name:** satisfaction.crash_free_users_daily  
**Type:** gauge  
**Unit:** percentage  
**Description:** Proxy for customer satisfaction; percentage of daily active users who did not experience a crash. NFR-R-001.  
**Data Source:** Firebase Crashlytics  
**Update Frequency:** daily  
    
  
- **Collection Interval Optimization:**
  
  - **Sampling Frequencies:**
    
    - **Metric Category:** Client Gameplay Analytics  
**Interval:** Batch on app background or every 10 mins  
**Justification:** Balances need for timely data for analysis with the need to conserve user battery life and data usage, as per REQ-8-002.  
**Resource Impact:** low  
    - **Metric Category:** Backend Service Metrics (Firebase)  
**Interval:** real-time  
**Justification:** Provided natively by Google Cloud Monitoring for immediate operational insight into backend health.  
**Resource Impact:** high  
    - **Metric Category:** Client Crash/Error Reporting  
**Interval:** on_event  
**Justification:** Critical errors and crashes must be reported immediately to enable rapid response and debugging.  
**Resource Impact:** medium  
    
  - **High Frequency Metrics:**
    
    - **Name:** backend.function.error_rate  
**Interval:** 1 minute  
**Criticality:** high  
**Cost Justification:** Essential for backend reliability and preventing widespread service degradation.  
    
  - **Cardinality Considerations:**
    
    - **Metric Name:** transaction.level.completion  
**Estimated Cardinality:** High (due to levelId)  
**Dimension Strategy:** Use 'levelId' and 'puzzleType' as dimensions, but avoid 'playerId'.  
**Mitigation Approach:** Leverage Firebase Analytics' ability to handle high cardinality dimensions for core reporting. Do not create custom metrics in other systems with player-id as a dimension.  
    
  - **Aggregation Periods:**
    
    - **Metric Type:** Operational Health (Errors, Latency)  
**Periods:**
    
    - 1m
    - 5m
    - 1h
    
**Retention Strategy:** 30 days raw, 1 year aggregated  
    - **Metric Type:** Business & Engagement KPIs  
**Periods:**
    
    - 1d
    - 7d
    - 30d
    
**Retention Strategy:** 14 months for user-level, indefinite for aggregated  
    
  - **Collection Methods:**
    
    - **Method:** batch  
**Applicable Metrics:**
    
    - transaction.level.completion
    - transaction.level.completion_time
    - engagement.session_length
    
**Implementation:** Firebase Analytics SDK's automatic batching.  
**Performance:** Optimized for mobile battery and data usage.  
    
  
- **Aggregation Method Selection:**
  
  - **Statistical Aggregations:**
    
    - **Metric Name:** transaction.level.completion_time  
**Aggregation Functions:**
    
    - avg
    - min
    - max
    
**Windows:**
    
    - 24h
    
**Justification:** Understanding average and outlier completion times is crucial for tuning level difficulty as per REQ-8-010.  
    
  - **Histogram Requirements:**
    
    - **Metric Name:** backend.function.execution_time  
**Buckets:**
    
    - 50ms
    - 100ms
    - 250ms
    - 500ms
    - 1000ms
    
**Percentiles:**
    
    - p50
    - p90
    - p95
    - p99
    
**Accuracy:** High  
    - **Metric Name:** latency.leaderboard.submission_e2e  
**Buckets:**
    
    - 100ms
    - 250ms
    - 500ms
    - 1000ms
    - 2000ms
    
**Percentiles:**
    
    - p50
    - p95
    - p99
    
**Accuracy:** High  
    
  - **Percentile Calculations:**
    
    - **Metric Name:** backend.firestore.request_latency  
**Percentiles:**
    
    - p50
    - p95
    - p99
    
**Algorithm:** provider_native  
**Accuracy:** High  
    
  - **Metric Types:**
    
    - **Name:** engagement.dau  
**Implementation:** gauge  
**Reasoning:** DAU represents a point-in-time value for a given day, not a value that monotonically increases.  
**Resets Handling:** Calculated daily.  
    - **Name:** error.client.crash  
**Implementation:** counter  
**Reasoning:** Crashes are discrete events that should be summed over time to understand trends and rates.  
**Resets Handling:** Aggregated over time windows (e.g., hourly, daily).  
    
  - **Dimensional Aggregation:**
    
    - **Metric Name:** transaction.level.completion  
**Dimensions:**
    
    - levelId
    - status
    
**Aggregation Strategy:** Sum counts for each unique combination of dimensions.  
**Cardinality Impact:** High, managed by Firebase Analytics.  
    
  - **Derived Metrics:**
    
    - **Name:** kpi.level.failure_rate  
**Calculation:** SUM(transaction.level.completion where status='fail') / SUM(transaction.level.completion where eventName='level_start')  
**Source Metrics:**
    
    - transaction.level.completion
    
**Update Frequency:** daily  
    - **Name:** ops.firebase_cost_per_dau  
**Calculation:** SUM(firebase.billing.cost) / GAUGE(engagement.dau)  
**Source Metrics:**
    
    - firebase.billing.cost
    - engagement.dau
    
**Update Frequency:** daily  
    
  
- **Storage Requirements Planning:**
  
  - **Retention Periods:**
    
    - **Metric Type:** Raw Backend Logs (Google Cloud Logging)  
**Retention Period:** 30 days  
**Justification:** Sufficient for short-term debugging and incident response without incurring high storage costs.  
**Compliance Requirement:** N/A  
    - **Metric Type:** Firebase Analytics User-Level Data  
**Retention Period:** 14 months  
**Justification:** Allows for year-over-year cohort analysis while complying with data minimization principles.  
**Compliance Requirement:** GDPR/CCPA  
    - **Metric Type:** Aggregated Firebase Analytics Data  
**Retention Period:** Indefinite  
**Justification:** Aggregated, anonymized data is valuable for long-term trend analysis and does not pose a privacy risk.  
**Compliance Requirement:** N/A  
    
  - **Data Resolution:**
    
    - **Time Range:** 0-30 days  
**Resolution:** 1 minute  
**Query Performance:** High  
**Storage Optimization:** Raw data for high-fidelity analysis.  
    - **Time Range:** 30-90 days  
**Resolution:** 1 hour  
**Query Performance:** Medium  
**Storage Optimization:** Downsampled data for trend analysis.  
    
  - **Downsampling Strategies:**
    
    - **Source Resolution:** 1 minute  
**Target Resolution:** 1 hour  
**Aggregation Method:** avg for gauges, sum for counters  
**Trigger Condition:** After 30 days  
    
  - **Storage Performance:**
    
    - **Write Latency:** <100ms for backend logs
    - **Query Latency:** <5s for typical dashboard queries
    - **Throughput Requirements:** Scales with user base (Firebase managed)
    - **Scalability Needs:** Must handle traffic spikes during new content releases.
    
  - **Query Optimization:**
    
    - **Query Pattern:** Time-series analysis of level completion rates by levelId.  
**Optimization Strategy:** Use partitioned storage in analytics backend (native feature).  
**Indexing Requirements:**
    
    - eventName
    - eventTimestamp
    - levelId
    
    
  - **Cost Optimization:**
    
    - **Strategy:** Analytics Event Batching  
**Implementation:** Leverage Firebase Analytics SDK's default batching mechanism.  
**Expected Savings:** Significant reduction in network requests and associated costs.  
**Tradeoffs:** Slight delay in data availability.  
    - **Strategy:** Log Level Configuration  
**Implementation:** Set backend function logging to INFO in production, not DEBUG.  
**Expected Savings:** Reduces log ingestion and storage costs in Google Cloud Logging.  
**Tradeoffs:** Less detailed logs for non-error scenarios.  
    
  
- **Project Specific Metrics Config:**
  
  - **Standard Metrics:**
    
    
  - **Custom Metrics:**
    
    
  - **Dashboard Metrics:**
    
    
  
- **Implementation Priority:**
  
  
- **Risk Assessment:**
  
  
- **Recommendations:**
  
  


---

