open MicrosoftEntraExternalIDSetup

[<EntryPoint>]
let main argv =
    RunMicrosoftEntraExternalIDSetup |> Async.RunSynchronously
    0