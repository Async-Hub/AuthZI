using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra;

public static class SiloClientBuilder
{
  public static IHost Build(Action<IServiceCollection> configureDelegate)
  {
    var hostBuilder = new HostBuilder()
      .UseOrleansClient(clientBuilder =>
      {
        clientBuilder
          .UseLocalhostClustering()
          .Configure<ClusterOptions>(options =>
          {
            options.ClusterId = "Orleans.Security.Test";
            options.ServiceId = "ServiceId";
          })
          .ConfigureServices(configureDelegate);
      });

    return hostBuilder.Build();
  }
}
