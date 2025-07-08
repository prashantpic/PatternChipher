# Specification

# 1. Dashboards

## 1.1. Client Health & Stability
Monitors client-side application stability, crash rates, and performance on user devices. Primarily for the client development team.

### 1.1.3. Type
application

### 1.1.4. Panels

- **Title:** Crash-Free Users (Daily)  
**Type:** gauge  
**Metrics:**
    
    - client.crash_free_users_rate
    
- **Title:** Crash & ANR Rate Trend (Last 14 Days)  
**Type:** line_chart  
**Metrics:**
    
    - client.anr_rate
    - error.client.crash
    
- **Title:** Top Crashing Versions & Devices  
**Type:** table  
**Metrics:**
    
    - error.client.crash{group_by: app_version, os_version, device_model}
    
- **Title:** Critical Non-Fatal Errors  
**Type:** stat  
**Metrics:**
    
    - error.client.pcg_unsolvable
    - SaveFileCorruptException
    

## 1.2. Backend Service Health & Cost
Real-time monitoring of Firebase backend services' health, performance, and operational cost. Primarily for the backend and operations team.

### 1.2.3. Type
overview

### 1.2.4. Panels

- **Title:** Cloud Function Error Rate  
**Type:** gauge  
**Metrics:**
    
    - backend.function.invocations{status:error}
    
- **Title:** Firestore Request Latency (p95)  
**Type:** line_chart  
**Metrics:**
    
    - backend.firestore.request_latency{percentile:p95}
    
- **Title:** Firebase Usage vs. Quotas  
**Type:** stacked_bar  
**Metrics:**
    
    - backend.firestore.usage_vs_quota
    - backend.function.usage_vs_quota
    
- **Title:** Firebase Cost Per DAU  
**Type:** stat  
**Metrics:**
    
    - ops.firebase_cost_per_dau
    

## 1.3. Business KPIs & Player Engagement
High-level dashboard for product owners and stakeholders to track key business metrics and user engagement.

### 1.3.3. Type
business

### 1.3.4. Panels

- **Title:** Core Engagement KPIs  
**Type:** stat  
**Metrics:**
    
    - engagement.dau
    - kpi.retention.d1
    - kpi.retention.d7
    
- **Title:** Onboarding Funnel  
**Type:** funnel  
**Metrics:**
    
    - funnel.tutorial_completion_rate
    
- **Title:** Session Length Distribution  
**Type:** histogram  
**Metrics:**
    
    - engagement.session_length
    
- **Title:** Online Feature Adoption Rate  
**Type:** gauge  
**Metrics:**
    
    - engagement.feature_adoption.cloud_save
    

## 1.4. Game Balance & Difficulty Analysis
Detailed gameplay metrics for game designers to analyze level difficulty, identify friction points, and tune the player experience.

### 1.4.3. Type
custom

### 1.4.4. Panels

- **Title:** Level Completion Rate  
**Type:** bar_chart  
**Metrics:**
    
    - transaction.level.completion{status:success, group_by:levelId}
    
- **Title:** Level Failure/Quit Rate  
**Type:** table  
**Metrics:**
    
    - kpi.level.failure_rate{group_by:levelId}
    
- **Title:** Average Completion Time by Level  
**Type:** line_chart  
**Metrics:**
    
    - transaction.level.completion_time{aggregation:avg, group_by:levelId}
    
- **Title:** Leaderboard Submission Failures  
**Type:** stat  
**Metrics:**
    
    - transaction.leaderboard.submission{status:validation_fail}
    



---

