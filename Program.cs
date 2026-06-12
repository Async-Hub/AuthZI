using System;
using System.Reflection;

var assembly = Assembly.LoadFrom("/home/runner/.nuget/packages/duende.identityserver/8.0.1/lib/net10.0/Duende.IdentityServer.dll");
var types = new[] { "Duende.IdentityServer.Services.IProfileService", "Duende.IdentityServer.Test.TestUser", "Duende.IdentityServer.Models.ApiResource", "Duende.IdentityServer.Models.Client" };
foreach(var t in types)
{
    var type = assembly.GetType(t);
    Console.WriteLine($"{t}: {(type != null ? "EXISTS" : "MISSING")}");
}
