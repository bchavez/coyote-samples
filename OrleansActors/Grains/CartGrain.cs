using System.Threading.Tasks;
using Actors;
using Core;
using Microsoft.Coyote.Actors;
using Microsoft.Extensions.Logging;

namespace Grains
{
   public class CartGrain : Orleans.Grain, ICart
   {
      private readonly ILogger logger;
      private readonly IActorRuntime actorRuntime;
      private ActorId actorId;

      public CartGrain(ILogger<CartGrain> logger, IActorRuntime actorRuntime)
      {
         this.logger = logger;
         this.actorRuntime = actorRuntime;
         this.actorId = actorRuntime.CreateActor(typeof(CartActor));
      }

      Task<string> ICart.Add(string sku, decimal price)
      {
         this.logger.LogInformation($"ADD SKU: '{sku}' {price:c}");

         var e = new AddItemEvent
            {
               Sku = sku,
               Price = price
            };

         // Note: The operation will place the event in the Coyote inbox
         // for the actor and immediately return after the event is placed
         // in the inbox for processing.
         //
         // The preferred semantic operation is to await for the event
         // to be processed by Coyote. If we return "right now" after
         // the SendEvent, the Coyote actor will "lag" behind
         // the Orleans grain. This lag might be problematic because
         // we'd want to know if an exception occured during this "Add"
         // operation during the processing of this AddItemEvent.
         this.actorRuntime.SendEvent(this.actorId, e);

         return Task.FromResult($"Added '{sku}' for '{price:c}'");
      }

      Task<string> ICart.Remove(string sku)
      {
         this.logger.LogInformation($"REMOVE SKU: '{sku}'");

         var e = new RemoveItemEvent
            {
               Sku = sku
            };

         this.actorRuntime.SendEvent(this.actorId, e);

         return Task.FromResult($"Removed '{sku}'");
      }

      async Task<decimal> ICart.GetTotal()
      {
         this.logger.LogInformation($"GET TOTAL");

         //Seems three ways to get data out of Coyote?
         //1. Use AwaitableEventGroup<T>
         //2. Use GetTotalRequestEvent reference and use a property for response
         //3. SharedRegister.


         // Note: Feels like a hack to use the CurrentEventGroup as a mechanism for
         // returning a simple value from Coyote.
         var response = new AwaitableEventGroup<decimal>();
         var e = new GetTotalRequestEvent();

         this.actorRuntime.SendEvent(this.actorId, e, response);

         var total = await response;
         this.logger.LogInformation($"TOTAL FOR '{this.IdentityString}' is {total:c}");

         return total;
      }
   }
}