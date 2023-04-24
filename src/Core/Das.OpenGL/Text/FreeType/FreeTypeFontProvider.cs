using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Das.Views.Core.Writing;

namespace Das.OpenGL.Text.FreeType;

public class FreeTypeFontProvider : IFontProvider<GLFont>, IFontProvider
{
   private readonly IGLContext _context;

   public FreeTypeFontProvider(DirectoryInfo fontDirectory, IGLContext context)
   {
      _context = context;
      _cached = new Dictionary<IFont, GLFont>();
      _fontFiles = new Dictionary<String, Dictionary<FontStyle, FileInfo>>(
         StringComparer.OrdinalIgnoreCase);

      var ret = FT.FT_Init_FreeType(out var library);
      if (ret != FtErrors.Ok)
      {
         throw new Exception("FreeType library Exception: " + ret);
      }

      foreach (var fi in fontDirectory.GetFiles("*.ttf"))
      {
         ret = FT.FT_New_Face(library, fi.FullName, 0, out var facePtr);
         if (ret != FtErrors.Ok)
            continue;

         var ftFont = (FreeTypeFont)Marshal.PtrToStructure(facePtr, typeof(FreeTypeFont));
         var strName = Marshal.PtrToStringAnsi(ftFont.family_name);
         var styleName = Marshal.PtrToStringAnsi(ftFont.style_name) ?? String.Empty;
         var fontStyle = GetFontStyle(styleName);
         if (fontStyle == FontStyle.Unusable)
            continue; 

         if (String.IsNullOrWhiteSpace(strName))
            continue;

         if (!_fontFiles.TryGetValue(strName, out var styleDic))
         {
            styleDic = new Dictionary<FontStyle, FileInfo>();
            _fontFiles[strName] = styleDic;
         }

         styleDic[fontStyle] = fi;
      }
   }

   private static FontStyle GetFontStyle(String name)
   {
      switch (name)
      {
         case "":
         case "Regular":
         case "Normal":
            return FontStyle.Regular;
         case "Bold":
            return FontStyle.Bold;
         case "Black":
            return FontStyle.Black;
         case "Italic":
            return FontStyle.Italic;
         case "Bold Italic":
         case "Black Italic":
            return FontStyle.BoldItalic;
         case "Narrow":
            return FontStyle.Narrow;
         case "Semibold":
            return FontStyle.SemiBold;
         default:
            Debug.WriteLine("WARNING: Unknown font style: " + name);
            return FontStyle.Unusable;
      }
   }

   public GLFont GetRenderer(IFont font)
   {
      if (_cached.TryGetValue(font, out var found))
         return found;

      if (!_fontFiles.TryGetValue(font.FamilyName, out var styleDic))
         throw new FileNotFoundException($"Font {font} not found");

      if (!styleDic.TryGetValue(font.FontStyle, out var file))
         throw new FileNotFoundException($"Font {font} not found");

      var sz = (UInt32)Math.Round(font.Size /* * 1.5 */, 0);

      var glFont = new GLFont(font, file.FullName, sz, _context);
      _cached[font] = glFont;
      return glFont;

   }

   private readonly Dictionary<IFont, GLFont> _cached;
   private readonly Dictionary<String, Dictionary<FontStyle, FileInfo>> _fontFiles;

   IFontRenderer IFontProvider.GetRenderer(IFont font)=> GetRenderer(font);
}