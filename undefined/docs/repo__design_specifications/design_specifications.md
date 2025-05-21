# Software Design Specification for glyph-puzzle-backend-api

## 1. Introduction
*   **Purpose:** To define the design for the Glyph Puzzle backend API responsible for game services including authentication, leaderboards, IAP validation, player data synchronization, procedural level seed management, player rewards, analytics ingestion, and basic administrative functions.
*   **Scope:** This document specifies the design for the `glyph-puzzle-backend-api` repository, covering its architecture, API endpoints, data structures (DTOs), core logic (Services), data access interfaces, external service integrations, middleware, utilities, and configuration.
*   **Goals:**
    *   Implement RESTful APIs for core game backend features.
    *   Provide secure and scalable services.
    *   Integrate with other system repositories (Data Access, Security, Procedural Generation, Analytics).
    *   Adhere to layered architecture and design patterns.

## 2. Architecture & Design Principles
*   **Architectural Style:** Layered Architecture (Presentation, Application Services, Infrastructure).
*   **Patterns:**
    *   **Client-Server:** This repository acts as the server component.
    *   **Service Layer:** Business logic is encapsulated within services (`src/services`).
    *   **Repository Pattern:** Services interact with data stores via repository interfaces defined within this repository but implemented in `REPO-DOMAIN-DATA`.
    *   **Dependency Injection:** Services and Controllers receive their dependencies (other services, repository interfaces, utilities) via constructor injection.
    *   **Domain-Driven Design (DDD) Principles:** Although domain models reside in `REPO-DOMAIN-DATA`, this API's services are organized around domain concepts (Auth, Leaderboard, IAP, etc.).
*   **Structure:** Express.js application structured with clear separation between routes, controllers, middleware, services, configurations, and utilities.

## 3. Technology Stack
*   **Language:** TypeScript 5.3.3
*   **Runtime:** Node.js 20.14.0 LTS
*   **Framework:** Express.js 4.19.2
*   **ORM/ODM:** Mongoose (used indirectly via repository interfaces, actual implementation in `REPO-DOMAIN-DATA`)
*   **Caching:** Redis (`ioredis` client, wrapped by `CacheService`)
*   **HTTP Client:** Axios (wrapped by `ExternalApiService`)
*   **Authentication:** JWT (`jsonwebtoken`)
*   **Validation:** Joi
*   **Logging:** Winston
*   **Documentation:** Swagger/OpenAPI (`swagger-ui-express`, `swagger-jsdoc`)
*   **Middleware:** `cors`, `dotenv`, `express-rate-limit`, built-in Express middleware (`json`, `urlencoded`).

## 4. Requirements Mapping
*   REQ-8-002: Player Rewards (RewardsController, PlayerRewardsService, relevant DTOs)
*   REQ-SCF-003: Leaderboard (LeaderboardController, LeaderboardService, relevant DTOs)
*   REQ-8-013: IAP Validation (IAPValidationController, IAPValidationService, relevant DTOs)
*   REQ-8-003: API Documentation (config/swagger.ts, app.ts setup)
*   REQ-SEC-006: Rate Limiting (rateLimit.middleware.ts)
*   REQ-CGLE-011, REQ-8-027: Procedural Level Seeds (ProceduralLevelController, ProceduralLevelService, relevant DTOs)
*   REQ-8-022, REQ-8-023: Admin Functions (AdminController, AdminService, relevant DTOs)
*   REQ-SEC-019, REQ-8-019, REQ-8-016: Audit Logging (API services will integrate calls to `IAuditLogRepository` from `REPO-DOMAIN-DATA`)
*   REQ-SEC-001, REQ-8-011: Secure Communication (Assumes HTTPS/TLS handled by infrastructure)
*   REQ-SEC-008: Server-side IAP Validation (IAPValidationService)
*   REQ-SEC-005, REQ-8-014: Score Validation (LeaderboardService, potentially integrating a cheat detection utility/service)
*   REQ-SEC-002, REQ-8-012: Secret Management (Configuration loaded from environment variables)
*   REQ-AMOT-001, REQ-AMOT-002, REQ-AMOT-003: Analytics Ingestion (AnalyticsController, AnalyticsService, relevant DTOs)

## 5. API Endpoints

All endpoints are prefixed with `/api/v1`.

*   **Authentication**
    *   `POST /auth/login`
        *   Description: Player login.
        *   Req Body: `LoginRequestDTO`
        *   Res Body: `TokenResponseDTO`
    *   `POST /auth/refresh`
        *   Description: Refresh access token.
        *   Req Body: `RefreshTokenRequestDTO`
        *   Res Body: `TokenResponseDTO`
*   **Player Data**
    *   `GET /player/data`
        *   Description: Fetch authenticated player's data.
        *   Auth: Required (JWT)
        *   Res Body: `PlayerDataDTO`
    *   `POST /player/sync`
        *   Description: Synchronize player data.
        *   Auth: Required (JWT)
        *   Req Body: `PlayerDataDTO` (client version)
        *   Res Body: `SyncResponseDTO` (latest server data)
*   **Leaderboard**
    *   `POST /leaderboard/scores`
        *   Description: Submit score for a specific leaderboard.
        *   Auth: Required (JWT)
        *   Req Body: `SubmitScoreRequestDTO`
        *   Res Body: Success (e.g., 200/201) with optional details like rank.
    *   `GET /leaderboard/:id`
        *   Description: Get leaderboard data.
        *   Auth: Optional (Public leaderboards) / Required (Private/Friend leaderboards)
        *   Path Params: `id` (Leaderboard ID)
        *   Query Params: `LeaderboardQueryDTO` (limit, offset, timeScope, etc.)
        *   Res Body: `LeaderboardResponseDTO`
*   **IAP Validation**
    *   `POST /iap/validate-receipt`
        *   Description: Validate an IAP receipt and grant items.
        *   Auth: Required (JWT)
        *   Req Body: `ValidateReceiptRequestDTO`
        *   Res Body: `ValidationResponseDTO`
*   **Player Rewards**
    *   `POST /rewards/grant`
        *   Description: Grant a player reward (e.g., after an ad).
        *   Auth: Required (JWT)
        *   Req Body: `GrantRewardRequestDTO`
        *   Res Body: `RewardResponseDTO`
*   **Procedural Level Seeds**
    *   `GET /levels/procedural/seed`
        *   Description: Request a procedural level seed.
        *   Auth: Required (JWT)
        *   Query Params: `LevelSeedRequestDTO` (parameters influencing seed generation/selection)
        *   Res Body: `LevelSeedResponseDTO`
    *   `POST /levels/procedural/log-seed`
        *   Description: Log details of a procedural level played by the client.
        *   Auth: Required (JWT)
        *   Req Body: `LevelSeedRequestDTO` (seed, outcome, stats, etc.)
        *   Res Body: Success (e.g., 200/201).
*   **Analytics Ingestion**
    *   `POST /analytics`
        *   Description: Submit a batch of analytics events.
        *   Auth: Required (JWT - to associate events with user)
        *   Req Body: `AnalyticsEventBatchRequestDTO` ({ events: AnalyticsEventDTO[] })
        *   Res Body: Success (e.g., 200/201).
*   **Admin**
    *   `GET /admin/health`
        *   Description: Get system health status.
        *   Auth: Admin Required (Separate API Key/Auth mechanism)
        *   Res Body: Status object.
    *   `PUT /admin/config/:key`
        *   Description: Update a game configuration setting.
        *   Auth: Admin Required
        *   Path Params: `key` (Config key)
        *   Req Body: `AdminActionRequestDTO` ({ value: any })
        *   Res Body: Success (e.g., 200/204).
    *   `GET /admin/configs`
        *   Description: Get all game configuration settings.
        *   Auth: Admin Required
        *   Res Body: Object containing all config keys/values.

## 6. Middleware
*   `src/api/middlewares/requestLogger.middleware.ts`: Logs incoming requests. Logs method, URL, timestamp, and user ID if authentication middleware has populated `req.user`.
*   `src/api/middlewares/rateLimit.middleware.ts`: Implements API rate limiting per IP or authenticated user. Uses `express-rate-limit`. Configurable window and max requests via environment variables.
*   `src/api/middlewares/auth.middleware.ts`: Verifies JWT provided in the `Authorization: Bearer <token>` header. Uses `ISecurityService.verifyJwt`. If valid, decodes user payload and attaches it to `req.user`. Returns 401 Unauthorized if token is missing or invalid.
*   `src/api/middlewares/validation.middleware.ts`: Validates incoming request payloads (body, query, params) against Joi schemas defined for DTOs. Uses a factory function `validate(schema: Joi.Schema, source: 'body' | 'query' | 'params')`. Returns 400 Bad Request if validation fails.
*   `src/api/middlewares/errorHandler.middleware.ts`: Global error handling middleware. Catches instances of `ApiError` or other errors. Formats responses consistently with appropriate HTTP status codes and error details. Logs server errors.

## 7. Services (Application Layer)
*   **`src/services/AuthService.ts`** (Implements `IAuthService`)
    *   Dependencies: `IPlayerRepository`, `ISecurityService`.
    *   `login(identifier: string, password: string): Promise<TokenResponseDTO>`: Finds player by identifier, validates password (delegating to repo or security service if hashing is complex), calls `ISecurityService.generateAuthTokens`, stores refresh token (via repo), returns tokens.
    *   `refreshTokens(refreshToken: string): Promise<TokenResponseDTO>`: Calls `ISecurityService.verifyJwt` for the refresh token. If valid, finds the user via repo using the token's user ID. Calls `ISecurityService.generateAuthTokens` for new tokens, updates refresh token in storage (via repo), invalidates old refresh token.
*   **`src/services/LeaderboardService.ts`** (Implements `ILeaderboardService`)
    *   Dependencies: `IScoreRepository`, `ILeaderboardRepository`, `ICacheService`, potentially `ICheatDetectionService`.
    *   `submitScore(playerId: string, leaderboardId: string, score: number, metadata?: any): Promise<{ rank: number; score: number; }>`: Validates input score. Performs cheat checks (delegated). Retrieves current best score for the player on this leaderboard. If new score is higher (or rules allow), updates score via `IScoreRepository`. Updates leaderboard ranking via `ILeaderboardRepository`. Invalidates relevant cache entries in `ICacheService`. Returns the player's new rank and score.
    *   `getLeaderboard(leaderboardId: string, query: LeaderboardQueryDTO): Promise<LeaderboardResponseDTO>`: Tries to fetch leaderboard data from `ICacheService`. If not found or expired, queries `ILeaderboardRepository` (joining with player info from `IPlayerRepository`). Handles pagination and time scope filtering. Caches results in `ICacheService` before returning. Optionally fetches the requesting player's rank if requested.
*   **`src/services/IAPValidationService.ts`** (Implements `IIAPValidationService`)
    *   Dependencies: `IIAPTransactionRepository`, `IInventoryRepository`, `ICurrencyRepository`, `IExternalApiService`, `IAuditLogRepository`.
    *   `validateReceipt(playerId: string, platform: 'apple' | 'google', receiptData: string, productId: string, transactionId?: string): Promise<ValidationResponseDTO>`: Records transaction attempt in `IIAPTransactionRepository` with status 'pending'. Determines platform validation URL from config. Calls `IExternalApiService.post` to send receipt data to the platform validation endpoint. Parses platform response. If validation is successful and the transaction is not a duplicate (checked via `IIAPTransactionRepository`), identifies purchased content/currency. Grants items/currency to the player via `IInventoryRepository` and `ICurrencyRepository` within a transaction context if possible. Updates transaction status in `IIAPTransactionRepository` to 'completed' or 'failed'. Logs the event via `IAuditLogRepository`. Returns validation result and granted items/currency.
*   **`src/services/PlayerRewardsService.ts`** (Implements `IPlayerRewardsService`)
    *   Dependencies: `IInventoryRepository`, `ICurrencyRepository`, `IAuditLogRepository`.
    *   `grantReward(playerId: string, rewardType: string, placementId: string, validationToken?: string): Promise<RewardResponseDTO>`: Validates the request, potentially using the `validationToken` to verify the reward eligibility (e.g., against a server-to-server callback from an ad network if applicable, or simpler validation). Looks up the reward details (what items/currency to grant) based on `rewardType` and `placementId`. Grants items/currency via `IInventoryRepository` and `ICurrencyRepository`. Logs the reward grant via `IAuditLogRepository`. Returns success status and granted rewards.
*   **`src/services/PlayerDataSyncService.ts`** (Implements `IPlayerDataSyncService`)
    *   Dependencies: `IPlayerRepository`, `IAuditLogRepository`.
    *   `getPlayerData(playerId: string): Promise<PlayerDataDTO>`: Retrieves the player's full data structure from `IPlayerRepository`. Returns the data including a version identifier.
    *   `syncPlayerData(playerId: string, clientData: PlayerDataDTO): Promise<SyncResponseDTO>`: Retrieves server data from `IPlayerRepository`. Compares clientData.version with serverData.version. Implements conflict resolution logic (e.g., simple last-write-wins if client version >= server version, or more complex merging). Updates player data in `IPlayerRepository`. Logs sync event via `IAuditLogRepository`. Returns the latest server data (`SyncResponseDTO`).
*   **`src/services/ProceduralLevelService.ts`** (Implements `IProceduralLevelService`)
    *   Dependencies: `IProceduralLevelDataRepository`, potentially `IProceduralGeneratorClient` (interface to REPO-PROCEDURAL-GEN), `IAuditLogRepository`.
    *   `requestNewSeed(playerId: string, parameters: LevelSeedRequestDTO): Promise<LevelSeedResponseDTO>`: Uses `parameters` (difficulty, progression) to determine how to get a seed. May query `IProceduralLevelDataRepository` for a suitable pre-generated seed, or call `IProceduralGeneratorClient` (if server-side generation is active). Stores the generated/retrieved seed and parameters via `IProceduralLevelDataRepository`. Returns the seed and level parameters. Logs request via `IAuditLogRepository`.
    *   `logUsedSeed(playerId: string, logData: LevelSeedRequestDTO): Promise<void>`: Stores the client-provided log data (seed, outcome, etc.) via `IProceduralLevelDataRepository` for later analysis. Logs event via `IAuditLogRepository`.
*   **`src/services/AnalyticsService.ts`** (Implements `IAnalyticsService`)
    *   Dependencies: `IExternalApiService`.
    *   `ingestEventBatch(events: AnalyticsEventDTO[]): Promise<void>`: Performs basic validation on the event batch structure. Formats the data if necessary for the target system. Calls `IExternalApiService.post` to forward the event batch to the `ANALYTICS_PIPELINE_ENDPOINT` configured in environment variables. Handles potential external API errors (e.g., retries, logging).
*   **`src/services/AdminService.ts`** (Implements `IAdminService`)
    *   Dependencies: `IGameConfigurationRepository`, potentially system monitoring interfaces.
    *   `getSystemHealth(): Promise<any>`: Queries status of critical dependencies like database (`IPlayerRepository` or direct check), cache (`ICacheService`), and key external services (`IExternalApiService` status or mock checks). Returns a status object.
    *   `updateConfiguration(key: string, value: any): Promise<void>`: Validates input key and value. Updates the configuration setting via `IGameConfigurationRepository`. Logs admin action via `IAuditLogRepository`.
    *   `getConfigurations(): Promise<any>`: Retrieves all configuration settings via `IGameConfigurationRepository`.

*   **`src/services/CacheService.ts`** (Implements `ICacheService`)
    *   Dependencies: Redis client instance (configured in `config/redis.ts`).
    *   Provides methods to interact with Redis: `get`, `set`, `del`, `increment`, `expire`. Handles serialization/deserialization (e.g., JSON.stringify/parse) for stored values. Manages Redis connection state and errors.
*   **`src/services/ExternalApiService.ts`** (Implements `IExternalApiService`)
    *   Dependencies: Axios instance.
    *   Provides methods for making HTTP requests (`get`, `post`, `put`, `delete`). Configures defaults (timeouts, headers). Includes basic error handling and logging for external calls.

## 8. Data Access (Repository Interfaces - Expected from REPO-DOMAIN-DATA)
*   **`src/services/interfaces/IPlayerRepository.ts`**: Interface for Player Profile/Data CRUD and queries.
*   **`src/services/interfaces/IScoreRepository.ts`**: Interface for Score CRUD and queries.
*   **`src/services/interfaces/ILeaderboardRepository.ts`**: Interface for Leaderboard ranking data manipulation and queries.
*   **`src/services/interfaces/IIAPTransactionRepository.ts`**: Interface for IAP transaction logging and lookup.
*   **`src/services/interfaces/IInventoryRepository.ts`**: Interface for managing player inventory items.
*   **`src/services/interfaces/ICurrencyRepository.ts`**: Interface for managing player virtual currency balances.
*   **`src/services/interfaces/IProceduralLevelDataRepository.ts`**: Interface for storing and retrieving procedural level seeds and log data.
*   **`src/services/interfaces/IGameConfigurationRepository.ts`**: Interface for accessing and modifying game configurations.
*   **`src/services/interfaces/IAuditLogRepository.ts`**: Interface for logging audit events.

## 9. Infrastructure Interfaces/Clients
*   **`src/services/interfaces/ICacheService.ts`**: Interface for cache operations (get, set, del, etc.).
*   **`src/services/interfaces/IExternalApiService.ts`**: Interface for making external HTTP requests (get, post, etc.).
*   **`src/services/interfaces/ISecurityService.ts`**: Interface for security operations (JWT, IAP validation logic). Expected to be provided by `REPO-SECURITY`.

## 10. Utilities & Helpers
*   **`src/utils/logger.ts`**: Winston logger configuration and instance export. Provides methods like `info`, `error`, `warn`, `debug`.
*   **`src/utils/ApiError.ts`**: Custom error class to standardize API errors with `statusCode` and `isOperational` flag. Used by services and controllers and handled by `errorHandler.middleware.ts`.
*   **`src/utils/helpers.ts`**: Miscellaneous helper functions (e.g., date formatting, simple checks).
*   **`src/utils/constants.ts`**: Application-wide constants (e.g., default pagination limits, cache TTLs, specific error codes, strings).

## 11. Data Transfer Objects (DTOs)

DTOs are defined as TypeScript interfaces or classes in `src/api/*/dtos/` and associated with Joi schemas for validation.

*   **`src/api/auth/dtos/Login.request.dto.ts`**: `{ identifier: string, password: string }`
*   **`src/api/auth/dtos/Token.response.dto.ts`**: `{ accessToken: string, refreshToken: string, expiresIn: number }`
*   **`src/api/auth/dtos/RefreshToken.request.dto.ts`**: `{ refreshToken: string }`
*   **`src/api/player-data/dtos/PlayerData.dto.ts`**: `{ levelProgress: any, inventory: any, currency: any, settings: any, version: number }` (Detailed structure for game data)
*   **`src/api/player-data/dtos/Sync.response.dto.ts`**: `{ success: boolean, latestData: PlayerDataDTO, conflict: boolean, message?: string }`
*   **`src/api/leaderboard/dtos/SubmitScore.request.dto.ts`**: `{ leaderboardId: string, score: number, metadata?: any }`
*   **`src/api/leaderboard/dtos/LeaderboardQuery.dto.ts`**: `{ limit?: number, offset?: number, timeScope?: 'daily' | 'weekly' | 'all', playerId?: string }`
*   **`src/api/leaderboard/dtos/Leaderboard.response.dto.ts`**: `{ leaderboardId: string, entries: Array<{ playerId: string, displayName: string, score: number, rank: number }>, totalEntries?: number, playerEntry?: { playerId: string, displayName: string, score: number, rank: number } }`
*   **`src/api/iap/dtos/ValidateReceipt.request.dto.ts`**: `{ platform: 'apple' | 'google', receiptData: string, productId: string, transactionId?: string }`
*   **`src/api/iap/dtos/Validation.response.dto.ts`**: `{ success: boolean, grantedItems?: Array<{ itemId: string, quantity: number }>, grantedCurrency?: Array<{ currencyId: string, amount: number }>, transactionDetails?: any, errorMessage?: string }`
*   **`src/api/rewards/dtos/GrantReward.request.dto.ts`**: `{ rewardType: string, placementId: string, validationToken?: string }`
*   **`src/api/rewards/dtos/Reward.response.dto.ts`**: `{ success: boolean, grantedItems?: Array<{ itemId: string, quantity: number }>, grantedCurrency?: Array<{ currencyId: string, amount: number }>, errorMessage?: string }`
*   **`src/api/procedural-level/dtos/LevelSeed.request.dto.ts`**: `{ difficultyLevel?: number, playerProgression?: any, seedParameters?: any, seed?: string, outcome?: 'win' | 'loss', score?: number, duration?: number, playerActions?: any }` (Used for both requesting and logging)
*   **`src/api/procedural-level/dtos/LevelSeed.response.dto.ts`**: `{ seed: string, levelParameters: any, levelStructure?: any, puzzleType: string }`
*   **`src/api/analytics/dtos/AnalyticsEvent.dto.ts`**: `{ eventName: string, timestamp: string, parameters?: any }`
*   **`src/api/analytics/dtos/AnalyticsEventBatch.request.dto.ts`**: `{ events: AnalyticsEventDTO[] }`
*   **`src/api/admin/dtos/AdminAction.request.dto.ts`**: `{ value: any }`

## 12. Configuration
*   Environment variables are loaded and validated via `src/config/environment.ts`.
*   Required variables: `PORT`, `MONGODB_URI`, `REDIS_URL`, `JWT_SECRET`, `JWT_ACCESS_TOKEN_EXPIRY`, `JWT_REFRESH_TOKEN_EXPIRY`, `CORS_ALLOWED_ORIGINS`, `ADMIN_API_KEY`, `APPLE_IAP_VALIDATION_URL`, `GOOGLE_IAP_VALIDATION_URL`, `ANALYTICS_PIPELINE_ENDPOINT`, `RATE_LIMIT_WINDOW_MS`, `RATE_LIMIT_MAX_REQUESTS`, `NODE_ENV`.
*   Configuration settings are accessed application-wide via `src/config/index.ts`.
*   Swagger configuration is defined in `src/config/swagger.ts`.
*   Database connection configuration is initiated in `src/config/database.ts` (using Mongoose with `MONGODB_URI`).
*   Redis client configuration is initiated in `src/config/redis.ts` (using `ioredis` with `REDIS_URL`).

## 13. Security Considerations
*   JWT-based authentication for most API endpoints using `auth.middleware.ts`.
*   Role-based access control (simple admin check) for `/admin` endpoints using a dedicated middleware/check based on `ADMIN_API_KEY` or similar mechanism.
*   Server-side validation of IAP receipts (`IAPValidationService`) using external platform APIs.
*   Server-side validation of score submissions (`LeaderboardService`).
*   Rate limiting to mitigate abuse (`rateLimit.middleware.ts`).
*   Input validation on all incoming requests (`validation.middleware.ts`).
*   Secure handling of secrets via environment variables (managed externally).
*   Usage of HTTPS assumed at the infrastructure level.
*   Audit logging for sensitive operations (via `IAuditLogRepository` calls from services).

## 14. Quality Attributes
*   **Performance:** Utilizes Redis caching for frequently accessed data (e.g., leaderboards), efficient data access patterns via repositories, asynchronous nature of Node.js.
*   **Scalability:** Stateless API design allows for horizontal scaling of Node.js instances.
*   **Maintainability:** Clear separation of concerns (layers, services, controllers, middleware), modular file structure, use of TypeScript interfaces for contracts, dependency injection, consistent coding style.
*   **Testability:** Design promotes testability by using interfaces for dependencies, allowing for mocking in unit and integration tests. Automated test suite (unit, integration) is planned.
*   **Reliability:** Robust error handling via `errorHandler.middleware.ts` and `ApiError` class. Dependency health checks (`AdminService.getSystemHealth`). Assumes database/cache high availability and backups are handled by infrastructure.