using System;

namespace PollyFuncs;

public interface IPollyLateFunc<in TIn, out TResult>
{
   TResult Execute(TIn p);
}