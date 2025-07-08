using PatternCipher.Application.Services;

namespace PatternCipher.Infrastructure.Firebase
{
    /// <summary>
    /// Acts as the single, simplified entry point for all backend services.
    /// It aggregates all individual backend service adapters and exposes them through a unified API,
    /// decoupling the application from the specifics of the Firebase SDKs.
    /// </summary>
    public class FirebaseServiceFacade
    {
        /// <summary>
        /// Gets the authentication service for user sign-in, sign-out, and state management.
        /// </summary>
        public IAuthenticationService Auth { get; }

        /// <summary>
        /// Gets the cloud save service for persisting player data to the cloud.
        /// </summary>
        public ICloudSaveService CloudSave { get; }

        /// <summary>
        /// Gets the leaderboard service for submitting and retrieving scores.
        /// </summary>
        public ILeaderboardService Leaderboards { get; }

        /// <summary>
        /// Gets the analytics service for logging custom events.
        /// </summary>
        public IAnalyticsService Analytics { get; }

        /// <summary>
        /// Gets the remote configuration service for fetching dynamic game parameters.
        /// </summary>
        public IRemoteConfigService RemoteConfig { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseServiceFacade"/> class.
        /// This constructor is designed to be used with a dependency injection container.
        /// </summary>
        /// <param name="auth">The concrete implementation of the authentication service.</param>
        /// <param name="cloudSave">The concrete implementation of the cloud save service.</param>
        /// <param name="leaderboards">The concrete implementation of the leaderboard service.</param>
        /// <param name="analytics">The concrete implementation of the analytics service.</param>
        /// <param name="remoteConfig">The concrete implementation of the remote config service.</param>
        public FirebaseServiceFacade(
            IAuthenticationService auth,
            ICloudSaveService cloudSave,
            ILeaderboardService leaderboards,
            IAnalyticsService analytics,
            IRemoteConfigService remoteConfig)
        {
            Auth = auth;
            CloudSave = cloudSave;
            Leaderboards = leaderboards;
            Analytics = analytics;
            RemoteConfig = remoteConfig;
        }
    }
}