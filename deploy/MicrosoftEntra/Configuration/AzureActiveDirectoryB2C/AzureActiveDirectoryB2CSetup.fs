module AzureActiveDirectoryB2CSetup

open Microsoft.Graph
open Microsoft.Identity.Client
open System.Net.Http.Headers
open System.Security
open System.Threading.Tasks
open Authzi.Deploy.MicrosoftEntra.Configuration

let microsoftGraphsScopes = ["https://graph.microsoft.com/User.Read"; 
        "https://graph.microsoft.com/Application.ReadWrite.All"]

let applicationOptions = new PublicClientApplicationOptions()

applicationOptions.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
applicationOptions.TenantId <- "1c59e6e7-xxx"
applicationOptions.ClientId <- "218bd76c-xxx"

let userName = "userName"
let password = "password"

let app = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(applicationOptions).Build();

let result = app.AcquireTokenByUsernamePassword(microsoftGraphsScopes, 
                userName, password).ExecuteAsync().Result

printfn "%s" result.AccessToken

let graphClient = MicrosoftGraph.getGraphServiceClient result.AccessToken

//B2B1.configure(graphClient)
