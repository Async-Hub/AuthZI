using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthZI.Tests.Duende.IdentityServer.Configuration;

public static class TestIdentityServerBuilder
{
  public static TestServer StartNew()
  {
    var builder = new HostBuilder()
      .ConfigureWebHost(webHost =>
      {
        webHost
          .UseUrls("http://localhost:5001")
          .UseTestServer()
          .ConfigureServices(services =>
          {
            services.AddIdentityServer()
              .AddDeveloperSigningCredential()
              .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
              .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
              .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
              .AddInMemoryClients(IdentityServerConfig.GetClients())
              .AddTestUsers(IdentityServerConfig.GetUsers());

            services.AddControllers();
          })
          .Configure(app =>
          {
            app.UseIdentityServer();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
              endpoints.MapDefaultControllerRoute();
            });
          });
      });

    var host = builder.Start();

    var identityServer4 = host.GetTestServer();
    identityServer4.CreateHandler();
    identityServer4.BaseAddress = new Uri("http://localhost:5001");

    return identityServer4;
  }
}