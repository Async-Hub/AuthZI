module SimpleTests

open System
open FluentAssertions
open Xunit

[<Theory>]
[<InlineData("Bob")>]
let ``This is a simple xUnit.net test``
    (userName: string) =
        Assert.Equal(userName, "Bob")