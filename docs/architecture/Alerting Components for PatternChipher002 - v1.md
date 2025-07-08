# Specification

# 1. Alerting And Incident Response Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - Unity (C#)
    - Firebase Firestore
    - Firebase Cloud Functions
    - Firebase Authentication
    - Firebase Remote Config
    - Firebase Analytics
    - Firebase Crashlytics
    - Google Cloud Monitoring
    
  - **Metrics Configuration:**
    
    - Firebase Crashlytics for client stability metrics (Crash-Free Users).
    - Firebase Analytics for custom gameplay events (level completion, hint usage, failures).
    - Google Cloud Monitoring for Firebase backend service health (Function errors/latency, Firestore reads/writes/latency, Auth failures).
    
  - **Monitoring Needs:**
    
    - Ensure core offline gameplay is highly reliable and performant.
    - Guarantee availability and low latency for optional online features (Cloud Save, Leaderboards).
    - Detect and respond to backend service degradation or failure.
    - Identify and triage client-side crashes and performance issues.
    - Monitor for fraudulent or abusive behavior (e.g., leaderboard cheating).
    - Manage operational costs by monitoring service usage against budgets.
    
  - **Environment:** production
  
- **Alert Condition And Threshold Design:**
  
  - **Critical Metrics Alerts:**
    
    - **Metric:** Firebase Cloud Function Error Rate  
**Condition:** Rate > 5% for 5 minutes  
**Threshold Type:** static  
**Value:** 5  
**Justification:** A sustained error rate indicates a significant backend bug affecting core online features like leaderboard submissions or cloud save logic. REQ-9-009, NFR-OP-003.  
**Business Impact:** High  
    - **Metric:** Firebase Crashlytics Crash-Free Users  
**Condition:** Percentage < 99.5% for a 24-hour period  
**Threshold Type:** static  
**Value:** 99.5  
**Justification:** Directly measures application stability and user experience as per NFR-R-001. A drop indicates a widespread issue possibly introduced in a new release.  
**Business Impact:** High  
    - **Metric:** Firestore Security Rule Denials  
**Condition:** Count > 20 over 10 minutes  
**Threshold Type:** static  
**Value:** 20  
**Justification:** A spike in security rule denials can indicate a misconfigured client, a buggy new release, or a potential security attack. REQ-9-010, NFR-SEC-004.  
**Business Impact:** Critical  
    - **Metric:** Google Cloud Billing Budget  
**Condition:** Actual spend > 90% of monthly budget  
**Threshold Type:** static  
**Value:** 90  
**Justification:** Essential for cost management to prevent unexpected overages on Firebase services. NFR-OP-004.  
**Business Impact:** Critical  
    - **Metric:** Dead Letter Queue (DLQ) Size  
**Condition:** Count > 0  
**Threshold Type:** static  
**Value:** 0  
**Justification:** Any message in the DLQ represents a permanently failed event (e.g., failed leaderboard submission) that requires manual intervention. Essential for data integrity.  
**Business Impact:** Medium  
    
  - **Threshold Strategies:**
    
    
  - **Baseline Deviation Alerts:**
    
    
  - **Predictive Alerts:**
    
    
  - **Compound Conditions:**
    
    - **Name:** New Version Instability  
**Conditions:**
    
    - Crash-Free Users < 99.8%
    - Active on new app version
    
**Logic:** AND  
**Time Window:** 24h  
**Justification:** Isolates stability problems to a new release, allowing for targeted investigation or rollback decisions, reducing false positives from older, known issues.  
    
  
- **Severity Level Classification:**
  
  - **Severity Definitions:**
    
    - **Level:** Critical  
**Criteria:** System-wide outage, PII data breach, critical data loss (e.g., cloud save), core gameplay feature unavailable for >5% of users. Requires immediate, all-hands response.  
**Business Impact:** Severe (Reputation, Revenue, Legal)  
**Customer Impact:** Severe  
**Response Time:** < 15 minutes  
**Escalation Required:** True  
    - **Level:** High  
**Criteria:** A major non-core feature is unavailable (e.g., all leaderboards), significant performance degradation, widespread crashes impacting a specific user segment. Requires immediate response from on-call personnel.  
**Business Impact:** Significant  
**Customer Impact:** Significant  
**Response Time:** < 30 minutes  
**Escalation Required:** True  
    - **Level:** Medium  
**Criteria:** A single non-critical feature is failing (e.g., one leaderboard), intermittent errors, performance degradation for a small subset of users. Requires attention during business hours.  
**Business Impact:** Moderate  
**Customer Impact:** Moderate  
**Response Time:** < 4 business hours  
**Escalation Required:** False  
    - **Level:** Low  
**Criteria:** Informational alerts, non-impacting errors, performance metrics nearing warning thresholds. For awareness and proactive monitoring.  
**Business Impact:** Minimal  
**Customer Impact:** Minimal  
**Response Time:** Next business day  
**Escalation Required:** False  
    
  - **Business Impact Matrix:**
    
    
  - **Customer Impact Criteria:**
    
    
  - **Sla Violation Severity:**
    
    
  - **System Health Severity:**
    
    
  
- **Notification Channel Strategy:**
  
  - **Channel Configuration:**
    
    - **Channel:** pagerduty  
**Purpose:** Primary alerting for Critical incidents requiring immediate on-call response.  
**Applicable Severities:**
    
    - Critical
    
**Time Constraints:** 24/7  
**Configuration:**
    
    
    - **Channel:** slack  
**Purpose:** Primary notification for High/Medium severity alerts to engage the relevant team channels.  
**Applicable Severities:**
    
    - Critical
    - High
    - Medium
    
**Time Constraints:** 24/7  
**Configuration:**
    
    
    - **Channel:** email  
**Purpose:** Notifications for Low severity alerts, reports, and budget warnings for stakeholder awareness.  
**Applicable Severities:**
    
    - Low
    - Warning
    
**Time Constraints:** N/A  
**Configuration:**
    
    
    
  - **Routing Rules:**
    
    - **Condition:** Severity == 'Critical'  
**Severity:** Critical  
**Alert Type:** Any  
**Channels:**
    
    - pagerduty
    - slack
    
**Priority:** 1  
    - **Condition:** Severity == 'High'  
**Severity:** High  
**Alert Type:** Any  
**Channels:**
    
    - slack
    - email
    
**Priority:** 2  
    - **Condition:** Severity == 'Medium'  
**Severity:** Medium  
**Alert Type:** Any  
**Channels:**
    
    - slack
    
**Priority:** 3  
    - **Condition:** Severity == 'Low' || Severity == 'Warning'  
**Severity:** Low  
**Alert Type:** Any  
**Channels:**
    
    - email
    
**Priority:** 4  
    
  - **Time Based Routing:**
    
    
  - **Ticketing Integration:**
    
    
  - **Emergency Notifications:**
    
    
  - **Chat Platform Integration:**
    
    - **Platform:** slack  
**Channels:**
    
    - #backend-alerts
    - #client-alerts
    
**Bot Integration:** True  
**Thread Management:** Create a new thread for each distinct incident.  
    
  
- **Alert Correlation Implementation:**
  
  - **Grouping Requirements:**
    
    - **Grouping Criteria:** Cloud Function Name  
**Time Window:** 5m  
**Max Group Size:** 0  
**Suppression Strategy:** Group multiple failures from the same function into a single notification.  
    - **Grouping Criteria:** Crash Signature  
**Time Window:** 1h  
**Max Group Size:** 0  
**Suppression Strategy:** Group all occurrences of the same crash into one incident in Crashlytics.  
    
  - **Parent Child Relationships:**
    
    
  - **Topology Based Correlation:**
    
    
  - **Time Window Correlation:**
    
    
  - **Causal Relationship Detection:**
    
    
  - **Maintenance Window Suppression:**
    
    - **Maintenance Type:** Backend Deployment / Config Change  
**Suppression Scope:**
    
    - Firebase Cloud Functions
    - Firestore
    
**Automatic Detection:** False  
**Manual Override:** True  
    
  
- **False Positive Mitigation:**
  
  - **Noise Reduction Strategies:**
    
    - **Strategy:** Confirmation Count  
**Implementation:** Alert only after a threshold is breached for 'N' consecutive data points.  
**Applicable Alerts:**
    
    - Firestore p99 Latency
    - Cloud Function Error Rate
    
**Effectiveness:** High  
    
  - **Confirmation Counts:**
    
    - **Alert Type:** High Latency  
**Confirmation Threshold:** 3  
**Confirmation Window:** 5m  
**Reset Condition:** Metric returns below threshold for 1 data point.  
    
  - **Dampening And Flapping:**
    
    
  - **Alert Validation:**
    
    
  - **Smart Filtering:**
    
    
  - **Quorum Based Alerting:**
    
    
  
- **On Call Management Integration:**
  
  - **Escalation Paths:**
    
    - **Severity:** Critical  
**Escalation Levels:**
    
    - **Level:** 1  
**Recipients:**
    
    - Primary On-Call Engineer (Backend/Client)
    
**Escalation Time:** 15m  
**Requires Acknowledgment:** True  
    - **Level:** 2  
**Recipients:**
    
    - Secondary On-Call Engineer
    - Engineering Lead
    
**Escalation Time:** 15m  
**Requires Acknowledgment:** True  
    
**Ultimate Escalation:** Head of Engineering  
    - **Severity:** High  
**Escalation Levels:**
    
    - **Level:** 1  
**Recipients:**
    
    - Primary On-Call Engineer
    
**Escalation Time:** 30m  
**Requires Acknowledgment:** True  
    
**Ultimate Escalation:** Engineering Lead  
    
  - **Escalation Timeframes:**
    
    
  - **On Call Rotation:**
    
    - **Team:** Backend  
**Rotation Type:** weekly  
**Handoff Time:** Monday 10:00 UTC  
**Backup Escalation:** Secondary On-Call from the same team.  
    - **Team:** Client  
**Rotation Type:** weekly  
**Handoff Time:** Monday 10:00 UTC  
**Backup Escalation:** Secondary On-Call from the same team.  
    
  - **Acknowledgment Requirements:**
    
    - **Severity:** Critical  
**Acknowledgment Timeout:** 15m  
**Auto Escalation:** True  
**Requires Comment:** False  
    - **Severity:** High  
**Acknowledgment Timeout:** 30m  
**Auto Escalation:** True  
**Requires Comment:** False  
    
  - **Incident Ownership:**
    
    - **Assignment Criteria:** First responder to acknowledge owns the incident.  
**Ownership Transfer:** Must be explicitly transferred via PagerDuty/Slack command.  
**Tracking Mechanism:** PagerDuty Incident Details  
    
  - **Follow The Sun Support:**
    
    
  
- **Project Specific Alerts Config:**
  
  - **Alerts:**
    
    - **Name:** High Cloud Function Error Rate  
**Description:** Alerts when the error rate for any Cloud Function exceeds 5% over a 5-minute period.  
**Condition:** metric.name='cloudfunctions.googleapis.com/function/execution_count' AND metric.label.status='error' GROUP_BY 5m | rate > 0.05  
**Threshold:** > 5%  
**Severity:** High  
**Channels:**
    
    - slack
    - email
    
**Correlation:**
    
    - **Group Id:** function_errors_{{metric.label.function_name}}
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 30m
    - **Escalation Path:**
      
      - Primary On-Call Engineer (Backend)
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 2
    - **Confirmation Window:** 5m
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** https://internal-wiki.patterncipher.com/runbooks/cloud-function-errors
    - **Troubleshooting Steps:**
      
      - Check Google Cloud Logging for the failing function name.
      - Identify the specific error message and stack trace.
      - Correlate with recent deployments or Remote Config changes.
      
    
    - **Name:** Critical Client Crash Rate  
**Description:** Alerts when the crash-free user percentage for any app version drops below 99.5% in a 24-hour period.  
**Condition:** Firebase Crashlytics: Crash-Free Users < 99.5%  
**Threshold:** < 99.5%  
**Severity:** Critical  
**Channels:**
    
    - pagerduty
    - slack
    
**Correlation:**
    
    - **Group Id:** client_stability_{{app_version}}
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 15m
    - **Escalation Path:**
      
      - Primary On-Call Engineer (Client)
      
    
**Suppression:**
    
    - **Maintenance Window:** False
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 1
    - **Confirmation Window:** 24h
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** https://internal-wiki.patterncipher.com/runbooks/client-crash-rate
    - **Troubleshooting Steps:**
      
      - Open Firebase Crashlytics dashboard.
      - Filter by the affected app version and time period.
      - Analyze the top crash signatures to identify root cause.
      - Consider halting staged rollout if applicable.
      
    
    - **Name:** Firestore Security Rule Denial Spike  
**Description:** Alerts when there is a spike in Firestore security rule denials.  
**Condition:** metric.name='cloud.firestore.googleapis.com/security_rules/denied_requests_count' | rate > 0.5/s  
**Threshold:** > 20 per 10 minutes  
**Severity:** Critical  
**Channels:**
    
    - pagerduty
    - slack
    
**Correlation:**
    
    - **Group Id:** firestore_security
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 15m
    - **Escalation Path:**
      
      - Primary On-Call Engineer (Backend)
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 1
    - **Confirmation Window:** 10m
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** https://internal-wiki.patterncipher.com/runbooks/firestore-security
    - **Troubleshooting Steps:**
      
      - Review Firestore security rules in the Firebase console.
      - Analyze logs to identify the source and type of denied requests.
      - Correlate with recent client releases that may have incorrect data access patterns.
      
    
    - **Name:** Leaderboard Submission DLQ  
**Description:** Alerts if any event lands in the leaderboard submission dead letter queue.  
**Condition:** Firestore Collection Size('failed_leaderboard_submissions') > 0  
**Threshold:** > 0  
**Severity:** Medium  
**Channels:**
    
    - slack
    
**Correlation:**
    
    - **Group Id:** leaderboard_dlq
    - **Suppression Rules:**
      
      - Notify only once per hour if the count remains > 0
      
    
**Escalation:**
    
    - **Enabled:** False
    - **Escalation Time:** N/A
    - **Escalation Path:**
      
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 1
    - **Confirmation Window:** 1m
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** https://internal-wiki.patterncipher.com/runbooks/dlq-reprocessing
    - **Troubleshooting Steps:**
      
      - Inspect the failed message payload in the Firestore DLQ collection.
      - Identify the reason for the validation failure.
      - Fix the underlying bug or data issue.
      - Manually re-process the message if it's recoverable.
      
    
    
  - **Alert Groups:**
    
    
  - **Notification Templates:**
    
    - **Template Id:** slack_critical_template  
**Channel:** slack  
**Format:** {
  "blocks": [
    {
      "type": "section",
      "text": {
        "type": "mrkdwn",
        "text": ":rotating_light: *CRITICAL ALERT: {{alert.name}}* :rotating_light:"
      }
    },
    {
      "type": "section",
      "fields": [
        {
          "type": "mrkdwn",
          "text": "*Severity:*\n`{{alert.severity}}`"
        },
        {
          "type": "mrkdwn",
          "text": "*Time:*\n`{{alert.timestamp}} UTC`"
        }
      ]
    },
    {
      "type": "context",
      "elements": [
        {
          "type": "mrkdwn",
          "text": "*Description:* {{alert.description}}"
        }
      ]
    },
    {
      "type": "actions",
      "elements": [
        {
          "type": "button",
          "text": {
            "type": "plain_text",
            "text": "Acknowledge Incident",
            "emoji": true
          },
          "style": "primary",
          "value": "ack_{{incident.id}}"
        },
        {
          "type": "button",
          "text": {
            "type": "plain_text",
            "text": "View Runbook",
            "emoji": true
          },
          "url": "{{alert.runbookUrl}}"
        }
      ]
    }
  ]
}  
**Variables:**
    
    - alert.name
    - alert.severity
    - alert.timestamp
    - alert.description
    - incident.id
    - alert.runbookUrl
    
    
  
- **Implementation Priority:**
  
  - **Component:** Firebase Crashlytics Integration  
**Priority:** high  
**Dependencies:**
    
    
**Estimated Effort:** Low  
**Risk Level:** low  
  - **Component:** Cloud Function Error Rate Alerting  
**Priority:** high  
**Dependencies:**
    
    - Google Cloud Monitoring Setup
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** Firestore Security Rule Denial Alerting  
**Priority:** high  
**Dependencies:**
    
    - Google Cloud Monitoring Setup
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** PagerDuty & Slack Integration  
**Priority:** medium  
**Dependencies:**
    
    
**Estimated Effort:** Medium  
**Risk Level:** low  
  
- **Risk Assessment:**
  
  - **Risk:** Alert Fatigue  
**Impact:** high  
**Probability:** high  
**Mitigation:** Start with a very small set of high-signal, critical alerts. Aggressively tune thresholds and use correlation to reduce noise. Ensure every alert is actionable and has a runbook.  
**Contingency Plan:** Temporarily disable noisy alerts and schedule a post-mortem to re-evaluate its necessity and thresholds.  
  - **Risk:** Missed Critical Incidents  
**Impact:** high  
**Probability:** low  
**Mitigation:** Configure alerts for foundational metrics (errors, crashes, security denials). Regularly review monitoring dashboards to identify gaps in alerting. Conduct failure-injection testing (chaos engineering) to validate alert triggers.  
**Contingency Plan:** Perform a comprehensive post-mortem to understand why the alert failed to trigger and implement corrective actions for the monitoring configuration.  
  
- **Recommendations:**
  
  - **Category:** Process  
**Recommendation:** Mandate the creation of a runbook for every new alert configured.  
**Justification:** Ensures that alerts are actionable and reduces the Mean Time To Recovery (MTTR) by providing on-call engineers with clear, immediate steps for investigation and remediation.  
**Priority:** high  
**Implementation Notes:** Runbooks should be stored in a version-controlled wiki (e.g., Confluence, GitHub Wiki) and linked directly from the alert notification.  
  - **Category:** Configuration  
**Recommendation:** Use Terraform or another Infrastructure-as-Code (IaC) tool to manage Google Cloud Monitoring and Alerting policies.  
**Justification:** Treating alert configuration as code allows for version control, peer review, and automated deployment, reducing the risk of manual configuration errors and providing a clear audit trail.  
**Priority:** medium  
**Implementation Notes:** Integrate the IaC deployment into the main CI/CD pipeline for the backend.  
  


---

