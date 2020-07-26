using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Coyote;
using Microsoft.Coyote.Actors;
using static System.Console;

namespace Actors
{
   [OnEventDoAction(typeof(AddItemEvent), nameof(OnAddItemEvent))]
   [OnEventDoAction(typeof(RemoveItemEvent), nameof(OnRemoveItemEvent))]
   [OnEventDoAction(typeof(GetTotalRequestEvent), nameof(OnGetTotal))]
   public class CartActor : Actor
   {
      private Dictionary<string, decimal> items = new Dictionary<string, decimal>();


      protected void OnAddItemEvent(Event e)
      {
         // Note: Not a fan of this parameter "Event e" and having to cast "e"
         // the appropriate event type.
         // I think "OnAddItemEvent(AddItemEvent e)" would be better?
         if ( !(e is AddItemEvent item) ) return;

         if( this.items.ContainsKey(item.Sku) )
         {
            this.Assert(true, "The item cannot be added again.");
         }

         this.items.Add(item.Sku, item.Price);
      }

      protected void OnRemoveItemEvent(Event e)
      {
         if( !(e is RemoveItemEvent item) ) return;

         if( this.items.TryGetValue(item.Sku, out var price) )
         {
            this.items.Remove(item.Sku);
         }
         else
         {
            this.Assert(true, "Cannot remove an item from the cart that never existed.");
         }
      }


      //protected void OnGetTotal(GetTotalRequestEvent e) -- doesn't work. :(
      protected void OnGetTotal(Event e)
      {
         if( !(e is GetTotalRequestEvent) ) return;

         var total = this.items.Values.Sum();

         //var response = new GetTotalResponseEvent()
         //   {
         //      Total = total
         //   };
         //this.SendEvent(caller, response);

         // Note: Feels like a hack to use the CurrentEventGroup as a mechanism for
         // returning a simple value from Coyote.
         if( this.CurrentEventGroup is AwaitableEventGroup<decimal> awaiter )
         {
            awaiter.TrySetResult(total);
         }
         else
         {
            this.Assert(true, $"CurrentEventGroup should be set to '{nameof(AwaitableEventGroup<decimal>)}'");
         }
      }
   }
}