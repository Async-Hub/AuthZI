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

let userName = "name"
let password = "pass"
let securePassword = new SecureString()
password |> Seq.iter (fun char -> securePassword.AppendChar(char))

let app = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(applicationOptions).Build();

let result = app.AcquireTokenByUsernamePassword(microsoftGraphsScopes, 
                userName, securePassword).ExecuteAsync().Result

printfn "%s" result.AccessToken

// Choose a Microsoft Graph authentication provider based on scenario
// https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS
let authProvider = new DelegateAuthenticationProvider(fun request -> 
    request.Headers.Authorization <- new AuthenticationHeaderValue("Bearer", result.AccessToken)
    Task.CompletedTask);

let graphClient = new GraphServiceClient(authProvider)

B2B1.configure(graphClient)