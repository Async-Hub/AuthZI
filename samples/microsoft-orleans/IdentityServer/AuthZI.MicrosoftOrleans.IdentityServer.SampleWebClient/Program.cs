using AuthZI.MicrosoftOrleans.IdentityServer.SampleWebClient;
using IdentityModel.Client;
using System.Net.Http;

Console.Title = "WebClient";

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Console.WriteLine("Please press 's' to start.");
Console.ReadLine();

var identityServerUrl = "https://localhost:7171";
var accessToken = await AccessTokenRetriever.RetrieveToken(identityServerUrl);
Console.WriteLine($"AccessToken: {accessToken}");

var apiUrl = "https://localhost:7116";
var httpClient = new HttpClient() { BaseAddress = new Uri(apiUrl) };
httpClient.SetBearerToken(accessToken);

var response = await httpClient.GetAsync($"/protected-grain");
if (!response.IsSuccessStatusCode)
{
  Console.WriteLine(response.StatusCode);
  Console.WriteLine(response.Content.ToString());
}
else
{
  var content = await response.Content.ReadAsStringAsync();
  Console.WriteLine(content);
}

app.MapGet("/", () => "WebClient is running...");

app.Run();