using System;
using System.Threading.Tasks;
using Core;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using static System.Console;

namespace Client
{
   public class Program
   {
      public static async Task<int> Main(string[] args)
      {
         WriteLine("Press any key to connect to Orleans cluster.");
         ReadKey();
         try
         {
            using (var client = await ConnectClient())
            {
               await DoClientWork(client);
               ReadKey();
            }

            return 0;
         }
         catch (Exception e)
         {
            WriteLine($"Exception while trying to run client: {e.Message}");
            WriteLine("Make sure the silo the client is trying to connect to is running.");
            WriteLine("Press any key to exit.");
            ReadKey();
            return 1;
         }
      }

      private static async Task DoClientWork(IClusterClient client)
      {
         // example of calling grains from the initialized client
         var cart = client.GetGrain<ICart>(0);
         var response = await cart.Add("candy_chocolate", 3.99m);
         WriteLine($"Response: {response}");
         response = await cart.Add("candy_gum", 2.99m);
         WriteLine($"Response: {response}");

         var total = await cart.GetTotal();

         WriteLine($"GET TOTAL: {total:c}");
      }

      private static async Task<IClusterClient> ConnectClient()
      {
         var client = new ClientBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
               {
                  options.ClusterId = "dev";
                  options.ServiceId = "OrleansBasics";
               })
            .ConfigureLogging(logging => logging.AddConsole())
            .Build();

         await client.Connect();
         WriteLine("Client successfully connected to silo host.");
         return client;
      }
   }
}
