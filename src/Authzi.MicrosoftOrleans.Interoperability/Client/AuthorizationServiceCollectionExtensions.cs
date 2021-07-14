//using Authzi.MicrosoftOrleans.Authorization;
//using Authzi.Security;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Orleans;
//using System;

//namespace Authzi.MicrosoftOrleans.Client
//{
//    public static class OrleansClusterSecurityServiceCollectionExtensions
//    {
//        // For the testing purposes.
//        internal static void AddOrleansClusteringAuthorization(this IServiceCollection services,
//            Action<Configuration> configure, Action<IServiceCollection> configureServices)
//        {
//            if (services == null)
//            {
//                throw new ArgumentNullException(nameof(services));
//            }

//            if (configure == null)
//            {
//                throw new ArgumentNullException(nameof(configure));
//            }

//            services.TryAddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>();
//            services.TryAddSingleton<IOutgoingGrainCallFilter, OutgoingGrainCallAuthorizationFilter>();
//            services.AddOrleansClusterSecurityServices(configure, configureServices);
//        }

//        // For the production usage.
//        public static void AddOrleansClientAuthorization(this IServiceCollection services,
//            Action<Configuration> configure)
//        {
//            if (services == null)
//            {
//                throw new ArgumentNullException(nameof(services));
//            }

//            if (configure == null)
//            {
//                throw new ArgumentNullException(nameof(configure));
//            }

//            services.AddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>();
//            services.AddSingleton<IOutgoingGrainCallFilter, OutgoingGrainCallAuthorizationFilter>();
//            services.AddOrleansClusterSecurityServices(configure);
//        }
//    }
//}