namespace AuthZI.Deploy.MicrosoftEntra.Configuration

open Microsoft.Graph
open Microsoft.Identity.Client
open Microsoft.Kiota.Abstractions.Authentication
open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

module MicrosoftGraph =
    let getGraphServiceClient accessToken =

        // Microsoft Graph .NET Client Library upgrading to v5
        // https://github.com/microsoftgraph/msgraph-sdk-dotnet/blob/main/docs/upgrade-to-v5.md#authentication

        let tokenProvider accessToken = 
            {
                new IAccessTokenProvider with
                    member this.GetAuthorizationTokenAsync(uri: Uri, 
                        additionalAuthenticationContext: Dictionary<string,obj>, cancellationToken: CancellationToken) =
                        Task.FromResult(accessToken)

                    member this.AllowedHostsValidator: AllowedHostsValidator = null
            }

        // Choose a Microsoft Graph authentication provider based on scenario
        // https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS
        let authenticationProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider accessToken);
        new GraphServiceClient(authenticationProvider);