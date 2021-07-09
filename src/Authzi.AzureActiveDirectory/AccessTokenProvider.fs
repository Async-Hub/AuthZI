namespace Authzi.AzureActiveDirectory

open Microsoft.Identity.Client
open System
open System.Collections.Generic
open System.Security

module AccessTokenProvider =
    let getToken =
        let appConfig = new PublicClientApplicationOptions()
        
        appConfig.ClientId <- "xxx-eaef-xxx-f25dba1fxxx"
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMultipleOrgs
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(appConfig)
                    .Build();

        let scopes = new List<string>()
        let password = new SecureString()

        "Passw@rd+1" |> Seq.iter (fun char -> password.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, 
                        "xxx@xxx.onmicrosoft.com", password).ExecuteAsync().Result

        result.AccessToken
