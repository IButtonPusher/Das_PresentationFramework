using System;

namespace Das.Views.Core.Geometry;

public interface IPoint2F : IEquatable<IPoint2F>
{
   Single X {get;}

   Single Y { get; }
}