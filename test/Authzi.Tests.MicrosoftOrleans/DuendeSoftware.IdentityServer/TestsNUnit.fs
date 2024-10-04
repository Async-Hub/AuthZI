module TestsNUnit

open NUnit.Framework
open Authzi.Security.AccessToken

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Test () =
    Assert.Pass()
