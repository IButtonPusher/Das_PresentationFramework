using System;
using System.Threading.Tasks;
using Das.Views.Text.Fonts;

namespace Das.Views.Core.Writing;

public interface IFont : IEquatable<IFont>
{
   IFont Resize(Double newSize);

   IFontFace GetFontFace();

   Boolean HasCharacter(UInt32 unicodeValue);

   String FamilyName { get; }

   FontStyle FontStyle { get; }

   Double Size { get; }

   FontMetrics Metrics { get; }
}