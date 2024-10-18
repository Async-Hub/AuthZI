namespace AuthZI.Identity.DuendeSoftware.IdentityServer

open AuthZI.Security
open AuthZI.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Net.Http
open Microsoft.Extensions.Http
open System.Security.Cryptography.X509Certificates
open System.Net.Security

module HttpClientConfiguration =
    let configureHttpMessageHandlerBuilder (builder: HttpMessageHandlerBuilder, securityOptions: SecurityOptions) =
        let httpClientHandler = new HttpClientHandler()
        builder.PrimaryHandler <- httpClientHandler

        if not securityOptions.RequireHttps then
            httpClientHandler.ServerCertificateCustomValidationCallback <- 
                Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>(fun _ _ _ _ -> true)

type ServiceCollectionExtensions = 
    [<Extension>]
    static member AddIdentityServerAuthorization(services: IServiceCollection,
        identityServerConfig: IdentityServerConfig, securityOptions: SecurityOptions) =
            if isNull (box identityServerConfig) then nullArg(nameof identityServerConfig)
            //if isNull configure then nullArg(nameof configure)
            
            services.AddSingleton(identityServerConfig) |> ignore
            services.AddSingleton<IClaimTypeResolver, ClaimTypeResolver>() |> ignore
            services.AddTransient<IAccessTokenIntrospectionService, AccessTokenIntrospectionService>() |> ignore

            //services.AddHttpClient("IdS4").ConfigureHttpMessageHandlerBuilder(fun builder -> 
            //    HttpClientConfiguration.configureHttpMessageHandlerBuilder(builder,securityOptions)) |> ignore

            let configureHandler (securityOptions: SecurityOptions) =
                let httpClientHandler = new HttpClientHandler()
                if not securityOptions.RequireHttps then
                   httpClientHandler.ServerCertificateCustomValidationCallback <- 
                    Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>(fun _ _ _ _ -> true)
                httpClientHandler :> HttpMessageHandler

            services.AddHttpClient("IdS4")
                    .ConfigurePrimaryHttpMessageHandler(fun () -> configureHandler(securityOptions)) |>ignore
        
            // Add Discovery document provider.
            let providerFunc (provider: IServiceProvider) =
                let securityOptions = provider.GetRequiredService<SecurityOptions>()
                let httpClientFactory = provider.GetRequiredService<IHttpClientFactory>()
                let discoveryEndpointUrl = provider.GetRequiredService<IdentityServerConfig>().DiscoveryEndpointUrl

                DiscoveryDocumentProvider(httpClientFactory, discoveryEndpointUrl, securityOptions)

            services.AddSingleton<DiscoveryDocumentProvider>(fun provider -> providerFunc(provider)) |> ignore