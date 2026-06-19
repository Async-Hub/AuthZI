using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra;

public static class SiloHostBuilder
{
  public static IHost Build(Action<IServiceCollection> configureDelegate)
  {
    var builder = new HostBuilder()
      .UseEnvironment(Environments.Development)
      .UseOrleans((_, siloBuilder) =>
      {
        siloBuilder
          .UseLocalhostClustering()
          .Configure<ClusterOptions>(options =>
          {
            options.ClusterId = "Orleans.Security.Test";
            options.ServiceId = "ServiceId";
          })
          .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
          .AddMemoryGrainStorage("MemoryGrainStorage")
          .ConfigureServices(configureDelegate);
      });

    return builder.Build();
  }
}
