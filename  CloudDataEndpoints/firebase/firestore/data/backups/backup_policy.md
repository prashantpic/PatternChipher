### Firestore Backup and Recovery Policy

**1. Policy Objective**
To ensure the durability and availability of all user data stored in Cloud Firestore for the Pattern Cipher application, and to establish clear objectives and procedures for data recovery in the event of a catastrophic data loss incident.

**2. Backup Strategy**
- The primary backup mechanism will be Cloud Firestore's built-in **Point-in-Time Recovery (PITR)** feature.
- PITR will be enabled for the production Firestore database, providing continuous backups and allowing for restoration to any microsecond within the last 7 days.

**3. Recovery Objectives**
- **Recovery Point Objective (RPO): 24 hours.** This is a worst-case scenario. With PITR, the RPO is effectively near-zero (minutes), but we set a 24-hour formal objective to account for detection and decision-making time.
- **Recovery Time Objective (RTO): 4 hours.** This is the target time from the moment a disaster is declared to the time the database is restored and operational.

**4. Recovery Procedure (High-Level)**
1.  **Incident Declaration:** The operations lead declares a disaster recovery event.
2.  **PITR Restore:** Using the Google Cloud Console or gcloud CLI, initiate a restore of the production database to a new database instance from a specific timestamp just prior to the data corruption event.
3.  **Validation:** Perform integrity checks on the restored data to ensure its validity.
4.  **Traffic Switch:** Update application configuration to point to the newly restored database instance.
5.  **Post-Mortem:** Conduct a post-mortem analysis to determine the root cause and prevent future occurrences.

**5. Validation**
- A simulated recovery drill will be conducted semi-annually to validate the documented procedure and confirm that the RTO can be met.