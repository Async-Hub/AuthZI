namespace AuthZI.Security

open System

[<Serializable>]
type AuthorizationException =
    inherit Exception

    new() = {inherit Exception();}
    new(message) = { inherit Exception(message); }
    new(message, innerException : Exception) = {inherit Exception(message, innerException);}