namespace AuthZI.Security

open Microsoft.Extensions.Logging

// TODO: Try to use best practices for logging events.
module LoggingEvents = 
    let AccessTokenValidationFailed = new EventId(938000, "Access token validation failed.")

    let AccessTokenVerified = new EventId(938001, "Access token validation passed.")

    let OutgoingGrainCallAuthorizationPassed = new EventId(938002, "Outgoing grain call authorization passed.")

    let OutgoingGrainCallAuthorizationFailed = new EventId(938003, "Outgoing grain call authorization failed.")

    let IncomingGrainCallAuthorizationPassed = new EventId(938004, "Incoming grain call authorization passed.")

    let IncomingGrainCallAuthorizationFailed= new EventId(938005, "Incoming grain call authorization failed.")
