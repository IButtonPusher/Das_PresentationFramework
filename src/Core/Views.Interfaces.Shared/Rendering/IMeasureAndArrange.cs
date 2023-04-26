using System;

namespace Das.Views.Rendering;

public interface IMeasureAndArrange
{
   void InvalidateMeasure();

   void InvalidateArrange();

   Boolean IsRequiresMeasure { get; }

   Boolean IsRequiresArrange { get; }

   void AcceptChanges(ChangeType changeType);
}