# Specification

# 1. Files

- **Path:** src/PatternCipher.Infrastructure.Firebase/PatternCipher.Infrastructure.Firebase.csproj  
**Description:** The C# project file for the BackendServiceFacade repository. It defines dependencies on the Firebase SDKs for Unity, other project layers (like Domain/Application for interfaces), and project settings.  
**Template:** C# Project File  
**Dependency Level:** 0  
**Name:** PatternCipher.Infrastructure.Firebase  
**Type:** Project  
**Relative Path:** PatternCipher.Infrastructure.Firebase.csproj  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Project Configuration
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** Defines the project structure, dependencies, and build settings for the Firebase infrastructure layer.  
**Logic Description:** This file will list PackageReference items for all required Firebase SDKs (Authentication, Firestore, Remote Config, Analytics), and ProjectReference items for the core Domain/Application layers where service interfaces are defined.  
**Documentation:**
    
    - **Summary:** Manages the compilation and dependency settings for the Firebase service facade.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/FirebaseInitializer.cs  
**Description:** A Unity MonoBehaviour script responsible for initializing the Firebase application at the start of the game. It handles checking for dependencies and safely initializing the Firebase SDK.  
**Template:** C# Unity MonoBehaviour  
**Dependency Level:** 1  
**Name:** FirebaseInitializer  
**Type:** Service  
**Relative Path:** FirebaseInitializer.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    - Singleton
    
**Members:**
    
    - **Name:** dependencyStatus  
**Type:** Firebase.DependencyStatus  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** Awake  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** InitializeFirebase  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Firebase SDK Initialization
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To ensure the Firebase SDK is ready for use before any other service tries to access it.  
**Logic Description:** In the Awake method, use Firebase.FirebaseApp.CheckAndFixDependenciesAsync to check for required Google Play services on Android. Upon completion, set a flag or invoke an event to signal that Firebase is ready. This script should be attached to a persistent GameObject in the initial scene.  
**Documentation:**
    
    - **Summary:** Handles the asynchronous initialization of the Firebase application, a prerequisite for all other Firebase services.
    
**Namespace:** PatternCipher.Infrastructure.Firebase  
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/FirebaseServiceFacade.cs  
**Description:** The central facade class that provides a single, unified interface to all Firebase backend services. It aggregates the different service adapters (Auth, Cloud Save, etc.) and exposes them to the Application layer.  
**Template:** C# Service  
**Dependency Level:** 2  
**Name:** FirebaseServiceFacade  
**Type:** Service  
**Relative Path:** FirebaseServiceFacade.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    - FacadePattern
    
**Members:**
    
    - **Name:** Auth  
**Type:** IAuthenticationService  
**Attributes:** public|readonly  
    - **Name:** CloudSave  
**Type:** ICloudSaveService  
**Attributes:** public|readonly  
    - **Name:** Leaderboards  
**Type:** ILeaderboardService  
**Attributes:** public|readonly  
    - **Name:** Analytics  
**Type:** IAnalyticsService  
**Attributes:** public|readonly  
    - **Name:** RemoteConfig  
**Type:** IRemoteConfigService  
**Attributes:** public|readonly  
    
**Methods:**
    
    - **Name:** FirebaseServiceFacade  
**Parameters:**
    
    - IAuthenticationService authService
    - ICloudSaveService cloudSaveService
    - ILeaderboardService leaderboardService
    - IAnalyticsService analyticsService
    - IRemoteConfigService remoteConfigService
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Backend Service Aggregation
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To simplify interaction with backend services for the rest of the application and act as a single point of entry.  
**Logic Description:** This class will be instantiated by a Dependency Injection container. Its constructor will receive instances of each specific service adapter (e.g., FirebaseAuthAdapter, FirestoreCloudSaveAdapter). It will expose these services through public properties, hiding the complexity of their individual management.  
**Documentation:**
    
    - **Summary:** Acts as a centralized access point for all backend functionalities, abstracting away the underlying Firebase implementations.
    
**Namespace:** PatternCipher.Infrastructure.Firebase  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/Authentication/FirebaseAuthAdapter.cs  
**Description:** The concrete implementation of the authentication service interface using the Firebase Authentication SDK. It handles all user sign-in, sign-out, and state-changed events.  
**Template:** C# Service  
**Dependency Level:** 1  
**Name:** FirebaseAuthAdapter  
**Type:** Adapter  
**Relative Path:** Authentication/FirebaseAuthAdapter.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    - AdapterPattern
    
**Members:**
    
    - **Name:** firebaseAuthInstance  
**Type:** FirebaseAuth  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** SignInAnonymouslyAsync  
**Parameters:**
    
    
**Return Type:** Task<FirebaseResult<FirebaseUser>>  
**Attributes:** public  
    - **Name:** SignInWithGoogleAsync  
**Parameters:**
    
    - string idToken
    
**Return Type:** Task<FirebaseResult<FirebaseUser>>  
**Attributes:** public  
    - **Name:** SignInWithAppleAsync  
**Parameters:**
    
    - string idToken
    
**Return Type:** Task<FirebaseResult<FirebaseUser>>  
**Attributes:** public  
    - **Name:** SignOut  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** GetCurrentUser  
**Parameters:**
    
    
**Return Type:** FirebaseUser  
**Attributes:** public  
    
**Implemented Features:**
    
    - User Authentication
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To abstract the Firebase Authentication SDK and provide authentication features to the application.  
**Logic Description:** This class will implement the IAuthenticationService interface (defined in Application layer). Methods will wrap the corresponding Firebase Auth SDK calls (e.g., `SignInAnonymouslyAsync`). It will catch Firebase-specific exceptions and wrap results in the custom `FirebaseResult` object for consistent error handling.  
**Documentation:**
    
    - **Summary:** Handles user authentication logic by adapting the application's authentication interface to the Firebase Authentication service.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.Authentication  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/CloudSave/FirestoreCloudSaveAdapter.cs  
**Description:** Implements the cloud save service interface using Cloud Firestore. It is responsible for serializing domain models into Firestore documents and vice-versa.  
**Template:** C# Service  
**Dependency Level:** 1  
**Name:** FirestoreCloudSaveAdapter  
**Type:** Adapter  
**Relative Path:** CloudSave/FirestoreCloudSaveAdapter.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    - AdapterPattern
    
**Members:**
    
    - **Name:** firestoreDb  
**Type:** FirebaseFirestore  
**Attributes:** private|readonly  
    - **Name:** playerProfileMapper  
**Type:** PlayerProfileMapper  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** SavePlayerProfileAsync  
**Parameters:**
    
    - string userId
    - PlayerProfile profile
    
**Return Type:** Task<FirebaseResult>  
**Attributes:** public  
    - **Name:** LoadPlayerProfileAsync  
**Parameters:**
    
    - string userId
    
**Return Type:** Task<FirebaseResult<PlayerProfile>>  
**Attributes:** public  
    
**Implemented Features:**
    
    - Cloud Data Persistence
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To provide a reliable mechanism for saving and loading player progress to and from the cloud.  
**Logic Description:** This class will implement the ICloudSaveService interface. The `SavePlayerProfileAsync` method will use a mapper to convert the domain `PlayerProfile` object into a `PlayerProfileDto` or dictionary, then write it to a Firestore document under a specific user's path. `LoadPlayerProfileAsync` will read the document and map it back. It will manage network errors and permissions issues, returning a `FirebaseResult`.  
**Documentation:**
    
    - **Summary:** Adapts the application's cloud save interface to use Cloud Firestore for persistent storage of player data.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.CloudSave  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/CloudSave/Mappers/PlayerProfileMapper.cs  
**Description:** A helper class responsible for mapping between the domain `PlayerProfile` model and the `PlayerProfileDto` used for Firestore persistence.  
**Template:** C# Class  
**Dependency Level:** 0  
**Name:** PlayerProfileMapper  
**Type:** Mapper  
**Relative Path:** CloudSave/Mappers/PlayerProfileMapper.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** ToDto  
**Parameters:**
    
    - PlayerProfile domainModel
    
**Return Type:** PlayerProfileDto  
**Attributes:** public|static  
    - **Name:** ToDomain  
**Parameters:**
    
    - PlayerProfileDto dto
    
**Return Type:** PlayerProfile  
**Attributes:** public|static  
    
**Implemented Features:**
    
    - Data Mapping
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To decouple the domain model from the persistence model, allowing them to evolve independently.  
**Logic Description:** This static class will contain methods to perform a field-by-field mapping from the domain object to the DTO and vice-versa. This prevents persistence-specific attributes (like Firestore's `[FirestoreData]`) from polluting the domain model.  
**Documentation:**
    
    - **Summary:** Provides mapping logic between the core domain PlayerProfile object and its data transfer representation for Firestore.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.CloudSave.Mappers  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/CloudSave/DTOs/PlayerProfileDto.cs  
**Description:** Data Transfer Object representing the player profile as it is stored in Cloud Firestore. May include Firestore-specific attributes.  
**Template:** C# DTO  
**Dependency Level:** 0  
**Name:** PlayerProfileDto  
**Type:** DTO  
**Relative Path:** CloudSave/DTOs/PlayerProfileDto.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** Settings  
**Type:** PlayerSettingsDto  
**Attributes:** public  
    - **Name:** Progress  
**Type:** PlayerProgressDto  
**Attributes:** public  
    - **Name:** LastUpdated  
**Type:** Timestamp  
**Attributes:** public  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Cloud Save Data Structure
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To define the exact data structure for player profiles stored in Firestore.  
**Logic Description:** A plain C# class with properties that match the fields of the Firestore document. It might be decorated with attributes like `[FirestoreData]` to guide the serialization process.  
**Documentation:**
    
    - **Summary:** Defines the data contract for a player's profile in the Firestore database.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.CloudSave.DTOs  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/Analytics/FirebaseAnalyticsAdapter.cs  
**Description:** Implementation of the analytics service interface. It adapts the application's need to log events to the specific API calls of the Firebase Analytics SDK.  
**Template:** C# Service  
**Dependency Level:** 1  
**Name:** FirebaseAnalyticsAdapter  
**Type:** Adapter  
**Relative Path:** Analytics/FirebaseAnalyticsAdapter.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    - AdapterPattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** LogEvent  
**Parameters:**
    
    - string eventName
    - Dictionary<string, object> parameters
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Analytics Event Logging
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To provide a simple, unified way for the application to log analytics events without being coupled to Firebase.  
**Logic Description:** Implements the IAnalyticsService interface. The `LogEvent` method will take a generic event name and a dictionary of parameters, convert them into the format expected by `FirebaseAnalytics.LogEvent()`, and make the call. It will handle the translation between application-defined event structures and Firebase's parameter types.  
**Documentation:**
    
    - **Summary:** Adapts the application's analytics interface to the Firebase Analytics SDK for logging player telemetry.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.Analytics  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/RemoteConfig/FirebaseRemoteConfigAdapter.cs  
**Description:** Implementation of the remote configuration service interface. It fetches, caches, and activates configuration values from the Firebase Remote Config service.  
**Template:** C# Service  
**Dependency Level:** 1  
**Name:** FirebaseRemoteConfigAdapter  
**Type:** Adapter  
**Relative Path:** RemoteConfig/FirebaseRemoteConfigAdapter.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    - AdapterPattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** InitializeAsync  
**Parameters:**
    
    - Dictionary<string, object> defaults
    
**Return Type:** Task  
**Attributes:** public  
    - **Name:** GetString  
**Parameters:**
    
    - string key
    
**Return Type:** string  
**Attributes:** public  
    - **Name:** GetBool  
**Parameters:**
    
    - string key
    
**Return Type:** bool  
**Attributes:** public  
    - **Name:** GetInt  
**Parameters:**
    
    - string key
    
**Return Type:** int  
**Attributes:** public  
    
**Implemented Features:**
    
    - Dynamic Game Configuration
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To allow for dynamic tuning of game parameters without requiring a client update.  
**Logic Description:** Implements the IRemoteConfigService interface. `InitializeAsync` will set default values, then fetch and activate the latest values from the Firebase backend. The `Get...` methods will retrieve the activated values from the Firebase SDK. This class will manage the fetch/activate lifecycle and caching behavior as configured.  
**Documentation:**
    
    - **Summary:** Adapts the application's configuration interface to fetch and provide values from Firebase Remote Config.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.RemoteConfig  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/Common/FirebaseResult.cs  
**Description:** A generic wrapper class for the results of Firebase operations. It encapsulates success or failure status and provides access to the result data or an error object.  
**Template:** C# Class  
**Dependency Level:** 0  
**Name:** FirebaseResult  
**Type:** Utility  
**Relative Path:** Common/FirebaseResult.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** IsSuccess  
**Type:** bool  
**Attributes:** public|readonly  
    - **Name:** Error  
**Type:** FirebaseError  
**Attributes:** public|readonly  
    - **Name:** Value  
**Type:** T  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Consistent Error Handling
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To standardize how asynchronous backend operations report their outcomes to the application layer.  
**Logic Description:** A generic class `FirebaseResult<T>` that can hold a value of type T on success, or a `FirebaseError` object on failure. A non-generic version `FirebaseResult` can be used for operations that don't return a value. Static factory methods like `Success(T value)` and `Failure(FirebaseError error)` will be used for easy instantiation.  
**Documentation:**
    
    - **Summary:** A data structure to represent the outcome of a Firebase operation, indicating success with a value or failure with an error.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.Common  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/Common/FirebaseError.cs  
**Description:** A custom error class to encapsulate error details from Firebase SDK operations, such as error codes and messages.  
**Template:** C# Class  
**Dependency Level:** 0  
**Name:** FirebaseError  
**Type:** Model  
**Relative Path:** Common/FirebaseError.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** ErrorCode  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** Message  
**Type:** string  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Error Details
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To provide a structured way of passing error information from the infrastructure layer to the application layer.  
**Logic Description:** A simple data class holding an error code (e.g., from a Firebase.Auth.AuthError enum) and a descriptive message. This allows the application layer to handle specific errors without needing a direct reference to the Firebase SDK's exception types.  
**Documentation:**
    
    - **Summary:** Represents a specific error returned from a Firebase service.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.Common  
**Metadata:**
    
    - **Category:** Infrastructure
    
- **Path:** src/PatternCipher.Infrastructure.Firebase/DependencyInjection/FirebaseServiceRegistration.cs  
**Description:** Contains extension methods for a DI container (e.g., VContainer, Zenject, or Microsoft.Extensions.DependencyInjection) to register all Firebase services and adapters.  
**Template:** C# Class  
**Dependency Level:** 2  
**Name:** FirebaseServiceRegistration  
**Type:** Configuration  
**Relative Path:** DependencyInjection/FirebaseServiceRegistration.cs  
**Repository Id:** REPO-PATT-005  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** AddFirebaseInfrastructure  
**Parameters:**
    
    - this IServiceCollection services
    
**Return Type:** IServiceCollection  
**Attributes:** public|static  
    
**Implemented Features:**
    
    - Dependency Injection Setup
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To centralize the registration of all infrastructure components, simplifying the application's startup configuration.  
**Logic Description:** A static class with an extension method. This method will register `FirebaseServiceFacade` and all its underlying adapter implementations (e.g., `IAuthenticationService` to `FirebaseAuthAdapter`) with the DI container, typically as singletons.  
**Documentation:**
    
    - **Summary:** Provides a single point for registering all Firebase-related infrastructure services into the application's dependency injection container.
    
**Namespace:** PatternCipher.Infrastructure.Firebase.DependencyInjection  
**Metadata:**
    
    - **Category:** Configuration
    


---

# 2. Configuration

- **Feature Toggles:**
  
  - EnableCloudSave
  - EnableLeaderboards
  - EnableAchievements
  - EnableAnalytics
  
- **Database Configs:**
  
  


---

