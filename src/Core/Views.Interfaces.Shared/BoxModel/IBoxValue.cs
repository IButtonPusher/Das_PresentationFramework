using System;

namespace Das.Views.Core;

public interface IBoxValue<out T>
{
   T Left { get; }

   T Right { get; }

   T Top { get; }

   T Bottom { get; }
}