sequenceDiagram
  actor "Administrator / CI/CD Pipeline" as ADMIN_CI_CD
  participant RCI as "<<Infrastructure>>\nCloud Infrastructure & Tooling"
  participant RBA as "<<Application>>\nBackend API Services"
  participant RDD as "<<DataStore>>\nDomain Data Services"

  ADMIN_CI_CD->>+RCI: Initiate Infrastructure Change (e.g., terraform apply)
  note over ADMIN_CI_CD, RCI: Administrator or CI/CD pipeline initiates infrastructure changes using Terraform (REQ-8-008).
  RCI-->>ADMIN_CI_CD: Apply Process Initiated

  RCI->>+RCI: Provision/Update Cloud Resources (VMs, Network, LB, IAM) via Cloud Provider APIs
  note right of RCI: Terraform (within Cloud Infrastructure) interacts with Cloud Provider APIs to orchestrate resource provisioning and configuration.
  RCI-->>-RCI: Resource Provisioning Status

  RCI->>+RDD: Provision/Update MongoDB Cluster Resources
  RDD-->>-RCI: MongoDB Resources Status

  RCI->>+RDD: Provision/Update Redis Cache Resources
  RDD-->>-RCI: Redis Resources Status

  RCI->>+RBA: Deploy/Update Backend Service Instances
  note right of RCI: Backend API deployment includes application code and necessary configurations to newly provisioned or updated compute instances.
  RBA-->>-RCI: Deployment Status

  RCI->>+RBA: Verify Backend Service & Dependencies Post-Deployment
  note over RBA: Comprehensive health checks validate service functionality and connectivity to dependencies like DB (MongoDB) and Cache (Redis).
  RBA->>+RDD: Check MongoDB Connection
  RDD-->>-RBA: MongoDB Connection OK/Fail
  RBA->>+RDD: Verify Redis Cache Connection (Redis cache deployment verification)
  note over RBA,RDD: Redis cache deployment verification is part of the post-deployment checks, ensuring Backend API can connect and use Redis.
  RDD-->>-RBA: Redis Connection OK/Fail
  RBA->>+RBA: Perform Internal Sanity Checks
  RBA-->>-RBA: Internal Checks OK/Fail
  RBA-->>-RCI: Overall Health Status (OK/Fail)

  RCI->>ADMIN_CI_CD: Async: Report Deployment Outcome (Success/Failure)
  note over RCI: Deployment failures (indicated by health checks) trigger alerts and may initiate automated rollback procedures (rollback not detailed).
  deactivate RCI %% Deactivate from initial trigger's main flow

  loop Perform Periodic Health Checks (Backend service health checks)
    activate RCI
    note right of RCI: Continuous health checks (Backend service health checks) by Load Balancers/Monitoring systems ensure service availability (REQ-8-008).
    RCI->>+RBA: Request Health Status (/health endpoint)
    RBA->>+RDD: Check DB Connection
    RDD-->>-RBA: DB Status OK/Fail
    RBA->>+RDD: Check Cache Connection
    RDD-->>-RBA: Cache Status OK/Fail
    RBA-->>-RCI: Health Status (Healthy/Unhealthy)
    RCI->>+RCI: Update Load Balancer/Service Discovery based on Health Status
    note right of RCI: If a backend instance is unhealthy, the Load Balancer/Service Discovery removes it from rotation.
    RCI-->>-RCI: Update Acknowledged
    deactivate RCI
  end

  loop Monitor MongoDB Metrics (for Auto-Scaling)
    activate RCI
    RCI->>+RDD: Read MongoDB Performance Metrics
    RDD-->>-RCI: Metrics Data
    alt MongoDB Auto-Scaling Decision Logic
      activate RCI
      RCI->>RCI: If Metrics exceed Threshold (e.g., High CPU/Connections)
      activate RCI
      RCI->>+RDD: Trigger MongoDB Cluster Auto-Scaling (MongoDB cluster auto-scaling triggers)
      note right of RCI: MongoDB cluster auto-scales based on predefined metrics (e.g., CPU, connections) ensuring performance and availability (REQ-8-024).
      RDD-->>-RCI: Scaling Operation Status
      activate RDD
      RDD->>RDD: New instances join cluster, data rebalances
      deactivate RDD
      RCI->>+RDD: Verify Cluster Stability Post-Scaling
      RDD-->>-RCI: Stability Status OK/Fail
      deactivate RCI
    else Else (Metrics within normal range)
      activate RCI
      RCI->>RCI: Continue Monitoring (No Action)
      deactivate RCI
    end
    deactivate RCI
    deactivate RCI
  end
  note over RCI: Similar auto-scaling mechanisms can be configured for Backend API instances and Redis Cache based on their respective metrics and policies.