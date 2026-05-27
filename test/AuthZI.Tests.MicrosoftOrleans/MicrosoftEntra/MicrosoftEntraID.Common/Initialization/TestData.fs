namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AuthZI.Deploy.MicrosoftEntra.Configuration
open AuthZI.Identity.MicrosoftEntra
open Orleans
open System
open System.Text.Json

type public TestData() =
  static member val public UserWithScopeAdeleV: obj[] list = [] with get, set

  static member val public UserWithScopeAlexW: obj[] list = [] with get, set

  static member val public Users: obj[] list = [] with get, set

  static member val public Web1ClientApp: MicrosoftEntraApp = MicrosoftEntraIDApp.EmptyApp with get, set

  static member val public Web2ClientApp: MicrosoftEntraApp = MicrosoftEntraIDApp.EmptyApp with get, set

  static member val IClusterClient: IClusterClient = null with get, set

  static member val public GetAccessTokenForUserOnMicrosoftEntraAppAsync: MicrosoftEntraApp -> string -> string -> string =
    fun app user pass -> "" with get, set

module TestDataInitialization =
  let getCredentialsJson (environmentVariableName: string) (fallbackJson: string) =
    let credentialsJson = Environment.GetEnvironmentVariable(environmentVariableName)

    if String.IsNullOrWhiteSpace(credentialsJson) then
      fallbackJson
    else
      credentialsJson

  let private initializeUsers (credentials: MicrosoftEntraCredentials) =
    TestData.UserWithScopeAdeleV <-
      [ [| credentials.AdeleV.Name; credentials.AdeleV.Password; [ "Api1"; "Orleans" ] |] ]

    TestData.UserWithScopeAlexW <-
      [ [| credentials.AlexW.Name; credentials.AlexW.Password; [ "Api1"; "Orleans" ] |] ]

    TestData.Users <- [ [| credentials.AdeleV.Name; credentials.AdeleV.Password |] ]

  let initializeUsersOnly (credentialsJson: string) =
    let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(credentialsJson)
    initializeUsers credentials

  let initialize
    (credentialsJson: string)
    (createWeb1ClientApp: MicrosoftEntraCredentials -> MicrosoftEntraApp)
    (createWeb2ClientApp: MicrosoftEntraCredentials -> MicrosoftEntraApp)
    tokenRetriever
    =
    let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(credentialsJson)

    initializeUsers credentials
    TestData.Web1ClientApp <- createWeb1ClientApp credentials
    TestData.Web2ClientApp <- createWeb2ClientApp credentials
    TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync <- tokenRetriever
