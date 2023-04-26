using System;

namespace Das.Views.DependencyProperties;

[Flags]
public enum PropertyMetadata
{
   None = 0,
   AffectsArrange = 1,
   AffectsMeasure = 2,
}