using System;
using System.Threading.Tasks;

namespace PollyFuncs;

public interface IPollyFunc<out TResult>
{
   TResult Execute();
}