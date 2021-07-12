namespace Authzi.AzureActiveDirectory

open Microsoft.Identity.Client
open System
open System.Collections.Generic
open System.Security

module AccessTokenProvider =
    let getToken =
        let appConfig = new PublicClientApplicationOptions()
        
        appConfig.ClientId <- "e64ef6f7-eaef-4e43-92af-f25dba1f2de2"
        appConfig.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMultipleOrgs
        appConfig.AzureCloudInstance <- AzureCloudInstance.AzurePublic

        let app = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(appConfig)
                    .Build();

        let scopes = new List<string>()
        let password = new SecureString()

        "Passw@rd+1" |> Seq.iter (fun char -> password.AppendChar(char))

        let result = app.AcquireTokenByUsernamePassword(scopes, 
                        "AdeleV@asynchub.onmicrosoft.com", password).ExecuteAsync().Result

        result.AccessToken
