# Software Design Specification (SDS) for REPO-PATT-005: BackendServiceFacade

## 1. Overview

This document specifies the design for the `BackendServiceFacade` repository. This repository acts as a client-side Infrastructure layer component, providing a single, clean, and abstracted interface to all Firebase backend services.

The primary architectural goals are:
*   **Decoupling:** To isolate the rest of the Unity application from the specifics of the Firebase SDKs. The Application and Domain layers will depend on application-centric interfaces, not Firebase-specific classes.
*   **Simplicity:** To provide a single point of access (the Facade) for all backend functionalities, simplifying its usage for consumer services like `GameManager` or `UIManager`.
*   **Consistency:** To ensure all backend operations, which are inherently asynchronous, follow a consistent pattern for returning results and handling errors, primarily through a custom `FirebaseResult<T>` wrapper.

This repository implements the **Facade Pattern** via `FirebaseServiceFacade.cs` and the **Adapter Pattern** for each specific Firebase service (e.g., `FirebaseAuthAdapter.cs`, `FirestoreCloudSaveAdapter.cs`).

---

## 2. Core Concepts & Patterns

### 2.1. Facade Pattern
The `FirebaseServiceFacade` class is the central entry point. It aggregates all individual backend service adapters (for Auth, Cloud Save, etc.) and exposes them through a simple, unified API. The rest of the application will only interact with this facade, not the individual adapters, thereby reducing coupling.

### 2.2. Adapter Pattern
Each Firebase service (Auth, Firestore, etc.) will have a corresponding `*Adapter` class. This class implements a domain-centric interface (e.g., `IAuthenticationService`) and translates the application's requests into specific Firebase SDK calls. This pattern encapsulates the Firebase dependency and allows the underlying implementation to be changed with minimal impact on the application's core logic.

### 2.3. Asynchronous Operations & Result Handling
All methods that interact with Firebase services will be asynchronous and return a `Task<FirebaseResult>` or `Task<FirebaseResult<T>>`. This approach provides several benefits:
*   It avoids blocking the main Unity thread, ensuring a responsive UI.
*   It provides a consistent error handling mechanism. Instead of relying on exceptions for control flow, the `FirebaseResult` object will encapsulate either a successful result (`IsSuccess = true`, `Value` contains data) or a failure (`IsSuccess = false`, `Error` contains details).
*   It decouples the application layer from Firebase-specific exception types.

### 2.4. Dependency Injection (DI)
The system will use a DI container (e.g., VContainer, Zenject). A dedicated registration class (`FirebaseServiceRegistration`) will be responsible for setting up the object graph, binding the service interfaces (e.g., `IAuthenticationService`) to their concrete adapter implementations (e.g., `FirebaseAuthAdapter`). This promotes loose coupling and high testability.

---

## 3. Component Specifications

### 3.1. Project File
#### `PatternCipher.Infrastructure.Firebase.csproj`
*   **Purpose:** Defines the C# project, its dependencies, and build configurations.
*   **Specifications:**
    *   Target Framework: Must be compatible with the Unity project's .NET version.
    *   **Package References:** Must include all necessary Firebase SDKs for Unity, installed via the Unity Package Manager. The project file should reflect these dependencies.
        *   `com.google.firebase.app`
        *   `com.google.firebase.auth`
        *   `com.google.firebase.firestore`
        *   `com.google.firebase.remote-config`
        *   `com.google.firebase.analytics`
    *   **Project References:** Must include a reference to the project containing the shared domain models and service interfaces (e.g., `PatternCipher.Application` or `PatternCipher.Domain`).

### 3.2. Initialization
#### `FirebaseInitializer.cs`
*   **Purpose:** Ensures the Firebase SDK is initialized safely and asynchronously at game startup before any other service attempts to use it.
*   **Type:** Unity `MonoBehaviour`.
*   **Design:**
    *   This script should be attached to a persistent GameObject that exists in the initial loading scene of the game.
    *   It should use a pattern to prevent multiple initializations (e.g., Singleton or a static flag).
*   **Methods:**
    *   `public static Task<bool> InitializeAsync()`: A static async method that other services can await. It encapsulates the initialization logic.
        *   **Logic:**
            1.  Check if initialization has already completed. If so, return `true`.
            2.  Call `Firebase.FirebaseApp.CheckAndFixDependenciesAsync()`. This is crucial for Android.
            3.  Await the result.
            4.  If `dependencyStatus == Firebase.DependencyStatus.Available`, the initialization is successful.
            5.  Store the result (`true` or `false`) in a static `TaskCompletionSource<bool>`.
            6.  Return the task from the `TaskCompletionSource`.
            7.  Log errors if initialization fails.

### 3.3. Core Facade
#### `FirebaseServiceFacade.cs`
*   **Purpose:** To act as the single, simplified entry point for all backend services.
*   **Type:** Standard C# class.
*   **Design:**
    *   This class will be registered as a singleton in the DI container.
    *   It will receive all service adapter interfaces via constructor injection.
*   **Members:**
    *   `public IAuthenticationService Auth { get; }`
    *   `public ICloudSaveService CloudSave { get; }`
    *   `public ILeaderboardService Leaderboards { get; }` (Interface and implementation to be defined in a separate Leaderboard service if needed, but the property should exist on the facade).
    *   `public IAnalyticsService Analytics { get; }`
    *   `public IRemoteConfigService RemoteConfig { get; }`
*   **Constructor:**
    *   `public FirebaseServiceFacade(IAuthenticationService auth, ICloudSaveService cloudSave, ...)`: Assigns the injected services to the public properties.

### 3.4. Service Adapters

#### 3.4.1. `Authentication/FirebaseAuthAdapter.cs`
*   **Purpose:** Implements `IAuthenticationService` using the Firebase Auth SDK.
*   **Methods (implementing `IAuthenticationService`):**
    *   `public Task<FirebaseResult<string>> SignInAnonymouslyAsync()`:
        *   Calls `FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync()`.
        *   Wraps the result in a `FirebaseResult`. On success, the `Value` will be the `FirebaseUser.UserId`. On failure, it catches the `FirebaseException` and creates a `FirebaseResult` with a `FirebaseError`.
    *   `public Task<FirebaseResult<string>> SignInWithGoogleAsync(string idToken)`:
        *   Creates a `Credential` object using `GoogleAuthProvider.GetCredential(idToken, null)`.
        *   Calls `FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential)`.
        *   Handles results and errors as above.
    *   `public Task<FirebaseResult<string>> SignInWithAppleAsync(string idToken)`:
        *   Similar to Google Sign-in, using `OAuthProvider.GetCredential("apple.com", ...)`
    *   `public void SignOut()`:
        *   Calls `FirebaseAuth.DefaultInstance.SignOut()`.
    *   `public string GetCurrentUserId()`:
        *   Returns `FirebaseAuth.DefaultInstance.CurrentUser?.UserId`.
    *   `public bool IsSignedIn()`:
        *   Returns `FirebaseAuth.DefaultInstance.CurrentUser != null`.
    *   **Events:**
        *   Exposes a C# event that wraps `FirebaseAuth.DefaultInstance.StateChanged` to notify the application of sign-in/sign-out events.

#### 3.4.2. `CloudSave/FirestoreCloudSaveAdapter.cs`
*   **Purpose:** Implements `ICloudSaveService` using Cloud Firestore.
*   **Members:**
    *   `private readonly FirebaseFirestore _db = FirebaseFirestore.DefaultInstance;`
*   **Methods (implementing `ICloudSaveService`):**
    *   `public async Task<FirebaseResult> SavePlayerProfileAsync(string userId, PlayerProfile profile)`:
        *   Maps the domain `PlayerProfile` to a `PlayerProfileDto` using `PlayerProfileMapper`.
        *   Adds a server-side timestamp to the DTO for `LastUpdated`.
        *   Gets the document reference: `_db.Collection("users").Document(userId)`.
        *   Calls `docRef.SetAsync(dto)`.
        *   Handles exceptions and returns a `FirebaseResult`.
    *   `public async Task<FirebaseResult<PlayerProfile>> LoadPlayerProfileAsync(string userId)`:
        *   Gets the document snapshot: `await _db.Collection("users").Document(userId).GetSnapshotAsync()`.
        *   Checks if `snapshot.Exists`.
        *   If it exists, converts the snapshot to a `PlayerProfileDto` using `snapshot.ConvertTo<T>()`.
        *   Maps the DTO back to a domain `PlayerProfile` using the mapper.
        *   Returns a `FirebaseResult<PlayerProfile>` with the domain object.
        *   If not exists or an error occurs, returns a failure `FirebaseResult`.

#### 3.4.3. `Analytics/FirebaseAnalyticsAdapter.cs`
*   **Purpose:** Implements `IAnalyticsService` using Firebase Analytics SDK.
*   **Methods (implementing `IAnalyticsService`):**
    *   `public void LogEvent(string eventName)`:
        *   Calls `FirebaseAnalytics.LogEvent(eventName)`.
    *   `public void LogEvent(string eventName, Dictionary<string, object> parameters)`:
        *   Converts the `Dictionary<string, object>` into an array of `Firebase.Analytics.Parameter`. This loop must handle converting C# types (`int`, `string`, `bool`, `double`) into their corresponding Firebase `Parameter` types.
        *   Calls `FirebaseAnalytics.LogEvent(eventName, firebaseParameters)`.
    *   `public void SetUserProperty(string propertyName, string value)`:
        *   Calls `FirebaseAnalytics.SetUserProperty(propertyName, value)`.

#### 3.4.4. `RemoteConfig/FirebaseRemoteConfigAdapter.cs`
*   **Purpose:** Implements `IRemoteConfigService` using Firebase Remote Config SDK.
*   **Methods (implementing `IRemoteConfigService`):**
    *   `public async Task<FirebaseResult> InitializeAsync(Dictionary<string, object> defaults)`:
        *   Sets default values using `FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)`.
        *   Fetches latest values using `FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero)`.
        *   Activates them using `FirebaseRemoteConfig.DefaultInstance.ActivateAsync()`.
        *   Returns a `FirebaseResult` indicating the success of the fetch and activate operations.
    *   `public string GetString(string key, string defaultValue)`:
        *   Retrieves the value using `FirebaseRemoteConfig.DefaultInstance.GetValue(key)`.
        *   Returns `value.StringValue` if source is not `Default`, otherwise `defaultValue`.
    *   `public bool GetBool(string key, bool defaultValue)`:
        *   Retrieves and returns `value.BooleanValue`.
    *   `public int GetInt(string key, int defaultValue)`:
        *   Retrieves `value.DoubleValue` or `StringValue` and parses to `int`.
    *   `public float GetFloat(string key, float defaultValue)`:
        *   Retrieves `value.DoubleValue` and casts to `float`.

### 3.5. Data Transfer Objects (DTOs) & Mappers

#### 3.5.1. `CloudSave/DTOs/PlayerProfileDto.cs`
*   **Purpose:** Defines the data structure for player profiles in Firestore.
*   **Design:** A Plain Old C# Object (POCO).
*   **Attributes:**
    *   `[FirestoreData]` at the class level.
*   **Properties:**
    *   `[FirestoreProperty] public PlayerSettingsDto Settings { get; set; }`
    *   `[FirestoreProperty] public PlayerProgressDto Progress { get; set; }`
    *   `[FirestoreProperty, ServerTimestamp] public Timestamp LastUpdated { get; set; }`
    *   (Sub-DTOs like `PlayerSettingsDto` and `PlayerProgressDto` will be defined similarly).

#### 3.5.2. `CloudSave/Mappers/PlayerProfileMapper.cs`
*   **Purpose:** Decouples domain models from persistence models.
*   **Design:** A static class with mapping methods.
*   **Methods:**
    *   `public static PlayerProfileDto ToDto(PlayerProfile domainModel)`: Converts a `PlayerProfile` object to a `PlayerProfileDto`.
    *   `public static PlayerProfile ToDomain(PlayerProfileDto dto)`: Converts a `PlayerProfileDto` back to a `PlayerProfile`.

### 3.6. Common Utilities

#### 3.6.1. `Common/FirebaseResult.cs`
*   **Purpose:** Standardize the return type of all asynchronous Firebase operations.
*   **Design:** A generic struct or class.
*   **`FirebaseResult<T>`:**
    *   `public bool IsSuccess { get; }`
    *   `public T Value { get; }`
    *   `public FirebaseError Error { get; }`
    *   Private constructor.
    *   Static factory methods: `public static FirebaseResult<T> Success(T value)` and `public static FirebaseResult<T> Failure(FirebaseError error)`.
*   **`FirebaseResult` (non-generic):**
    *   Similar structure but without the `Value` property.

#### 3.6.2. `Common/FirebaseError.cs`
*   **Purpose:** Provide a structured, service-agnostic error object.
*   **Design:** A struct or class.
*   **Properties:**
    *   `public int ErrorCode { get; }` (Can store enum values from Firebase SDKs).
    *   `public string Message { get; }`
    *   `public Exception OriginalException { get; }` (Optional, for logging/debugging).

### 3.7. Dependency Injection Setup
#### `DependencyInjection/FirebaseServiceRegistration.cs`
*   **Purpose:** To provide a single point of configuration for registering all services in this layer.
*   **Design:** A static class with an extension method.
*   **Method:**
    *   `public static IServiceCollection AddFirebaseInfrastructure(this IServiceCollection services)`:
        *   **Logic:**
            *   `services.AddSingleton<IAuthenticationService, FirebaseAuthAdapter>();`
            *   `services.AddSingleton<ICloudSaveService, FirestoreCloudSaveAdapter>();`
            *   `services.AddSingleton<IAnalyticsService, FirebaseAnalyticsAdapter>();`
            *   `services.AddSingleton<IRemoteConfigService, FirebaseRemoteConfigAdapter>();`
            *   `services.AddSingleton<FirebaseServiceFacade>();`

---
## 4. Testing Strategy

*   **Unit Testing:** Each `*Adapter` class will be unit tested. The Firebase SDK dependencies will be mocked using a framework like Moq or NSubstitute. Tests will verify that the adapter correctly calls the mocked SDK methods with the right parameters and correctly handles both success and failure responses from the mock, wrapping them in the `FirebaseResult` object.
*   **Integration Testing:** A separate suite of tests will run within the Unity Editor to test the actual integration with live Firebase services (on a dedicated 'dev' or 'staging' project). These tests will validate the entire flow, from calling the facade to the data appearing correctly in the Firebase console.

---