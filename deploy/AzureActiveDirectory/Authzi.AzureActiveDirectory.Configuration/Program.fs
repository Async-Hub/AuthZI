open Microsoft.Identity.Client
open System.Security

let applicationOptions = new PublicClientApplicationOptions()

applicationOptions.AadAuthorityAudience <- AadAuthorityAudience.AzureAdMyOrg
applicationOptions.TenantId <- "1c59e6e7-0a62-4dfe-bfbc-38aeb089b0b9"
applicationOptions.ClientId <- "218bd76c-5f54-43d0-a564-88164cc9f398"

let app = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(applicationOptions).Build();

let userName = "xxx@xxx.onmicrosoft.com"
let password = "pass"
let securePassword = new SecureString()

password |> Seq.iter (fun char -> securePassword.AppendChar(char))

let result = app.AcquireTokenByUsernamePassword(["https://graph.microsoft.com/User.Read"], 
                userName, securePassword).ExecuteAsync().Result

printfn "%s" result.AccessToken