using System;
using System.Threading.Tasks;

namespace PollyFuncs;

public interface IPollyPromise<TResult>
{
   Task<TResult> ExecuteAsync();

   Boolean TryCancel();
}