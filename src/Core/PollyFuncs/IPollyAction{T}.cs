using System;
using System.Threading.Tasks;

namespace PollyFuncs;

/// <summary>
///     Idea from: https://github.com/App-vNext/Polly/issues/271
/// </summary>
public interface IPollyAction<in TInput>
{
   void Execute(TInput input);
}