module AccessToken

open Microsoft.Identity.Client
open System.Security

/// Represents the configuration needed to acquire an access token.
type AccessTokenConfig = {
    /// The tenant ID of the Azure AD.
    TenantId: string
    /// The client ID of the application.
    ClientId: string
    /// The username for authentication.
    UserName: string
    /// The password for authentication.
    Password: string
    /// The scopes required for the access token.
    Scopes: string list
}

let getAccessTokenByUsernamePassword (config: AccessTokenConfig) =
    async {
        let applicationOptions = new PublicClientApplicationOptions()
        applicationOptions.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
        applicationOptions.TenantId <- config.TenantId
        applicationOptions.ClientId <- config.ClientId

        let app = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(applicationOptions)
                    .Build()

        let! result = app.AcquireTokenByUsernamePassword(config.Scopes, config.UserName, config.Password)
                        .ExecuteAsync() |> Async.AwaitTask
        
        return result.AccessToken
    }