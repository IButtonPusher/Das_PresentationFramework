using System;
using System.Collections.Generic;

namespace Das.Views.BoxModel;

public interface IBoxShadow : IEnumerable<IBoxShadowLayer>
{
   Boolean IsEmpty { get; }
}