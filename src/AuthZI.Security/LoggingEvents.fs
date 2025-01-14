namespace AuthZI.Security

open Microsoft.Extensions.Logging

// TODO: Try to use best practices for logging events.
module LoggingEvents = 
    let AccessTokenValidationFailed = EventId(938000, "Access token validation failed.")

    let AccessTokenVerified = EventId(938001, "Access token validation passed.")

    let OutgoingGrainCallAuthorizationPassed = EventId(938002, "Outgoing grain call authorization passed.")

    let OutgoingGrainCallAuthorizationFailed = EventId(938003, "Outgoing grain call authorization failed.")

    let IncomingGrainCallAuthorizationPassed = EventId(938004, "Incoming grain call authorization passed.")

    let IncomingGrainCallAuthorizationFailed= EventId(938005, "Incoming grain call authorization failed.")