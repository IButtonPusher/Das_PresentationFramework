using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Writing;

[Flags]
public enum FontStyle
{
   Unusable = -1,

   //
   // Summary:
   //     Normal text.
   Regular = 0,

   //
   // Summary:
   //     Bold text.
   Bold = 1,

   //
   // Summary:
   //     Italic text.
   Italic = 2,

   BoldItalic = 3,

   //
   // Summary:
   //     Underlined text.
   Underline = 4,

   //
   // Summary:
   //     Text with a line through the middle.
   Strikeout = 8,

   Narrow = 16,

   SemiBold = 32,

   Black = 64
}