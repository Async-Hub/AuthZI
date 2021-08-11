namespace Authzi.AzureActiveDirectory

open Microsoft.Identity.Client
open System.Security

module AccessTokenRetriever =
    // Username/Password flow to acquire a token silently. For testing purposes only!
    // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Username-Password-Authentication
    let getTokenByUserNameAndPassword clientId scopes userName password: string =
        let appConfig = new PublicClientApplicationOptions()
        appConfig.ClientId <- clientId
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMultipleOrgs
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

        let securePassword = new SecureString()

        password |> Seq.iter (fun char -> securePassword.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, userName, securePassword).ExecuteAsync().Result

        result.AccessToken
