using System;
using System.Threading.Tasks;

namespace Das.Views.Styles;

[Flags]
public enum VisualStateType
{
   Invalid = 0,
   None = 1,
   Hover = 2,
   Active = 4,
   Checked = 8,
   Disabled = 16,
   Focus = 32
}