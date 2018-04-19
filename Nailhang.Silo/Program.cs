using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Nailhang.Silo
{
    class Program
    {
        static IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            return builder.Build();
        }

        public static async Task<int> Main(string[] args)
        {
            try
            {
                var host = await StartSilo();

                if (args.Contains("/service", StringComparer.InvariantCultureIgnoreCase))
                {
                    while (true)
                        await Task.Delay(TimeSpan.FromSeconds(10));
                }
                else
                {
                    Console.WriteLine("Press Enter to terminate...");
                    Console.ReadLine();
                }

                await host.StopAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var mongoCS = GetConfig()["MongoConnectionString"];
            var liteDatabase = new LiteDB.LiteDatabase("litedb.db");

            var builder = new SiloHostBuilder()
                .UseMongoDBClustering(options =>
                {
                    options.ConnectionString = mongoCS;
                })
                .UseMongoDBReminders(options =>
                {
                    options.ConnectionString = mongoCS;
                })
                .AddMongoDBGrainStorage("ModulesStoreProvider", options =>
                {
                    options.ConnectionString = mongoCS;
                })
                .Configure<ClusterOptions>(options => options.ClusterId = "cluster_machine_1")
                .ConfigureServices(w => w.AddSingleton(liteDatabase))
                .AddStartupTask(InitWork)
                .UseLocalhostClustering()
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static async Task InitWork(IServiceProvider sp, CancellationToken cancel)
        {            
            var gf = sp.GetRequiredService<IGrainFactory>();
            await Task.CompletedTask;
         }
    }
}
