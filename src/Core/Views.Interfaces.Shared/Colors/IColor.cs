using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Drawing;

public interface IColor
{
   Byte A { get; }

   Byte B { get; }

   Byte G { get; }

   Byte R { get; }

   IBrush ToBrush();
}