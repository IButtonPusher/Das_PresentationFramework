using System;
using System.Collections.Generic;
using System.Linq;
using Das.Views.Core.Writing;

namespace Das.OpenGL.Text;

public abstract class FontProvider : IFontProvider
{
   private readonly IGLContext _context;

   public FontProvider(IGLContext context)
   {
      _context = context;
      _fontBitmapEntries = new List<FontBitmapEntry>();
   }

   private readonly List<FontBitmapEntry> _fontBitmapEntries;
   public IFontRenderer GetRenderer(IFont font)
   {
      var fontHeight = (Int32)(font.Size * (16.0f / 12.0f));

      var result = (from fbe in _fontBitmapEntries
         where fbe.HDC == _context.DeviceContextHandle
               && fbe.HRC == _context.RenderContextHandle
               && String.Compare(fbe.FaceName, font.FamilyName, 
                  StringComparison.OrdinalIgnoreCase) == 0
               && fbe.Height == fontHeight
         select fbe).FirstOrDefault();

      if (result == null)
      {
         result = CreateFontBitmapEntry(font);
         _fontBitmapEntries.Add(result);
      }

      return result;

   }

   protected abstract FontBitmapEntry CreateFontBitmapEntry(IFont font);
        
}