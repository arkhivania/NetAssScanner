using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nailhang.Services.Interfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Linq;
using System.Net;
using System.Runtime.Loader;
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

        static bool needed = true;

        public static async Task<int> Main(string[] args)
        {
            try
            {
                var ended = new ManualResetEventSlim();

                AssemblyLoadContext.Default.Unloading += ctx =>
                {
                    System.Console.WriteLine("Unloding fired");
                    needed = false;

                    System.Console.WriteLine("Waiting for completion");
                    ended.Wait();
                };


                var host = await StartSilo();

                if (args.Contains("/service", StringComparer.InvariantCultureIgnoreCase))
                {
                    while (needed)
                        await Task.Delay(TimeSpan.FromSeconds(1));
                }
                else
                {
                    Console.WriteLine("Press Enter to terminate...");
                    Console.ReadLine();
                }

                await host.StopAsync();
                ended.Set();
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
            var config = GetConfig();
            var mongoCS = config["MongoConnectionString"];

            var builder = new SiloHostBuilder()
                .AddMongoDBGrainStorage("GlobalDB", options =>
                {
                    options.ConnectionString = mongoCS;
                })
                .UseMongoDBClustering(c => c.ConnectionString = mongoCS)
                .UseMongoDBReminders(options =>
                {
                    options.ConnectionString = mongoCS;
                })
                .AddMongoDBGrainStorage("MongoDBStore", options =>
                {
                    options.ConnectionString = mongoCS;
                })
                .ConfigureServices(s =>
                {
                    new Nailhang.Services.ModulesMarks.Statistics.Module().Load(s);
                })
                .AddStartupTask(async (s, ct) =>
                {
                    var logger = s.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("StartUp services");
                    foreach (var start in s.GetServices<IStartUp>())
                    {
                        try
                        {
                            await start.StartUp();
                        }catch(Exception e)
                        {
                            logger.LogError("StartUp failed:" + e);
                        }
                    }
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = config["ClusterID"];
                })
                .ConfigureEndpoints(IPAddress.Parse(GetConfig()["IPAddress"]), 11111, 30000)
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
