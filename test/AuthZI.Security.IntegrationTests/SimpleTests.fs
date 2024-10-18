module SimpleTests

open System
open FluentAssertions
open Xunit

[<Theory>]
[<InlineData("Bob")>]
let ``This is a simple xUnit.net test``
    (userName: string) =
        let actual = Environment.GetEnvironmentVariable("microsoftEntraIdCredentials")
        Assert.Equal(userName, "Bob")
        //Assert.Equal(userName, actual)