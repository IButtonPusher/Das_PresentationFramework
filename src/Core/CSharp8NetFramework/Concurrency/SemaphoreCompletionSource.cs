using System.Threading.Tasks;

namespace System.Threading;

public class SemaphoreCompletionSource<T> : AsyncTaskCompletionSource<Boolean>
{
   public T Data { get; }

   public SemaphoreCompletionSource(T data)
   {
      Data = data;
   }
   
}
