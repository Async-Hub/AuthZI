namespace AuthZI.Security.AccessToken

open System

type InvalidAccessTokenException=
    inherit Exception
    new () = {}
    new (message) = {inherit Exception(message);}
    new (message : string, innerException) = {inherit Exception(message,innerException);}