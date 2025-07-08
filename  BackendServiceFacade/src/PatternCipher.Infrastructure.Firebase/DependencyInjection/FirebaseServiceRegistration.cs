using Microsoft.Extensions.DependencyInjection;
using PatternCipher.Application.Services;
using PatternCipher.Infrastructure.Firebase.Analytics;
using PatternCipher.Infrastructure.Firebase.Authentication;
using PatternCipher.Infrastructure.Firebase.CloudSave;
using PatternCipher.Infrastructure.Firebase.RemoteConfig;
using System;

namespace PatternCipher.Infrastructure.Firebase.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for registering all Firebase infrastructure services
    /// into a dependency injection container.
    /// </summary>
    public static class FirebaseServiceRegistration
    {
        /// <summary>
        /// Adds all Firebase-related infrastructure services as singletons to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddFirebaseInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationService, FirebaseAuthAdapter>();
            services.AddSingleton<ICloudSaveService, FirestoreCloudSaveAdapter>();
            services.AddSingleton<IAnalyticsService, FirebaseAnalyticsAdapter>();
            services.AddSingleton<IRemoteConfigService, FirebaseRemoteConfigAdapter>();

            // The Leaderboard service implementation is defined elsewhere.
            // For now, we can register a dummy implementation to satisfy the facade's dependency.
            // In a real scenario, this would be replaced with the actual implementation.
            services.AddSingleton<ILeaderboardService, DummyLeaderboardService>();

            // Register the facade itself, which aggregates all the above services.
            services.AddSingleton<FirebaseServiceFacade>();

            return services;
        }
    }

    /// <summary>
    /// A placeholder implementation of the leaderboard service.
    /// This should be replaced with the actual implementation when available.
    /// </summary>
    internal class DummyLeaderboardService : ILeaderboardService
    {
        // Dummy implementations of the interface methods.
    }
}