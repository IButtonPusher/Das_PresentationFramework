using System;
using System.Collections.Generic;

namespace PollyFuncs;

public interface IPollyIterator<in TIn, out TOut>
{
   IEnumerable<TOut> Execute(TIn arg);
}