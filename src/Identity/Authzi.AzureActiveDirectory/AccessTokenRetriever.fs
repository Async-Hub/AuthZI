namespace Authzi.AzureActiveDirectory

open Microsoft.Identity.Client
open System.Security

module AccessTokenRetriever =
    // Username/Password flow to acquire a token silently. For testing purposes only!
    // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Username-Password-Authentication
    let getTokenByUserNameAndPassword directoryId clientId scopes userName password: string =
        let appConfig = new PublicClientApplicationOptions()
        appConfig.TenantId <- directoryId
        appConfig.ClientId <- clientId
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

        let securePassword = new SecureString()

        password |> Seq.iter (fun char -> securePassword.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, userName, securePassword).ExecuteAsync().Result

        result.AccessToken
