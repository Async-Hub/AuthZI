namespace Authzi.Security.Authorization

open System.Reflection
open System.Linq

type internal AuthorizationAdmission() =
    static member IsRequired() =
        true