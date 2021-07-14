namespace Authzi.AzureActiveDirectory

open System.Security
open System.Collections.Generic
open Microsoft.Identity.Client

module AccessTokenProvider =
    let getToken =
        let appConfig = new PublicClientApplicationOptions()
        
        appConfig.ClientId <- "xxx-f25dba1f2de2"
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMultipleOrgs
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(appConfig)
                    .Build();

        let scopes = new List<string>()
        let password = new SecureString()

        "xxx" |> Seq.iter (fun char -> password.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, 
                        "xxx", password).ExecuteAsync().Result

        result.AccessToken
