# Architecture Design Specification

# 1. Patterns

## 1.1. Request-Reply
### 1.1.2. Type
RequestReply

### 1.1.3. Implementation
Synchronous and asynchronous calls via Firebase SDKs (implementing secure HTTP/REST or gRPC).

### 1.1.4. Applicability
The foundational pattern for most client-server interactions. Essential for: user authentication (REQ-9-001), fetching remote configuration at startup (REQ-8-006), and direct data queries/writes to Firestore for cloud save (REQ-10-007), achievement status (REQ-9-004), and retrieving leaderboard data (REQ-9-002). This pattern is used whenever the client needs to request data or a state change and receive a direct response from the backend.

## 1.2. Publish-Subscribe
### 1.2.2. Type
PublishSubscribe

### 1.2.3. Implementation
Asynchronous, one-way event submission via the Firebase Analytics SDK.

### 1.2.4. Applicability
Essential for collecting anonymized telemetry data (REQ-8-001, REQ-8-002). The game client acts as a `Publisher`, sending gameplay events (e.g., 'LEVEL_COMPLETE', 'HINT_USED') to a topic. The client is completely decoupled and does not require a response. The Firebase Analytics backend is the `Subscriber` that consumes and processes these events asynchronously for analysis and game balancing.

## 1.3. Remote Procedure Invocation (RPC)
### 1.3.2. Type
RPC

### 1.3.3. Implementation
Client invokes server-side logic via the Firebase Cloud Functions SDK.

### 1.3.4. Applicability
Strictly necessary for implementing server-side validation logic that cannot be trusted on the client. Its primary, essential use case is for leaderboard score submissions (REQ-9-009, REQ-CPS-012), where the client calls a specific named function on the backend (e.g., `validateAndSubmitScore`) to perform validation before the data is written to the database.

## 1.4. Caching
### 1.4.2. Type
Performance

### 1.4.3. Implementation
Client-side via Firebase SDKs and server-side via caching layers as defined in the database design (e.g., Redis).

### 1.4.4. Applicability
An essential pattern for meeting performance and scalability goals. The Firebase Remote Config SDK uses it to reduce app startup latency (REQ-8-006). It is explicitly required by the database design for `LevelDefinition` and `Leaderboard` entities to reduce database read load and improve response times for frequently accessed data, which is critical for a good user experience.



---

