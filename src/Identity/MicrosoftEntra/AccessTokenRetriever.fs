namespace Authzi.MicrosoftEntra

open Microsoft.Identity.Client
open System.Security

module AccessTokenRetriever =
    // Username/Password flow to acquire a token silently. For testing purposes only!
    // https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth-ropc
    let getTokenByUserNameAndPassword directoryId clientId scopes userName password: string =
        let appConfig = new PublicClientApplicationOptions()
        appConfig.TenantId <- directoryId
        appConfig.ClientId <- clientId
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

        //let securePassword = new SecureString()
        //password |> Seq.iter (fun char -> securePassword.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, userName, password.ToString()).ExecuteAsync().Result

        result.AccessToken
