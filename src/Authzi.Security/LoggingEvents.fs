namespace AuthZI.Security

open Microsoft.Extensions.Logging

// TODO: Try to use best practices for logging events.
module LoggingEvents = 
    let AccessTokenValidationFailed = new EventId(938003, "Access Token validation failed.")

    let OutgoingGrainCallAuthorizationPassed = new EventId(938001, "Outgoing Grain Call Authorization Passed.")

    let AccessTokenVerified = new EventId(938004, "Access Token Verified.")

    let IncomingGrainCallAuthorizationPassed = new EventId(938002, "Incoming Grain Call Authorization Passed.")
