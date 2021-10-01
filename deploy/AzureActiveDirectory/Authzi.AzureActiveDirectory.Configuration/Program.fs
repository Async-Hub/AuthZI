open Microsoft.Identity.Client
open System.Security

let applicationOptions = new PublicClientApplicationOptions()

applicationOptions.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
applicationOptions.TenantId <- "xxx"
applicationOptions.ClientId <- "xxx"

let app = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(applicationOptions).Build();

let userName = "xxx@xxx.onmicrosoft.com"
let password = "pass"
let securePassword = new SecureString()

password |> Seq.iter (fun char -> securePassword.AppendChar(char))

let result = app.AcquireTokenByUsernamePassword(["https://graph.microsoft.com/User.Read"], 
                userName, securePassword).ExecuteAsync().Result

printfn "%s" result.AccessToken