# System Architecture Documentation

## Components

### Identity and Access Management Service
- **ID**: identity-access-service
- **Description**: Handles user authentication, authorization, registration workflows, role and permission management, password policies, MFA, and session management. Consumes centrally managed password policies if overridden by Admin.
- **Type**: Service
- **Dependencies**: notification-service, master-data-service
- **Properties**:
  - Domain Focus: User Identity, Authentication, Authorization
  - Data Sensitivity: High (Credentials, PII)
  - Key Features: User Registration, Login, RBAC, MFA, Password Management
- **Interfaces**:
  - POST /api/v1/auth/register
  - POST /api/v1/auth/login
  - POST /api/v1/auth/logout
  - POST /api/v1/auth/refresh-token
  - POST /api/v1/auth/recover-password
  - POST /api/v1/auth/reset-password
  - GET /api/v1/users/{userId}
  - PUT /api/v1/users/{userId}
  - GET /api/v1/users/me/profile
  - PUT /api/v1/users/me/password
  - POST /api/v1/users/me/mfa/setup
  - POST /api/v1/users/me/mfa/verify
  - GET /api/v1/admin/users
  - POST /api/v1/admin/users
  - PUT /api/v1/admin/users/{userId}/status
  - GET /api/v1/admin/roles
  - POST /api/v1/admin/roles
  - PUT /api/v1/admin/roles/{roleId}
  - GET /api/v1/admin/permissions
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Security, Spring Data JPA
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Small
  - Network: Medium
- **Configuration**:
  - jwt_secret_key_ref: secrets.jwt.secret
  - jwt_issuer: thesss.platform
  - jwt_access_token_expiration_ms: 3600000
  - jwt_refresh_token_expiration_ms: 86400000
  - password_policy_min_length: 8
  - password_policy_complexity_regex: ^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$
  - otp_length: 6
  - otp_validity_seconds: 300
  - db_connection_uri: env:DB_IAM_URI
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-FRM-016, REQ-FRM-017, REQ-FRM-022, REQ-FRM-023, REQ-FRM-024, REQ-FRM-025, REQ-2-018, REQ-2-021, REQ-5-001, REQ-5-002, REQ-5-003, REQ-5-004, REQ-5-005, REQ-5-006, REQ-5-007, REQ-5-008, REQ-5-009, REQ-5-010, REQ-5-011, REQ-5-012, REQ-5-013, REQ-5-014, REQ-5-015, REQ-5-016, REQ-5-017, REQ-5-018, REQ-5-019, REQ-5-020, REQ-5-021, REQ-12-002, REQ-13-007, REQ-DGC-002, REQ-DGC-004, REQ-DGC-005
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT, ROLE_SYSTEM

### Farmer Registry Service
- **ID**: farmer-registry-service
- **Description**: Manages farmer profiles, including personal details, contact information, national ID, bank details, family size, experience, consent, status, and approval history.
- **Type**: Service
- **Dependencies**: master-data-service, notification-service, identity-access-service
- **Properties**:
  - Domain Focus: Farmer Profile Management
  - Data Sensitivity: Very High (PII, Financial Info)
  - Key Features: Farmer CRUD, Consent Management, Approval Workflows
- **Interfaces**:
  - POST /api/v1/farmers
  - GET /api/v1/farmers
  - GET /api/v1/farmers/{farmerId}
  - PUT /api/v1/farmers/{farmerId}
  - GET /api/v1/farmers/{farmerId}/address
  - PUT /api/v1/farmers/{farmerId}/address
  - GET /api/v1/farmers/{farmerId}/bank-details
  - PUT /api/v1/farmers/{farmerId}/bank-details
  - GET /api/v1/farmers/{farmerId}/consent
  - POST /api/v1/farmers/{farmerId}/consent
  - GET /api/v1/farmers/{farmerId}/organizations
  - POST /api/v1/farmers/{farmerId}/organizations
  - GET /api/v1/farmers/{farmerId}/approval-history
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Data JPA
- **Resources**:
  - CPU: Medium
  - Memory: Large
  - Storage: Medium-Large
  - Network: Medium
- **Configuration**:
  - db_connection_uri: env:DB_FARMER_REGISTRY_URI
  - encryption_key_ref_national_id: secrets.encryption.national_id
  - encryption_key_ref_bank_account: secrets.encryption.bank_account
  - profile_photo_storage_bucket: farmer-profile-photos
  - default_farmer_status_on_creation: Pending Approval
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-FRM-001, REQ-FRM-002, REQ-FRM-003, REQ-FRM-004, REQ-FRM-005, REQ-FRM-006, REQ-FRM-007, REQ-FRM-008, REQ-FRM-009, REQ-FRM-010, REQ-FRM-011, REQ-FRM-012, REQ-FRM-013, REQ-FRM-014, REQ-FRM-015, REQ-FRM-016, REQ-FRM-017, REQ-FRM-018, REQ-FRM-019, REQ-FRM-020, REQ-FRM-021, REQ-FRM-022, REQ-FRM-023, REQ-FRM-024, REQ-FRM-025, REQ-12-006, REQ-DGC-004, REQ-DGC-005, REQ-DGC-008, REQ-DGC-012, REQ-DGC-013
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT_WITH_CONSENT

### Farm Land Service
- **ID**: farm-land-service
- **Description**: Manages farm land records, including parcel details, area, ownership, soil information, irrigation, topography, elevation, and geospatial boundaries using PostGIS.
- **Type**: Service
- **Dependencies**: master-data-service, farmer-registry-service, identity-access-service
- **Properties**:
  - Domain Focus: Farm Land Data, Geospatial Management
  - Data Sensitivity: Medium
  - Key Features: Land Record CRUD, GPS Data Handling, Area Calculation
- **Interfaces**:
  - POST /api/v1/farmlands
  - GET /api/v1/farmlands
  - GET /api/v1/farmlands/{landId}
  - PUT /api/v1/farmlands/{landId}
  - POST /api/v1/farmlands/{landId}/geospatial
  - GET /api/v1/farmlands/{landId}/geospatial
  - GET /api/v1/farmlands/farmer/{farmerId}
  - POST /api/v1/farmlands/{landId}/documents
  - GET /api/v1/farmlands/{landId}/soil-tests
  - POST /api/v1/farmlands/{landId}/soil-tests
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Data JPA, PostGIS, JTS
- **Resources**:
  - CPU: High
  - Memory: Large
  - Storage: Large
  - Network: Medium
- **Configuration**:
  - db_connection_uri: env:DB_FARM_LAND_URI
  - land_document_storage_bucket: farm-land-documents
  - gps_default_srid: 4326
  - area_discrepancy_threshold_percentage: 10
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 10
- **Responsible Features**: REQ-2-001, REQ-2-002, REQ-2-003, REQ-2-004, REQ-2-005, REQ-2-006, REQ-2-007, REQ-2-008, REQ-2-009, REQ-2-010, REQ-2-011, REQ-2-012, REQ-2-013, REQ-2-014, REQ-2-015, REQ-2-016, REQ-2-017, REQ-2-018, REQ-2-019, REQ-2-020, REQ-2-021, REQ-1.3-001, REQ-1.3-002, REQ-1.3-003, REQ-1.3-004, REQ-1.3-005, REQ-1.3-006, REQ-1.3-007, REQ-1.3-008, REQ-1.3-009, REQ-1.3-010, REQ-1.3-011, REQ-1.3-012, REQ-DGC-012, REQ-DGC-013
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT_WITH_CONSENT

### Crop Management Service
- **ID**: crop-management-service
- **Description**: Manages crop cultivation cycles, including sowing/harvest dates, cultivated area, activities, inputs used, yield, and market sales information.
- **Type**: Service
- **Dependencies**: master-data-service, farm-land-service, farmer-registry-service, identity-access-service
- **Properties**:
  - Domain Focus: Crop Cultivation Tracking
  - Data Sensitivity: Medium
  - Key Features: Crop Cycle CRUD, Activity Logging, Yield Calculation
- **Interfaces**:
  - POST /api/v1/crop-cycles
  - GET /api/v1/crop-cycles
  - GET /api/v1/crop-cycles/{cropCycleId}
  - PUT /api/v1/crop-cycles/{cropCycleId}
  - GET /api/v1/farmlands/{landId}/crop-cycles
  - POST /api/v1/crop-cycles/{cropCycleId}/activities
  - POST /api/v1/crop-cycles/{cropCycleId}/inputs
  - POST /api/v1/crop-cycles/{cropCycleId}/yield
  - POST /api/v1/crop-cycles/{cropCycleId}/sales
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Data JPA
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Medium-Large
  - Network: Medium
- **Configuration**:
  - db_connection_uri: env:DB_CROP_MGMT_URI
  - default_crop_cycle_status: Planned
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-4-001, REQ-4-002, REQ-4-003, REQ-4-004, REQ-4-005, REQ-4-006, REQ-4-007, REQ-4-008, REQ-4-009, REQ-4-010, REQ-4-011, REQ-DGC-012, REQ-DGC-013
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT_WITH_CONSENT

### Master Data Management Service
- **ID**: master-data-service
- **Description**: Manages all shared master data lists, such as crop types, soil types, units of measure, statuses, etc. Provides CRUD operations for Admin users and lookup for other services. Includes unit conversion capabilities.
- **Type**: Service
- **Dependencies**: []
- **Properties**:
  - Domain Focus: Centralized Master Data, Unit of Measure Management
  - Data Sensitivity: Low-Medium
  - Key Features: CRUD for Master Lists, Versioning, Unit Conversion
- **Interfaces**:
  - GET /api/v1/masterdata/{listName}
  - POST /api/v1/admin/masterdata/{listName}
  - PUT /api/v1/admin/masterdata/{listName}/{itemId}
  - DELETE /api/v1/admin/masterdata/{listName}/{itemId}
  - GET /api/v1/masterdata/units
  - POST /api/v1/admin/masterdata/units
  - GET /api/v1/masterdata/convert-unit
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Data JPA
- **Resources**:
  - CPU: Low-Medium
  - Memory: Medium
  - Storage: Small-Medium
  - Network: Low-Medium
- **Configuration**:
  - db_connection_uri: env:DB_MASTER_DATA_URI
  - cache_ttl_seconds_master_lists: 3600
- **Health Check**:
  - Path: /actuator/health
  - Interval: 60
  - Timeout: 5
- **Responsible Features**: REQ-FRM-005, REQ-FRM-010, REQ-FRM-013, REQ-FRM-015, REQ-FRM-018, REQ-2-007, REQ-2-010, REQ-2-013, REQ-2-014, REQ-2-015, REQ-2-019, REQ-4-002, REQ-4-004, REQ-4-005, REQ-4-006, REQ-4-007, REQ-MDM-001, REQ-MDM-002, REQ-MDM-003, REQ-MDM-004, REQ-MDM-005, REQ-MDM-006, REQ-MDM-007, REQ-MDM-008, REQ-MDM-009, REQ-UOM-001, REQ-UOM-002, REQ-UOM-003, REQ-UOM-004, REQ-UOM-005, REQ-UOM-006, REQ-UOM-007, REQ-UOM-008, REQ-UOM-009, REQ-UOM-010
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT, ROLE_SYSTEM

### Notification Service
- **ID**: notification-service
- **Description**: Handles sending system-wide notifications via multiple channels (In-App, SMS, Email) using third-party integrations. Manages notification templates and OTP delivery.
- **Type**: Service
- **Dependencies**: identity-access-service
- **Properties**:
  - Domain Focus: User Communications
  - Data Sensitivity: Medium (Contact details if logged)
  - Key Features: Multi-channel delivery, Template Management, OTP Delivery
- **Interfaces**:
  - POST /api/v1/notifications/send
  - POST /api/v1/admin/notifications/templates
  - GET /api/v1/admin/notifications/templates
  - GET /api/v1/users/me/notifications
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Cloud Stream/AMQP
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Small (for templates, logs)
  - Network: High
- **Configuration**:
  - sms_provider_api_key_ref: secrets.sms.twilio_api_key
  - sms_provider_account_sid_ref: secrets.sms.twilio_account_sid
  - sms_provider_from_number: +1234567890
  - email_provider_api_key_ref: secrets.email.sendgrid_api_key
  - email_provider_from_address: noreply@thesss.platform
  - fcm_server_key_ref: secrets.fcm.server_key
  - db_connection_uri: env:DB_NOTIFICATION_URI
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-WF-003, REQ-10-005, REQ-10-024, REQ-15-001, REQ-15-002, REQ-15-003, REQ-15-004, REQ-15-005, REQ-15-006, REQ-15-007, REQ-15-008, REQ-15-009, REQ-15-010, REQ-15-011, REQ-15-012, REQ-15-013, REQ-15-014, REQ-15-015, REQ-15-016, REQ-5-001
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_SYSTEM, ROLE_ADMIN

### Weather Service
- **ID**: weather-service
- **Description**: Integrates with third-party weather APIs to provide current weather, forecasts, and alerts. Implements caching for weather data.
- **Type**: Service
- **Dependencies**: notification-service, farm-land-service
- **Properties**:
  - Domain Focus: Weather Information Provision
  - Data Sensitivity: Low (Location for forecast)
  - Key Features: Forecast Retrieval, Severe Alert Processing, Caching
- **Interfaces**:
  - GET /api/v1/weather/current
  - GET /api/v1/weather/forecast/hourly
  - GET /api/v1/weather/forecast/daily
  - GET /api/v1/weather/alerts
  - GET /api/v1/weather/historical
- **Technology**: Java 21 LTS, Spring Boot 3.2.x
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Small (cache if disk-based)
  - Network: High
- **Configuration**:
  - weather_api_provider_name: OpenWeatherMap
  - weather_api_key_ref: secrets.weather.openweathermap_api_key
  - weather_api_base_url: https://api.openweathermap.org/data/2.5/
  - cache_ttl_current_weather_seconds: 900
  - cache_ttl_daily_forecast_seconds: 3600
- **Health Check**:
  - Path: /actuator/health
  - Interval: 60
  - Timeout: 10
- **Responsible Features**: REQ-WF-001, REQ-WF-002, REQ-WF-003, REQ-WF-004, REQ-WF-005, REQ-WF-006, REQ-WF-007, REQ-WF-008, REQ-WF-009, REQ-12-005
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT

### Query Management Service
- **ID**: query-management-service
- **Description**: Manages farming queries submitted by farmers, consultant responses, query lifecycle, SLAs, and the public Knowledge Base (KB).
- **Type**: Service
- **Dependencies**: notification-service, identity-access-service, farmer-registry-service, master-data-service
- **Properties**:
  - Domain Focus: Farming Advisory, Knowledge Sharing
  - Data Sensitivity: Medium (Query content)
  - Key Features: Query Submission/Response, KB Management, SLA Tracking
- **Interfaces**:
  - POST /api/v1/queries
  - GET /api/v1/queries
  - GET /api/v1/queries/{queryId}
  - POST /api/v1/queries/{queryId}/responses
  - PUT /api/v1/queries/{queryId}/status
  - POST /api/v1/queries/{queryId}/feedback
  - GET /api/v1/knowledgebase/articles
  - GET /api/v1/knowledgebase/articles/{articleId}
  - POST /api/v1/admin/knowledgebase/articles
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, Spring Data JPA
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Medium
  - Network: Medium
- **Configuration**:
  - db_connection_uri: env:DB_QUERY_MGMT_URI
  - query_attachment_storage_bucket: query-attachments
  - sla_default_response_hours_critical: 4
  - sla_default_response_hours_high: 8
  - query_reopen_window_days: 7
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-10-001, REQ-10-002, REQ-10-003, REQ-10-004, REQ-10-005, REQ-10-006, REQ-10-007, REQ-10-008, REQ-10-009, REQ-10-010, REQ-10-011, REQ-10-012, REQ-10-013, REQ-10-014, REQ-10-015, REQ-10-016, REQ-10-017, REQ-10-018, REQ-10-019, REQ-10-020, REQ-10-021, REQ-10-022, REQ-10-023, REQ-10-024, REQ-10-025, REQ-10-026, REQ-10-027, REQ-12-004, REQ-5-016, REQ-5-018, REQ-5-019
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARMER, ROLE_CONSULTANT

### Dashboard Service
- **ID**: dashboard-service
- **Description**: Aggregates data from various services to provide role-specific dashboards (Farmer, Admin, Farm Plot Admin, Consultant). May use materialized views or real-time aggregation. Supports data export.
- **Type**: Service
- **Dependencies**: farmer-registry-service, farm-land-service, crop-management-service, query-management-service, identity-access-service
- **Properties**:
  - Domain Focus: Data Visualization and Summarization
  - Data Sensitivity: Medium (Aggregated data)
  - Key Features: Role-Specific Data Aggregation, KPI Calculation, Report Generation Support
- **Interfaces**:
  - GET /api/v1/dashboards/farmer
  - GET /api/v1/dashboards/admin
  - GET /api/v1/dashboards/farm-plot-admin
  - GET /api/v1/dashboards/consultant
  - GET /api/v1/dashboards/{dashboardName}/export
- **Technology**: Java 21 LTS, Spring Boot 3.2.x, iText/Apache POI (for exports)
- **Resources**:
  - CPU: High
  - Memory: Large
  - Storage: Small (if relying on other DBs)
  - Network: Medium
- **Configuration**:
  - db_connection_uri_read_replicas: env:DB_READ_REPLICA_URI
  - materialized_view_refresh_cron_farmer: 0 0 * * * ?
  - materialized_view_refresh_cron_admin: 0 */5 * * * ?
- **Health Check**:
  - Path: /actuator/health
  - Interval: 60
  - Timeout: 10
- **Responsible Features**: REQ-DASH-001, REQ-DASH-002, REQ-DASH-003, REQ-DASH-004, REQ-DASH-005, REQ-DASH-006, REQ-DASH-007, REQ-DASH-008, REQ-DASH-009, REQ-DASH-010, REQ-DASH-011, REQ-DASH-012, REQ-DASH-013, REQ-DASH-014, REQ-DASH-015, REQ-DASH-016, REQ-DASH-017, REQ-DASH-018, REQ-DASH-019, REQ-DASH-020, REQ-DASH-021, REQ-DASH-022, REQ-DASH-023, REQ-DASH-024, REQ-DASH-025, REQ-DASH-026, REQ-DASH-027, REQ-10-025, REQ-10-026, REQ-10-027
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_FARM_PLOT_ADMIN, ROLE_FARMER, ROLE_CONSULTANT

### Administrative Configuration Service
- **ID**: admin-configuration-service
- **Description**: Provides APIs for Admin users to manage system-wide configurations, settings, and policies (e.g., notification templates, API limits, GPS thresholds, security policies, feature flags). Interacts with the underlying configuration store (e.g., Spring Cloud Config backend).
- **Type**: Service
- **Dependencies**: identity-access-service
- **Properties**:
  - Domain Focus: System Configuration Management
  - Data Sensitivity: Medium (System settings)
  - Key Features: CRUD for Configurations, Policy Management
- **Interfaces**:
  - GET /api/v1/admin/config/{configKey}
  - PUT /api/v1/admin/config/{configKey}
  - GET /api/v1/admin/config/feature-flags
  - PUT /api/v1/admin/config/feature-flags/{flagName}
  - GET /api/v1/admin/config/password-policy
  - PUT /api/v1/admin/config/password-policy
- **Technology**: Java 21 LTS, Spring Boot 3.2.x
- **Resources**:
  - CPU: Low
  - Memory: Low
  - Storage: Small (if it caches or mirrors some configs)
  - Network: Low
- **Configuration**:
  - config_store_type: SpringCloudConfigServer
  - config_store_uri: http://config-server:8888
- **Health Check**:
  - Path: /actuator/health
  - Interval: 60
  - Timeout: 5
- **Responsible Features**: REQ-8-001, REQ-8-002, REQ-8-003, REQ-8-004, REQ-8-005, REQ-8-006, REQ-8-007, REQ-8-008, REQ-8-009, REQ-8-010, REQ-8-011, REQ-DGC-001, REQ-DGC-002, REQ-DGC-006, REQ-DGC-007, REQ-DGC-010, REQ-DGC-012, REQ-DGC-013
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN

### Synchronization Orchestration Service
- **ID**: synchronization-orchestration-service
- **Description**: Manages the backend logic for bi-directional data synchronization with mobile clients. Handles conflict detection rules and orchestrates data updates with relevant services.
- **Type**: Service
- **Dependencies**: farmer-registry-service, farm-land-service, crop-management-service, master-data-service, identity-access-service
- **Properties**:
  - Domain Focus: Offline Data Synchronization
  - Data Sensitivity: High (Handles various data types during sync)
  - Key Features: Conflict Detection, Data Merging, Sync Process Management
- **Interfaces**:
  - POST /api/v1/sync/upload
  - GET /api/v1/sync/download
  - POST /api/v1/sync/conflicts/{conflictId}/resolve
- **Technology**: Java 21 LTS, Spring Boot 3.2.x
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Small (transaction logs)
  - Network: Medium
- **Configuration**:
  - default_conflict_resolution_strategy: MANUAL_INTERVENTION_REQUIRED
  - max_batch_size_sync_upload: 100
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-14-002, REQ-14-004, REQ-14-005, REQ-14-006, REQ-14-007, REQ-14-010
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_FARMER, ROLE_FARM_PLOT_ADMIN, ROLE_ADMIN

### Financial Integration Service
- **ID**: financial-integration-service
- **Description**: Handles integration with external financial systems or payment gateways for subsidy disbursal or payment tracking, if in scope. Manages secure communication and data exchange.
- **Type**: Service
- **Dependencies**: farmer-registry-service, notification-service, identity-access-service
- **Properties**:
  - Domain Focus: Financial Transactions
  - Data Sensitivity: Very High (Financial transaction data)
  - Key Features: Payment Initiation, Transaction Tracking, Reconciliation Support
- **Interfaces**:
  - POST /api/v1/financial/payments/initiate
  - GET /api/v1/financial/payments/{paymentId}/status
  - POST /api/v1/financial/reconciliation/reports
- **Technology**: Java 21 LTS, Spring Boot 3.2.x
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Medium (transaction logs)
  - Network: Medium-High
- **Configuration**:
  - payment_gateway_api_url: env:PAYMENT_GW_URL
  - payment_gateway_api_key_ref: secrets.payment_gw.api_key
  - sftp_host_financial_system: env:FIN_SFTP_HOST
  - sftp_user_financial_system: env:FIN_SFTP_USER
- **Health Check**:
  - Path: /actuator/health
  - Interval: 60
  - Timeout: 10
- **Responsible Features**: REQ-FRM-011, REQ-13-001, REQ-13-002, REQ-13-003, REQ-13-004, REQ-13-005, REQ-13-006, REQ-13-007
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN, ROLE_SYSTEM

### API Gateway Component
- **ID**: api-gateway-component
- **Description**: Single entry point for all client requests. Routes requests to backend microservices, handles authentication, rate limiting, SSL termination, and serves as the host for the Public API.
- **Type**: Gateway
- **Dependencies**: identity-access-service, farmer-registry-service, farm-land-service, crop-management-service, master-data-service, notification-service, weather-service, query-management-service, dashboard-service, admin-configuration-service, synchronization-orchestration-service, financial-integration-service
- **Properties**:
  - Routing Strategy: Path-based, Service ID based
  - Security Enforcement Point: True
  - Public API Host: True
- **Interfaces**:
  - All client-facing API endpoints, e.g., /api/v1/farmers/*, /publicapi/v1/stats/*
- **Technology**: Spring Cloud Gateway
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Minimal (logs)
  - Network: High
- **Configuration**:
  - service_discovery_uri: http://service-discovery-registry:8761/eureka/
  - default_timeout_ms: 10000
  - global_cors_config_ref: config.global_cors
  - rate_limit_per_route_config: config.rate_limits
  - jwt_public_key_uri_for_validation: http://identity-access-service/auth/token_keys
- **Health Check**:
  - Path: /actuator/health
  - Interval: 15
  - Timeout: 3
- **Responsible Features**: REQ-12-001, REQ-12-002, REQ-12-003, REQ-12-004, REQ-12-005, REQ-12-006, REQ-12-007, Implicitly supports all features requiring client-backend communication.
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: Varies per route based on downstream service requirements

### Web Application Client (SPA)
- **ID**: web-app-client
- **Description**: Single Page Application providing the user interface for web browsers. Interacts with the API Gateway for all backend operations. Manages client-side state and rendering.
- **Type**: Controller
- **Dependencies**: api-gateway-component
- **Properties**:
  - Client Type: Web Browser SPA
  - UI Framework: React
- **Interfaces**:
  - User Interface for all features accessible via web.
- **Technology**: React 18.x, TypeScript, Redux Toolkit, Material-UI/Ant Design, Leaflet, Chart.js
- **Resources**:
  - CPU: Client-Side
  - Memory: Client-Side
  - Storage: Client-Side (localStorage, sessionStorage)
  - Network: Client-Side
- **Configuration**:
  - api_gateway_base_url: env:API_GATEWAY_URL
  - map_tile_server_url: https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png
  - client_side_log_level: INFO
  - google_analytics_id: env:GA_ID
- **Health Check**: null
- **Responsible Features**: REQ-FRM-001 to REQ-FRM-025 (UI), REQ-2-001 to REQ-2-021 (UI), REQ-1.3-001 to REQ-1.3-009 (UI), REQ-4-001 to REQ-4-011 (UI), REQ-5-001 to REQ-5-021 (UI), REQ-MDM-001 to REQ-MDM-009 (Admin UI), REQ-UOM-001 to REQ-UOM-010 (Admin UI), REQ-8-001 to REQ-8-010 (Admin UI), REQ-WF-001 to REQ-WF-009 (UI), REQ-10-001 to REQ-10-027 (UI), REQ-DASH-001 to REQ-DASH-027 (UI), REQ-DGC-003 (Admin UI), REQ-DGC-004 (Admin UI for DSAR), REQ-DGC-009 (Admin UI for audit access), REQ-17-001 to REQ-17-009 (Admin UI for viewing logs/metrics via integrated tools)
- **Security**:
  - Requires Authentication: false
  - Requires Authorization: false
  - Allowed Roles: []

### Mobile Application Client
- **ID**: mobile-app-client
- **Description**: Native or cross-platform mobile application for iOS and Android. Provides user interface, offline data capture capabilities, GPS integration, and synchronization with the backend via API Gateway.
- **Type**: Controller
- **Dependencies**: api-gateway-component
- **Properties**:
  - Client Type: Mobile (iOS, Android)
  - UI Framework: React Native
  - Offline Support: True
- **Interfaces**:
  - User Interface for features accessible via mobile, including offline variants.
- **Technology**: React Native, SQLite, React Native Maps, React Native Geolocation
- **Resources**:
  - CPU: Client-Side
  - Memory: Client-Side
  - Storage: Client-Side (Local SQLite DB)
  - Network: Client-Side
- **Configuration**:
  - api_gateway_base_url: env:API_GATEWAY_URL
  - sync_interval_background_ms: 3600000
  - local_db_encryption_key_ref: device_secure_storage.db_key
  - gps_accuracy_desired_meters: 10
- **Health Check**: null
- **Responsible Features**: REQ-FRM-001 to REQ-FRM-025 (Mobile UI), REQ-2-001 to REQ-2-021 (Mobile UI), REQ-1.3-001 to REQ-1.3-010 (Mobile UI & Capture), REQ-4-001 to REQ-4-011 (Mobile UI), REQ-5-001 to REQ-5-003 (Mobile UI), REQ-WF-001 to REQ-WF-005 (Mobile UI), REQ-10-001 to REQ-10-009 (Mobile UI), REQ-DASH-001 to REQ-DASH-008 (Mobile UI), REQ-14-001, REQ-14-002, REQ-14-003, REQ-14-004, REQ-14-005, REQ-14-006, REQ-14-007, REQ-14-008, REQ-14-009
- **Security**:
  - Requires Authentication: false
  - Requires Authorization: false
  - Allowed Roles: []

### Service Discovery Registry
- **ID**: service-discovery-registry
- **Description**: Provides a registry for microservices to register themselves and for other services to discover them. Essential for dynamic scaling and resilience in the microservice architecture.
- **Type**: PlatformService
- **Dependencies**: []
- **Properties**:
  - Availability: High
  - Consistency: AP (Typically, e.g., Eureka)
- **Interfaces**:
  - POST /eureka/apps/{appName}
  - GET /eureka/apps
  - GET /eureka/apps/{appName}
  - GET /eureka/apps/{appName}/{instanceId}
- **Technology**: Netflix Eureka
- **Resources**:
  - CPU: Low-Medium
  - Memory: Medium
  - Storage: Small (for registry data, potentially in-memory with replication)
  - Network: Medium
- **Configuration**:
  - server_port: 8761
  - enable_self_preservation: true
  - peer_eureka_nodes_urls: env:EUREKA_PEER_URLS
- **Health Check**:
  - Path: /actuator/health
  - Interval: 15
  - Timeout: 5
- **Responsible Features**: Implicitly supports all microservices by enabling dynamic service discovery and load balancing.
- **Security**:
  - Requires Authentication: false
  - Requires Authorization: false
  - Allowed Roles: []

### Centralized Configuration Server
- **ID**: config-server
- **Description**: Provides centralized externalized configuration management for all microservices. Supports dynamic configuration updates and secure storage of configuration properties.
- **Type**: PlatformService
- **Dependencies**: []
- **Properties**:
  - Config Backend Type: Git, Vault, JDBC, FileSystem
  - Dynamic Refresh Support: True
- **Interfaces**:
  - GET /{application}/{profile}[/{label}]
  - GET /{application}-{profile}.[yml|yaml|properties|json]
  - POST /actuator/busrefresh (if Spring Cloud Bus is used for refresh)
- **Technology**: Spring Cloud Config Server
- **Resources**:
  - CPU: Low-Medium
  - Memory: Medium
  - Storage: Small (caches configurations, main storage in backend e.g. Git)
  - Network: Medium
- **Configuration**:
  - server_port: 8888
  - git_uri: env:CONFIG_GIT_REPO_URI
  - git_search_paths: {application}
  - encrypt_key_ref: secrets.config_server.encrypt_key
- **Health Check**:
  - Path: /actuator/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-8-011 (as the backend providing configurations), Implicitly supports all microservices by centralizing their configurations.
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_SERVICE_CONFIG_CLIENT

### Message Broker
- **ID**: message-broker
- **Description**: Facilitates asynchronous communication between microservices using message queues and topics. Supports event-driven architecture patterns, ensuring reliable message delivery.
- **Type**: PlatformService
- **Dependencies**: []
- **Properties**:
  - Messaging Protocols: AMQP 0-9-1
  - Persistence: Configurable (Transient, Persistent)
  - Clustering Support: True
- **Interfaces**:
  - AMQP Protocol Interface (for publishing/consuming messages)
- **Technology**: RabbitMQ
- **Resources**:
  - CPU: Medium-High
  - Memory: Medium-Large
  - Storage: Medium-Large (for persistent messages)
  - Network: High
- **Configuration**:
  - amqp_port: 5672
  - management_plugin_port: 15672
  - default_vhost: /
  - cluster_formation_method: dns or static list
- **Health Check**:
  - Path: /api/aliveness-test/%2F
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-15-013 (as the underlying broker for event-driven notifications), Supports Event-Driven Architecture patterns across the system.
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_SERVICE_MESSAGING_PRODUCER, ROLE_SERVICE_MESSAGING_CONSUMER

### Object Storage Service
- **ID**: object-storage-service
- **Description**: Provides scalable and durable storage for binary objects such as images, documents, backups, and other unstructured data. Accessed via an S3-compatible API.
- **Type**: PlatformService
- **Dependencies**: []
- **Properties**:
  - Storage Type: Distributed Object Store
  - API Compatibility: S3
  - Data Redundancy: Configurable (e.g., erasure coding)
- **Interfaces**:
  - S3 API (PUT Object, GET Object, DELETE Object, List Bucket, etc.)
- **Technology**: MinIO
- **Resources**:
  - CPU: Medium (scales with load)
  - Memory: Medium (scales with load)
  - Storage: Very Large (scalable to Petabytes)
  - Network: High
- **Configuration**:
  - endpoint_url: http://minio-host:9000
  - access_key: env:MINIO_ROOT_USER
  - secret_key_ref: secrets.minio.root_password
  - default_bucket_region: us-east-1
- **Health Check**:
  - Path: /minio/health/live
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-FRM-020 (backend for profile photos), REQ-2-008 (backend for land documents), REQ-10-002 (backend for query attachments), Serves as a target for system backups.
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_SERVICE_OBJECT_STORAGE_CLIENT

### Distributed Tracing System
- **ID**: distributed-tracing-system
- **Description**: Collects, processes, stores, and visualizes trace data from microservices to provide insights into request flows, latencies, and dependencies in a distributed environment.
- **Type**: ObservabilityPlatformService
- **Dependencies**: log-aggregation-system
- **Properties**:
  - Sampling Strategy: Configurable (e.g., probabilistic, rate-limiting)
  - Trace Storage Backend: Elasticsearch or Cassandra (via Jaeger)
  - UI Visualization: True
- **Interfaces**:
  - Jaeger Collector API (Thrift, gRPC, HTTP)
  - Jaeger Query API (HTTP for UI)
  - Zipkin API (HTTP for compatibility)
- **Technology**: Jaeger
- **Resources**:
  - CPU: Medium-High (Collector), Medium (Query)
  - Memory: Large (Collector), Large (Query)
  - Storage: Dependent on trace storage backend (e.g., Elasticsearch)
  - Network: Medium
- **Configuration**:
  - collector_zipkin_http_port: 9411
  - collector_jaeger_grpc_port: 14250
  - collector_jaeger_thrift_http_port: 14268
  - query_http_port: 16686
  - agent_host_port: localhost:6831
- **Health Check**:
  - Path: /
  - Interval: 60
  - Timeout: 10
- **Responsible Features**: REQ-17-002 (provides the backend system for trace correlation), Enhances overall system observability and debuggability.
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_DEVOPS_ENGINEER, ROLE_SYSTEM_ADMINISTRATOR

### Metrics Monitoring System
- **ID**: monitoring-system-metrics
- **Description**: Collects, stores, and allows querying of time-series metrics data from microservices and infrastructure. Powers dashboards and alerting.
- **Type**: ObservabilityPlatformService
- **Dependencies**: []
- **Properties**:
  - Scrape Interval Default: 15s
  - Evaluation Interval Default: 15s
  - Metrics Retention Period: Configurable (e.g., 30d)
- **Interfaces**:
  - HTTP API for querying (PromQL)
  - /metrics endpoint for scraping by Prometheus server itself
- **Technology**: Prometheus
- **Resources**:
  - CPU: Medium-High (scales with number of targets and series)
  - Memory: Large (scales with number of series)
  - Storage: Large (for time-series data)
  - Network: Medium
- **Configuration**:
  - server_port: 9090
  - config_file_path: /etc/prometheus/prometheus.yml
  - storage_tsdb_path: /prometheus_data/
  - alertmanager_config_file_path: /etc/alertmanager/config.yml
- **Health Check**:
  - Path: /-/healthy
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-17-005 (metrics collection and storage), REQ-17-006 (alert rule evaluation, integrates with Alertmanager), REQ-17-007, REQ-17-008
- **Security**:
  - Requires Authentication: false
  - Requires Authorization: false
  - Allowed Roles: []

### Log Aggregation and Analysis System
- **ID**: log-aggregation-system
- **Description**: Collects, parses, stores, indexes, and provides powerful search and analysis capabilities for logs from all system components (applications, infrastructure).
- **Type**: ObservabilityPlatformService
- **Dependencies**: []
- **Properties**:
  - Log Collection Method: Filebeat, Fluentd, Logstash agents
  - Log Storage Engine: Elasticsearch
  - Log Visualization Tool: Kibana
  - Log Retention Period: Configurable (e.g., 7d hot, 90d warm, 1y cold)
- **Interfaces**:
  - Elasticsearch REST API (for indexing and querying)
  - Kibana Web UI (for search, visualization, dashboards)
- **Technology**: Elasticsearch, Logstash (or Fluentd), Kibana (ELK/EFK Stack)
- **Resources**:
  - CPU: High (Elasticsearch, Logstash)
  - Memory: Very Large (Elasticsearch, Logstash)
  - Storage: Very Large (for log data, scales with volume and retention)
  - Network: High
- **Configuration**:
  - elasticsearch_http_port: 9200
  - elasticsearch_transport_port: 9300
  - kibana_http_port: 5601
  - logstash_input_beats_port: 5044
- **Health Check**:
  - Path: /_cluster/health
  - Interval: 60
  - Timeout: 10
- **Responsible Features**: REQ-17-001 (provides the backend system for log aggregation), REQ-17-003 (manages log retention through Elasticsearch ILM), REQ-17-004 (stores and makes searchable key user activity logs)
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_DEVOPS_ENGINEER, ROLE_SYSTEM_ADMINISTRATOR, ROLE_SUPPORT_ENGINEER

### Monitoring Visualization and Alerting Manager
- **ID**: monitoring-visualization-alerting-manager
- **Description**: Provides dashboards for visualizing metrics (from Prometheus) and logs (from Elasticsearch). Manages alert rules based on monitoring data and dispatches alerts via various channels.
- **Type**: ObservabilityPlatformService
- **Dependencies**: monitoring-system-metrics, log-aggregation-system, notification-service
- **Properties**:
  - Supported Datasources: Prometheus, Elasticsearch, Loki, InfluxDB, etc.
  - Alerting Channels: Email, Slack, PagerDuty, Webhook (via Notification Service or direct)
- **Interfaces**:
  - Web UI for dashboard creation, alert rule management, and data exploration
- **Technology**: Grafana
- **Resources**:
  - CPU: Medium
  - Memory: Medium
  - Storage: Small (for dashboard/alert configurations, user data - typically in its own DB)
  - Network: Medium
- **Configuration**:
  - server_http_port: 3000
  - admin_user_default: admin
  - admin_password_default_ref: secrets.grafana.admin_password
  - database_type: sqlite3 (default), mysql, postgres
  - database_connection_string: env:GRAFANA_DB_URI
- **Health Check**:
  - Path: /api/health
  - Interval: 30
  - Timeout: 5
- **Responsible Features**: REQ-DASH-013 (provides UI for system health dashboards), REQ-17-006 (provides UI for alert management and configuration), REQ-17-009 (supports escalation paths and severity configuration for alerts)
- **Security**:
  - Requires Authentication: true
  - Requires Authorization: true
  - Allowed Roles: ROLE_ADMIN_GRAFANA, ROLE_EDITOR_GRAFANA, ROLE_VIEWER_GRAFANA

## Component Relationships

### Web App to API Gateway
- **ID**: rel_web_app_to_api_gw
- **Source**: web-app-client
- **Target**: api-gateway-component
- **Type**: Dependency
- **Description**: Web application communicates with backend services via the API Gateway.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTPS
  - DataFormat: JSON
- **Configuration**:
  - api_base_url_config_key: env:API_GATEWAY_URL

### Mobile App to API Gateway
- **ID**: rel_mobile_app_to_api_gw
- **Source**: mobile-app-client
- **Target**: api-gateway-component
- **Type**: Dependency
- **Description**: Mobile application communicates with backend services via the API Gateway for online operations and synchronization.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTPS
  - DataFormat: JSON
- **Configuration**:
  - api_base_url_config_key: env:API_GATEWAY_URL

### API Gateway to IAM
- **ID**: rel_api_gw_to_iam
- **Source**: api-gateway-component
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: API Gateway routes authentication and user management requests to Identity Access Service and relies on it for token validation.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix_auth: /api/v1/auth
  - routing_prefix_users: /api/v1/users
  - jwt_validation_via: identity-access-service

### API Gateway to Farmer Registry
- **ID**: rel_api_gw_to_farmer_reg
- **Source**: api-gateway-component
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: API Gateway routes farmer profile requests to Farmer Registry Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/farmers

### API Gateway to Farm Land
- **ID**: rel_api_gw_to_farm_land
- **Source**: api-gateway-component
- **Target**: farm-land-service
- **Type**: Dependency
- **Description**: API Gateway routes farm land data requests to Farm Land Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/farmlands

### API Gateway to Crop Management
- **ID**: rel_api_gw_to_crop_mgmt
- **Source**: api-gateway-component
- **Target**: crop-management-service
- **Type**: Dependency
- **Description**: API Gateway routes crop management requests to Crop Management Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/crop-cycles

### API Gateway to Master Data
- **ID**: rel_api_gw_to_master_data
- **Source**: api-gateway-component
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: API Gateway routes master data requests to Master Data Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/masterdata

### API Gateway to Notification Admin
- **ID**: rel_api_gw_to_notification_admin
- **Source**: api-gateway-component
- **Target**: notification-service
- **Type**: Dependency
- **Description**: API Gateway routes admin requests for notification templates to Notification Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix_admin: /api/v1/admin/notifications/templates

### API Gateway to Weather
- **ID**: rel_api_gw_to_weather
- **Source**: api-gateway-component
- **Target**: weather-service
- **Type**: Dependency
- **Description**: API Gateway routes weather forecast requests to Weather Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/weather

### API Gateway to Query Management
- **ID**: rel_api_gw_to_query_mgmt
- **Source**: api-gateway-component
- **Target**: query-management-service
- **Type**: Dependency
- **Description**: API Gateway routes query management and knowledge base requests to Query Management Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix_queries: /api/v1/queries
  - routing_prefix_kb: /api/v1/knowledgebase

### API Gateway to Dashboard
- **ID**: rel_api_gw_to_dashboard
- **Source**: api-gateway-component
- **Target**: dashboard-service
- **Type**: Dependency
- **Description**: API Gateway routes dashboard data requests to Dashboard Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/dashboards

### API Gateway to Admin Configuration Service
- **ID**: rel_api_gw_to_admin_config_svc
- **Source**: api-gateway-component
- **Target**: admin-configuration-service
- **Type**: Dependency
- **Description**: API Gateway routes administrative configuration requests to Admin Configuration Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/admin/config

### API Gateway to Synchronization
- **ID**: rel_api_gw_to_sync
- **Source**: api-gateway-component
- **Target**: synchronization-orchestration-service
- **Type**: Dependency
- **Description**: API Gateway routes data synchronization requests to Synchronization Orchestration Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/sync

### API Gateway to Financial
- **ID**: rel_api_gw_to_financial
- **Source**: api-gateway-component
- **Target**: financial-integration-service
- **Type**: Dependency
- **Description**: API Gateway routes financial integration requests to Financial Integration Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - routing_prefix: /api/v1/financial

### API Gateway to Service Discovery
- **ID**: rel_api_gw_to_service_discovery
- **Source**: api-gateway-component
- **Target**: service-discovery-registry
- **Type**: Dependency
- **Description**: API Gateway uses Service Discovery Registry to find backend service instances for routing.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP (Eureka API)
- **Configuration**:
  - service_discovery_uri_ref: service_discovery_uri

### API Gateway to Config Server Platform
- **ID**: rel_api_gw_to_config_server_platform
- **Source**: api-gateway-component
- **Target**: config-server
- **Type**: Dependency
- **Description**: API Gateway fetches its operational configuration from the Centralized Configuration Server.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP
- **Configuration**:
  - config_profile: api-gateway

### IAM to Notification
- **ID**: rel_iam_to_notification
- **Source**: identity-access-service
- **Target**: notification-service
- **Type**: Dependency
- **Description**: Identity Access Service uses Notification Service for sending OTPs, registration emails, password reset links, etc.
- **Properties**:
  - Communication: Asynchronous (preferred via Message Broker) or Synchronous
  - Protocol: AMQP or HTTP/Internal
- **Configuration**:
  - event_type_user_registered: UserRegisteredEvent
  - event_type_otp_request: OtpRequestEvent

### IAM to Master Data
- **ID**: rel_iam_to_master_data
- **Source**: identity-access-service
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: Identity Access Service may use Master Data Service for predefined roles, permission lists, or other identity-related static data.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - master_list_roles: roles
  - master_list_permissions: permissions

### Farmer Registry to IAM
- **ID**: rel_farmer_reg_to_iam
- **Source**: farmer-registry-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Farmer Registry Service associates farmer profiles with user accounts in Identity Access Service and checks authorization.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - user_id_linkage: FK_UserID_to_IAM_User

### Farmer Registry to Notification
- **ID**: rel_farmer_reg_to_notification
- **Source**: farmer-registry-service
- **Target**: notification-service
- **Type**: Dependency
- **Description**: Farmer Registry Service uses Notification Service for farmer-related notifications (e.g., profile approval status, consent updates).
- **Properties**:
  - Communication: Asynchronous (preferred via Message Broker) or Synchronous
  - Protocol: AMQP or HTTP/Internal
- **Configuration**:
  - event_type_profile_approved: FarmerProfileApprovedEvent

### Farmer Registry to Master Data
- **ID**: rel_farmer_reg_to_master_data
- **Source**: farmer-registry-service
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: Farmer Registry Service uses Master Data Service for lookups (e.g., gender, status, ID types, education levels, languages).
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - master_list_keys: gender, farmer_status, id_types, education_levels, languages

### Farmer Registry to Object Storage
- **ID**: rel_farmer_reg_to_object_storage
- **Source**: farmer-registry-service
- **Target**: object-storage-service
- **Type**: Dependency
- **Description**: Farmer Registry Service stores farmer profile photos in Object Storage.
- **Properties**:
  - Communication: Synchronous
  - Protocol: S3 API (HTTPS)
- **Configuration**:
  - bucket_name_ref: profile_photo_storage_bucket

### Farm Land to IAM
- **ID**: rel_farm_land_to_iam
- **Source**: farm-land-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Farm Land Service uses Identity Access Service for user context (e.g., created by, last updated by) and authorization checks.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Farm Land to Farmer Registry
- **ID**: rel_farm_land_to_farmer_reg
- **Source**: farm-land-service
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: Farm Land Service links land records to specific farmers managed by Farmer Registry Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - farmer_id_linkage: FK_FarmerID_to_FarmerRegistry

### Farm Land to Master Data
- **ID**: rel_farm_land_to_master_data
- **Source**: farm-land-service
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: Farm Land Service uses Master Data Service for lookups (e.g., soil type, ownership type, irrigation sources, land status).
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - master_list_keys: soil_types, land_ownership_types, irrigation_sources, land_topography, land_status

### Farm Land to Object Storage
- **ID**: rel_farm_land_to_object_storage
- **Source**: farm-land-service
- **Target**: object-storage-service
- **Type**: Dependency
- **Description**: Farm Land Service stores land ownership documents in Object Storage.
- **Properties**:
  - Communication: Synchronous
  - Protocol: S3 API (HTTPS)
- **Configuration**:
  - bucket_name_ref: land_document_storage_bucket

### Crop Management to IAM
- **ID**: rel_crop_mgmt_to_iam
- **Source**: crop-management-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Crop Management Service uses Identity Access Service for user context and authorization.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Crop Management to Farmer Registry
- **ID**: rel_crop_mgmt_to_farmer_reg
- **Source**: crop-management-service
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: Crop Management Service links crop cycles to farmers via Farmer Registry Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - farmer_id_linkage: FK_FarmerID_to_FarmerRegistry

### Crop Management to Farm Land
- **ID**: rel_crop_mgmt_to_farm_land
- **Source**: crop-management-service
- **Target**: farm-land-service
- **Type**: Dependency
- **Description**: Crop Management Service links crop cycles to specific farm lands managed by Farm Land Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - land_id_linkage: FK_LandID_to_FarmLandService

### Crop Management to Master Data
- **ID**: rel_crop_mgmt_to_master_data
- **Source**: crop-management-service
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: Crop Management Service uses Master Data Service for lookups (e.g., crop types, activity types, input types, units of measure).
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - master_list_keys: crop_types, crop_varieties, activity_types, input_types, crop_cycle_status, units

### Notification Service to IAM
- **ID**: rel_notification_svc_to_iam
- **Source**: notification-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Notification Service fetches user contact details (phone, email) and notification preferences from Identity Access Service.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - user_profile_endpoint: /api/v1/users/{userId}/profile
  - user_preferences_endpoint: /api/v1/users/{userId}/notification-preferences

### Notification Service to Message Broker
- **ID**: rel_notification_svc_to_message_broker
- **Source**: notification-service
- **Target**: message-broker
- **Type**: Dependency
- **Description**: Notification Service consumes notification request events from Message Broker and may publish delivery status events.
- **Properties**:
  - Communication: Asynchronous
  - Protocol: AMQP
- **Configuration**:
  - consume_queue: notification_requests_q
  - publish_exchange: notification_status_ex

### Weather to Notification
- **ID**: rel_weather_to_notification
- **Source**: weather-service
- **Target**: notification-service
- **Type**: Dependency
- **Description**: Weather Service uses Notification Service to send severe weather alerts to users.
- **Properties**:
  - Communication: Asynchronous (preferred via Message Broker) or Synchronous
  - Protocol: AMQP or HTTP/Internal
- **Configuration**:
  - event_type_severe_weather: SevereWeatherAlertEvent

### Weather to Farm Land
- **ID**: rel_weather_to_farm_land
- **Source**: weather-service
- **Target**: farm-land-service
- **Type**: Dependency
- **Description**: Weather Service fetches farm land locations (GPS coordinates) to provide localized weather forecasts.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - land_geospatial_endpoint: /api/v1/farmlands/{landId}/geospatial

### Query Management to IAM
- **ID**: rel_query_mgmt_to_iam
- **Source**: query-management-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Query Management Service uses Identity Access Service for identifying farmers and consultants, and for authorization.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Query Management to Farmer Registry
- **ID**: rel_query_mgmt_to_farmer_reg
- **Source**: query-management-service
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: Query Management Service links queries to farmer profiles from Farmer Registry Service for context.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - farmer_info_endpoint: /api/v1/farmers/{farmerId}

### Query Management to Notification
- **ID**: rel_query_mgmt_to_notification
- **Source**: query-management-service
- **Target**: notification-service
- **Type**: Dependency
- **Description**: Query Management Service uses Notification Service for notifying users about query status updates, new answers, etc.
- **Properties**:
  - Communication: Asynchronous (preferred via Message Broker) or Synchronous
  - Protocol: AMQP or HTTP/Internal
- **Configuration**:
  - event_type_query_answered: QueryAnsweredEvent
  - event_type_new_query_for_consultant: NewQueryAssignedEvent

### Query Management to Master Data
- **ID**: rel_query_mgmt_to_master_data
- **Source**: query-management-service
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: Query Management Service uses Master Data Service for query categories, KB tags, query statuses.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal
- **Configuration**:
  - master_list_keys: query_categories, query_status, kb_tags

### Query Management to Object Storage
- **ID**: rel_query_mgmt_to_object_storage
- **Source**: query-management-service
- **Target**: object-storage-service
- **Type**: Dependency
- **Description**: Query Management Service stores attachments for queries and KB articles in Object Storage.
- **Properties**:
  - Communication: Synchronous
  - Protocol: S3 API (HTTPS)
- **Configuration**:
  - bucket_name_ref: query_attachment_storage_bucket

### Dashboard to IAM
- **ID**: rel_dashboard_to_iam
- **Source**: dashboard-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Dashboard Service uses Identity Access Service for user context and to determine role-specific dashboard views.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Dashboard to Farmer Registry
- **ID**: rel_dashboard_to_farmer_reg
- **Source**: dashboard-service
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: Dashboard Service aggregates data from Farmer Registry Service for display.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal (potentially via read replicas or materialized views)

### Dashboard to Farm Land
- **ID**: rel_dashboard_to_farm_land
- **Source**: dashboard-service
- **Target**: farm-land-service
- **Type**: Dependency
- **Description**: Dashboard Service aggregates data from Farm Land Service for display.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal (potentially via read replicas or materialized views)

### Dashboard to Crop Management
- **ID**: rel_dashboard_to_crop_mgmt
- **Source**: dashboard-service
- **Target**: crop-management-service
- **Type**: Dependency
- **Description**: Dashboard Service aggregates data from Crop Management Service for display.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal (potentially via read replicas or materialized views)

### Dashboard to Query Management
- **ID**: rel_dashboard_to_query_mgmt
- **Source**: dashboard-service
- **Target**: query-management-service
- **Type**: Dependency
- **Description**: Dashboard Service aggregates data from Query Management Service for display.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal (potentially via read replicas or materialized views)

### Admin Configuration Service to IAM
- **ID**: rel_admin_config_svc_to_iam
- **Source**: admin-configuration-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Admin Configuration Service uses Identity Access Service for authorizing admin users to manage configurations.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Admin Configuration Service to Config Server
- **ID**: rel_admin_config_svc_to_config_server
- **Source**: admin-configuration-service
- **Target**: config-server
- **Type**: Dependency
- **Description**: Admin Configuration Service interacts with the Config Server's backend (e.g., Git, Vault) to manage configurations presented via its UI.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal (or direct backend interaction)
- **Configuration**:
  - config_store_management_interface: proprietary or direct backend API

### Synchronization to IAM
- **ID**: rel_sync_to_iam
- **Source**: synchronization-orchestration-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Synchronization Service uses Identity Access Service for user authentication/authorization during sync operations.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Synchronization to Farmer Registry
- **ID**: rel_sync_to_farmer_reg
- **Source**: synchronization-orchestration-service
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: Synchronization Service interacts with Farmer Registry Service to sync farmer profile data.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Synchronization to Farm Land
- **ID**: rel_sync_to_farm_land
- **Source**: synchronization-orchestration-service
- **Target**: farm-land-service
- **Type**: Dependency
- **Description**: Synchronization Service interacts with Farm Land Service to sync land data.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Synchronization to Crop Management
- **ID**: rel_sync_to_crop_mgmt
- **Source**: synchronization-orchestration-service
- **Target**: crop-management-service
- **Type**: Dependency
- **Description**: Synchronization Service interacts with Crop Management Service to sync crop cycle data.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Synchronization to Master Data
- **ID**: rel_sync_to_master_data
- **Source**: synchronization-orchestration-service
- **Target**: master-data-service
- **Type**: Dependency
- **Description**: Synchronization Service may need to sync or validate against Master Data Service for lookups used in offline data.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Financial to IAM
- **ID**: rel_financial_to_iam
- **Source**: financial-integration-service
- **Target**: identity-access-service
- **Type**: Dependency
- **Description**: Financial Integration Service uses Identity Access Service for authorization, especially for administrative operations or linking transactions to system users.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Financial to Farmer Registry
- **ID**: rel_financial_to_farmer_reg
- **Source**: financial-integration-service
- **Target**: farmer-registry-service
- **Type**: Dependency
- **Description**: Financial Integration Service retrieves farmer bank details and other relevant PII from Farmer Registry Service for payment processing.
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP/Internal

### Financial to Notification
- **ID**: rel_financial_to_notification
- **Source**: financial-integration-service
- **Target**: notification-service
- **Type**: Dependency
- **Description**: Financial Integration Service uses Notification Service for sending payment status updates to farmers or alerts to admins.
- **Properties**:
  - Communication: Asynchronous (preferred via Message Broker) or Synchronous
  - Protocol: AMQP or HTTP/Internal

### Generic Service to Service Discovery
- **ID**: rel_generic_service_to_service_discovery
- **Source**: master-data-service
- **Target**: service-discovery-registry
- **Type**: Dependency
- **Description**: Microservices register with and discover other services via the Service Discovery Registry. (Representative for all microservices).
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP (Eureka API)
- **Configuration**:
  - applies_to_all_microservices: true

### Generic Service to Config Server
- **ID**: rel_generic_service_to_config_server
- **Source**: master-data-service
- **Target**: config-server
- **Type**: Dependency
- **Description**: Microservices fetch their configuration from the Centralized Configuration Server. (Representative for all microservices).
- **Properties**:
  - Communication: Synchronous
  - Protocol: HTTP
- **Configuration**:
  - applies_to_all_microservices: true

### Generic Service to Message Broker (Publish)
- **ID**: rel_generic_service_to_message_broker_pub
- **Source**: farmer-registry-service
- **Target**: message-broker
- **Type**: Dependency
- **Description**: Microservices publish domain events to the Message Broker. (Representative for event-producing microservices).
- **Properties**:
  - Communication: Asynchronous
  - Protocol: AMQP
- **Configuration**:
  - role: Producer
  - applies_to_event_producing_microservices: true

### Generic Service to Message Broker (Subscribe)
- **ID**: rel_generic_service_to_message_broker_sub
- **Source**: notification-service
