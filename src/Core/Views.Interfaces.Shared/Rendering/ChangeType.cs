using System;
using System.Threading.Tasks;

namespace Das.Views.Rendering;

[Flags]
public enum ChangeType
{
   None = 1,
   Measure = 2,
   Arrange = 4,
   MeasureAndArrange = 6
}