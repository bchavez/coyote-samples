using System;
using System.Threading.Tasks;
using Grains;
using Microsoft.Coyote.Actors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using static System.Console;

namespace Silo
{
   public class Program
   {
      public static async Task<int> Main(string[] args)
      {
         try
         {
            var host = await StartSilo();
            WriteLine("Press Enter to terminate...");
            ReadLine();

            await host.StopAsync();

            return 0;
         }
         catch (Exception ex)
         {
            WriteLine(ex);
            return 1;
         }
      }

      private static async Task<ISiloHost> StartSilo()
      {
         // define the cluster configuration
         var builder = new SiloHostBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
               {
                  options.ClusterId = "dev";
                  options.ServiceId = "OrleansBasics";
               })
            .ConfigureApplicationParts(parts =>
               {
                  parts.AddApplicationPart(typeof(CartGrain).Assembly).WithReferences();
               })
            .ConfigureLogging(logging => logging.AddConsole())
            .ConfigureServices(config =>
               {
                  config.AddSingleton(_ => RuntimeFactory.Create());
               });


         var host = builder.Build();
         await host.StartAsync();
         return host;
      }
   }
}
