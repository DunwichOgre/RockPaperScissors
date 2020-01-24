using System.Net;
using System.Threading.Tasks;
using RockPaperScissors.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace OrleansSiloHost
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "RockPaperScissorsApp";
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(MatchGrain).Assembly).WithReferences())
                        .AddMemoryGrainStorage(name: "ArchiveStorage")
                        .AddMemoryGrainStorage(name: "profileStore")
                        // .AddAzureBlobGrainStorage(
                        //     name: "profileStore",
                        //     configureOptions: options =>
                        //     {
                        //         // Use JSON for serializing the state in storage
                        //         options.UseJson = true;

                        //         // Configure the storage connection key
                        //         options.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=data1;AccountKey=SOMETHING1";
                        //     })
                        ;
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .RunConsoleAsync();
        }
    }
}
