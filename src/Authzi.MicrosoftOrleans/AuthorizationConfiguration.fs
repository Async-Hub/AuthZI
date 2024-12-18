namespace AuthZI.MicrosoftOrleans.Authorization

type AuthorizationConfiguration(isCoHostingEnabled: bool) =
    member val IsCoHostingEnabled = isCoHostingEnabled with get