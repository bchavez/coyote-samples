using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
   public interface ICart : Orleans.IGrainWithIntegerKey
   {
      Task<string> Add(string sku, decimal price);
      Task<string> Remove(string sku);
      Task<decimal> GetTotal();
   }
}