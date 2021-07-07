using Authzi.MicrosoftOrleans.Authorization;
using Authzi.Security;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System;

namespace Authzi.MicrosoftOrleans.Clustering
{
    public static class ServiceCollectionExtensions
    {
        // For the testing purposes.
        internal static void AddOrleansClusteringAuthorization(this IServiceCollection services,
            Action<Configuration> configure, Action<IServiceCollection> configureServices)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.AddSingleton<IIncomingGrainCallFilter, IncomingGrainCallAuthorizationFilter>();
            services.AddOrleansClusterSecurityServices(configure, configureServices);
        }

        // For the production usage.
        public static void AddOrleansClusteringAuthorization(this IServiceCollection services,
            Action<Configuration> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.AddSingleton<IIncomingGrainCallFilter, IncomingGrainCallAuthorizationFilter>();
            services.AddOrleansClusterSecurityServices(configure);
        }
        
        public static void AddOrleansCoHostedClusterAuthorization(this IServiceCollection services,
            Action<Configuration> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.AddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>();
            services.AddSingleton<IIncomingGrainCallFilter, IncomingGrainCallAuthorizationFilter>();
            services.AddOrleansClusterSecurityServices(configure);
        }
    }
}