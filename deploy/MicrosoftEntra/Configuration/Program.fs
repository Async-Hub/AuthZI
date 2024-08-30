open Authzi.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID

[<EntryPoint>]
let main argv =
    //TenantCreation.start |> Async.RunSynchronously
    TenantConfiguration.start |> Async.RunSynchronously
    0