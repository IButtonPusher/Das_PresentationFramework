using System;
using System.Threading.Tasks;
using Das.Views.Text;
using Das.Views.Text.Fonts;
using Das.Views.Text.Fonts.Open;
using Das.Views.Validation;

namespace Das.Views.Layout
{
    public static class Positioning
    {
        public static Int32 DesignToPixels(UInt16 DesignUnitsPerEm,
                                           UInt16 PixelsPerEm,
                                           Int32 Value)
        {
            if (DesignUnitsPerEm == 0)
                return Value;
            var num1 = DesignUnitsPerEm / 2;
            var num2 = Value < 0 ? -(DesignUnitsPerEm >> 1) + 1 : DesignUnitsPerEm / 2;
            return (Value * PixelsPerEm + num2) / DesignUnitsPerEm;
        }

        public static unsafe void AlignAnchors(IOpenTypeFont Font,
                                               FontTable Table,
                                               LayoutMetrics Metrics,
                                               GlyphInfoList GlyphInfo,
                                               Int32* Advances,
                                               LayoutOffset* Offsets,
                                               Int32 StaticGlyph,
                                               Int32 MobileGlyph,
                                               AnchorTable StaticAnchor,
                                               AnchorTable MobileAnchor,
                                               Boolean UseAdvances)
        {
            Invariant.Assert(StaticGlyph >= 0 && StaticGlyph < GlyphInfo.Length);
            Invariant.Assert(MobileGlyph >= 0 && MobileGlyph < GlyphInfo.Length);
            Invariant.Assert(!StaticAnchor.IsNull());
            Invariant.Assert(!MobileAnchor.IsNull());
            var ContourPoint = new LayoutOffset();
            if (StaticAnchor.NeedContourPoint(Table))
                ContourPoint =
                    Font.GetGlyphPointCoord(GlyphInfo.Glyphs[MobileGlyph], StaticAnchor.ContourPointIndex(Table));
            var layoutOffset1 = StaticAnchor.AnchorCoordinates(Table, Metrics, ContourPoint);
            if (MobileAnchor.NeedContourPoint(Table))
                ContourPoint =
                    Font.GetGlyphPointCoord(GlyphInfo.Glyphs[MobileGlyph], MobileAnchor.ContourPointIndex(Table));
            var layoutOffset2 = MobileAnchor.AnchorCoordinates(Table, Metrics, ContourPoint);
            var num1 = 0;
            if (StaticGlyph < MobileGlyph)
            {
                for (var index = StaticGlyph + 1; index < MobileGlyph; ++index)
                    num1 += Advances[index];
            }
            else
            {
                for (var index = MobileGlyph + 1; index < StaticGlyph; ++index)
                    num1 += Advances[index];
            }

            if (Metrics.Direction != TextFlowDirection.LTR && Metrics.Direction != TextFlowDirection.RTL)
                return;
            Offsets[MobileGlyph].dy = Offsets[StaticGlyph].dy + layoutOffset1.dy - layoutOffset2.dy;
            if (Metrics.Direction == TextFlowDirection.LTR == StaticGlyph < MobileGlyph)
            {
                var num2 = Offsets[StaticGlyph].dx - Advances[StaticGlyph] + layoutOffset1.dx - num1 - layoutOffset2.dx;
                if (UseAdvances)
                {
                    var numPtr = Advances + StaticGlyph;
                    *numPtr = *numPtr + num2;
                }
                else
                    Offsets[MobileGlyph].dx = num2;
            }
            else
            {
                var num3 = Offsets[StaticGlyph].dx + Advances[MobileGlyph] + layoutOffset1.dx + num1 - layoutOffset2.dx;
                if (UseAdvances)
                {
                    var numPtr = Advances + MobileGlyph;
                    *numPtr = *numPtr - num3;
                }
                else
                    Offsets[MobileGlyph].dx = num3;
            }
        }
    }
}
