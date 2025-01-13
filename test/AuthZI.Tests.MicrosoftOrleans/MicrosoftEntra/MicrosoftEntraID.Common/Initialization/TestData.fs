namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AuthZI.Deploy.MicrosoftEntra.Configuration
open AuthZI.Identity.MicrosoftEntra
open Orleans

type public TestData() =
  static member val public UserWithScopeAdeleV: obj[] list = [] with get, set

  static member val public UserWithScopeAlexW: obj[] list = [] with get, set

  static member val public Users: obj[] list = [] with get, set

  static member val public Web1ClientApp: MicrosoftEntraApp = MicrosoftEntraIDApp.EmptyApp with get, set

  static member val public Web2ClientApp: MicrosoftEntraApp = MicrosoftEntraIDApp.EmptyApp with get, set

  static member val IClusterClient: IClusterClient = null with get, set

  static member val GetAccessTokenForUserOnMicrosoftEntraAppAsync: MicrosoftEntraApp -> string -> string -> string =
    fun app user pass -> "" with get, set
