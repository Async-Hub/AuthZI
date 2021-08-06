namespace Authzi.AzureActiveDirectory

open Microsoft.Identity.Client
open System.Collections.Generic
open System.Security

module AccessTokenRetriever =
    let getTokenByUserNameAndPassword clientId userName password: string =
        let appConfig = new PublicClientApplicationOptions()
        appConfig.ClientId <- clientId
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMultipleOrgs
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

        let scopes = new List<string>()
        let securePassword = new SecureString()

        password |> Seq.iter (fun char -> securePassword.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, userName, securePassword).ExecuteAsync().Result

        result.AccessToken
