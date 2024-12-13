namespace AuthZI.MicrosoftOrleans.Authorization

type OrleansAuthorizationConfiguration(isCoHostingEnabled: bool) =
    member val IsCoHostingEnabled = isCoHostingEnabled with get
