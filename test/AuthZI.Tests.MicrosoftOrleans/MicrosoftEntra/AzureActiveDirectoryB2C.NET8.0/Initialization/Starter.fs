namespace Initialization

open AuthZI.MicrosoftEntra
open AuthZI.Deploy.MicrosoftEntra.Configuration
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open RootConfiguration
open System.Threading.Tasks

[<assembly: Orleans.ApplicationPartAttribute("AuthZI.Tests.MicrosoftOrleans.Grains")>]
()

type TestDataPipelineStartup() =
    interface Xunit.v3.ITestPipelineStartup with
        member _.StartAsync(_diagnosticMessageSink: Xunit.Sdk.IMessageSink) =
            let credentialsJson =
                TestDataInitialization.getCredentialsJson "microsoftAzureADB2CCredentials" Literals.azureADB2C1Json

            TestDataInitialization.initializeUsersOnly credentialsJson
            ValueTask()

        member _.StopAsync() = ValueTask()

type Starter() =
    do ()

[<assembly: Xunit.v3.TestPipelineStartup(typeof<TestDataPipelineStartup>)>]
[<assembly: Xunit.AssemblyFixture(typeof<Starter>)>]
()
