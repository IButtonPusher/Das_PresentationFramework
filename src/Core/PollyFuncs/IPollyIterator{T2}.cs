using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PollyFuncs;

public interface IPollyIterator<in TIn1, in TIn2, out TOut>
{
   IEnumerable<TOut> Execute(TIn1 arg1,
                             TIn2 arg2);
}