﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry;

public interface IRoundedRectangle : IPointContainer,
                                     IEquatable<IRoundedRectangle?>
{
   Int32 Height { get; }

   Int32 Width { get; }

   Int32 X { get; }

   Int32 Y { get; }

   ValueIntRectangle GetUnion(IRoundedRectangle other);

   ValueIntRectangle GetUnion(IEnumerable<IRoundedRectangle> others);
}