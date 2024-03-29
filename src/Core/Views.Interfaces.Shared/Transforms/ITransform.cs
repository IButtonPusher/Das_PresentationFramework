﻿using System;

namespace Das.Views.Transforms;

public interface ITransform
{
   Boolean IsIdentity {get;}

   TransformationMatrix Value { get; }
}