namespace Authzi.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID

open Authzi.Deploy.MicrosoftEntra.Configuration
open Authzi.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraUsers
open Microsoft.Graph
open Microsoft.Graph.Models

module Applications =
    let createAnApplicationIfNotExists (graphClient : GraphServiceClient) 
        (displayName : string) applications =
        async {
            if not (Seq.exists (fun (app: Application) -> app.DisplayName = displayName) applications) then
                let application = new Application()
                application.DisplayName <- displayName
                let! result = graphClient.Applications.PostAsync(application) |> Async.AwaitTask
                printfn "%s is created: %s" displayName result.AppId
            else
                printfn "Application: %s already exists" displayName
        }

    let createUserIfNotExists (graphClient : GraphServiceClient) (user: MicrosoftEntraUser) tenatName users =
        async {
            if not (Seq.exists (fun (u: User) -> u.UserPrincipalName.StartsWith(user.MailNickname)) users) then
                let newUser = new User()
                newUser.UserPrincipalName <- user.MailNickname
                newUser.AccountEnabled <- true
                newUser.DisplayName <- user.DisplayName
                newUser.MailNickname <- user.MailNickname
                newUser.PasswordProfile <- PasswordProfile(
                    Password = user.Password,
                    ForceChangePasswordNextSignIn = false
                )
                newUser.UserPrincipalName <- $"{user.MailNickname}@{tenatName}.onmicrosoft.com"

                let! result = graphClient.Users.PostAsync(newUser) |> Async.AwaitTask
                printfn "User is created: %s" result.UserPrincipalName
            else
                printfn "User: %s already exists" user.MailNickname
        }

    let configure(graphClient : GraphServiceClient) tenantName =
        async {

            let! result = graphClient.Applications.GetAsync() |> Async.AwaitTask
            let applications = result.Value

            printfn "Existing applications list:"
            applications |> Seq.iter (fun app -> printfn "Display Name: %s" app.DisplayName)
            
            do! (createAnApplicationIfNotExists graphClient Clients.api1 applications)
            do! (createAnApplicationIfNotExists graphClient Clients.webClient1 applications)

            let! result = graphClient.Users.GetAsync() |> Async.AwaitTask
            let users = result.Value

            printfn "Existing users list:"
            users |> Seq.iter (fun user -> printfn "User Principal Name: %s" user.UserPrincipalName)

            do! (createUserIfNotExists graphClient adeleV tenantName users)
            do! (createUserIfNotExists graphClient alexW tenantName users)
        }