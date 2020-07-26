using Microsoft.Coyote;
using Microsoft.Coyote.Actors;

namespace Actors
{
   public class AddItemEvent : Event
   {
      public string Sku { get; set; }
      public decimal Price { get; set; }
   }

   public class RemoveItemEvent : Event
   {
      public string Sku { get; set; }
   }

   public class GetTotalRequestEvent : Event
   {
      public GetTotalRequestEvent()
      {
      }
   }

   public class GetTotalResponseEvent : Event
   {
      public decimal Total { get; set; }
   }
}