using System;
using System.Threading.Tasks;

namespace Das.ViewModels
{
   public interface IAsyncQueryable<T>
   {
      Task<T> FirstOrDefaultAsync(Func<T, Boolean> predicate);

      Task<T> FirstOrDefaultAsync();
   }
}
