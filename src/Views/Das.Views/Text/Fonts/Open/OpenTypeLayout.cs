using System;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Text.Fonts.Open
{
    internal static class OpenTypeLayout
    {
        internal static TagInfoFlags FindScript(IOpenTypeFont Font,
                                                UInt32 ScriptTag)
        {
            var script1 = TagInfoFlags.None;
            ScriptTable script2;
            try
            {
                var fontTable = Font.GetFontTable(OpenTypeTags.GSUB);
                if (fontTable.IsPresent)
                {
                    script2 = new GSUBHeader(0).GetScriptList(fontTable).FindScript(fontTable, ScriptTag);
                    if (!script2.IsNull)
                        script1 |= TagInfoFlags.Substitution;
                }
            }
            catch (FormatException ex)
            {
                return TagInfoFlags.None;
            }

            try
            {
                var fontTable = Font.GetFontTable(OpenTypeTags.GPOS);
                if (fontTable.IsPresent)
                {
                    script2 = new GPOSHeader(0).GetScriptList(fontTable).FindScript(fontTable, ScriptTag);
                    if (!script2.IsNull)
                        script1 |= TagInfoFlags.Positioning;
                }
            }
            catch (FormatException ex)
            {
                return TagInfoFlags.None;
            }

            return script1;
        }

        internal static TagInfoFlags FindLangSys(IOpenTypeFont Font,
                                                 UInt32 ScriptTag,
                                                 UInt32 LangSysTag)
        {
            var langSys1 = TagInfoFlags.None;
            LangSysTable langSys2;
            try
            {
                var fontTable = Font.GetFontTable(OpenTypeTags.GSUB);
                if (fontTable.IsPresent)
                {
                    var script = new GSUBHeader(0).GetScriptList(fontTable).FindScript(fontTable, ScriptTag);
                    if (!script.IsNull)
                    {
                        langSys2 = script.FindLangSys(fontTable, LangSysTag);
                        if (!langSys2.IsNull)
                            langSys1 |= TagInfoFlags.Substitution;
                    }
                }
            }
            catch (FormatException ex)
            {
                return TagInfoFlags.None;
            }

            try
            {
                var fontTable = Font.GetFontTable(OpenTypeTags.GPOS);
                if (fontTable.IsPresent)
                {
                    var script = new GPOSHeader(0).GetScriptList(fontTable).FindScript(fontTable, ScriptTag);
                    if (!script.IsNull)
                    {
                        langSys2 = script.FindLangSys(fontTable, LangSysTag);
                        if (!langSys2.IsNull)
                            langSys1 |= TagInfoFlags.Positioning;
                    }
                }
            }
            catch (FormatException ex)
            {
                return TagInfoFlags.None;
            }

            return langSys1;
        }

        internal static unsafe OpenTypeLayoutResult SubstituteGlyphs(IOpenTypeFont Font,
                                                                     OpenTypeLayoutWorkspace workspace,
                                                                     UInt32 ScriptTag,
                                                                     UInt32 LangSysTag,
                                                                     Feature[] FeatureSet,
                                                                     Int32 featureCount,
                                                                     Int32 featureSetOffset,
                                                                     Int32 CharCount,
                                                                     UshortList Charmap,
                                                                     GlyphInfoList Glyphs)
        {
            try
            {
                var fontTable = Font.GetFontTable(OpenTypeTags.GSUB);
                if (!fontTable.IsPresent)
                    return OpenTypeLayoutResult.ScriptNotFound;
                var gsubHeader = new GSUBHeader(0);
                var script = gsubHeader.GetScriptList(fontTable).FindScript(fontTable, ScriptTag);
                if (script.IsNull)
                    return OpenTypeLayoutResult.ScriptNotFound;
                var langSys = script.FindLangSys(fontTable, LangSysTag);
                if (langSys.IsNull)
                    return OpenTypeLayoutResult.LangSysNotFound;
                var featureList = gsubHeader.GetFeatureList(fontTable);
                var lookupList = gsubHeader.GetLookupList(fontTable);
                LayoutEngine.ApplyFeatures(Font, workspace, OpenTypeTags.GSUB, fontTable, new LayoutMetrics(), langSys,
                    featureList, lookupList, FeatureSet, featureCount, featureSetOffset, CharCount, Charmap, Glyphs,
                    null, null);
            }
            catch (FormatException ex)
            {
                return OpenTypeLayoutResult.BadFontTable;
            }

            return OpenTypeLayoutResult.Success;
        }

        internal static unsafe OpenTypeLayoutResult PositionGlyphs(IOpenTypeFont Font,
                                                                   OpenTypeLayoutWorkspace workspace,
                                                                   UInt32 ScriptTag,
                                                                   UInt32 LangSysTag,
                                                                   LayoutMetrics Metrics,
                                                                   Feature[] FeatureSet,
                                                                   Int32 featureCount,
                                                                   Int32 featureSetOffset,
                                                                   Int32 CharCount,
                                                                   UshortList Charmap,
                                                                   GlyphInfoList Glyphs,
                                                                   Int32* Advances,
                                                                   LayoutOffset* Offsets)
        {
            try
            {
                var fontTable = Font.GetFontTable(OpenTypeTags.GPOS);
                if (!fontTable.IsPresent)
                    return OpenTypeLayoutResult.ScriptNotFound;
                var gposHeader = new GPOSHeader(0);
                var script = gposHeader.GetScriptList(fontTable).FindScript(fontTable, ScriptTag);
                if (script.IsNull)
                    return OpenTypeLayoutResult.ScriptNotFound;
                var langSys = script.FindLangSys(fontTable, LangSysTag);
                if (langSys.IsNull)
                    return OpenTypeLayoutResult.LangSysNotFound;
                var featureList = gposHeader.GetFeatureList(fontTable);
                var lookupList = gposHeader.GetLookupList(fontTable);
                LayoutEngine.ApplyFeatures(Font, workspace, OpenTypeTags.GPOS, fontTable, Metrics, langSys, featureList,
                    lookupList, FeatureSet, featureCount, featureSetOffset, CharCount, Charmap, Glyphs, Advances,
                    Offsets);
            }
            catch (FormatException ex)
            {
                return OpenTypeLayoutResult.BadFontTable;
            }

            return OpenTypeLayoutResult.Success;
        }

        internal static OpenTypeLayoutResult CreateLayoutCache(IOpenTypeFont font,
                                                               Int32 maxCacheSize)
        {
            OpenTypeLayoutCache.CreateCache(font, maxCacheSize);
            return OpenTypeLayoutResult.Success;
        }

        internal static OpenTypeLayoutResult GetComplexLanguageList(IOpenTypeFont Font,
                                                                    UInt32[] featureList,
                                                                    UInt32[] glyphBits,
                                                                    UInt16 minGlyphId,
                                                                    UInt16 maxGlyphId,
                                                                    out WritingSystem[] complexLanguages)
        {
            try
            {
                WritingSystem[] complexLanguages1 = null;
                WritingSystem[] complexLanguages2 = null;
                var complexLanguageCount1 = 0;
                var complexLanguageCount2 = 0;
                var fontTable1 = Font.GetFontTable(OpenTypeTags.GSUB);
                var fontTable2 = Font.GetFontTable(OpenTypeTags.GPOS);
                if (fontTable1.IsPresent)
                    LayoutEngine.GetComplexLanguageList(OpenTypeTags.GSUB, fontTable1, featureList, glyphBits,
                        minGlyphId, maxGlyphId, out complexLanguages1, out complexLanguageCount1);
                if (fontTable2.IsPresent)
                    LayoutEngine.GetComplexLanguageList(OpenTypeTags.GPOS, fontTable2, featureList, glyphBits,
                        minGlyphId, maxGlyphId, out complexLanguages2, out complexLanguageCount2);
                if (complexLanguages1 == null && complexLanguages2 == null)
                {
                    complexLanguages = null;
                    return OpenTypeLayoutResult.Success;
                }

                var index1 = 0;
                for (var index2 = 0; index2 < complexLanguageCount2; ++index2)
                {
                    var flag = false;
                    for (var index3 = 0; index3 < complexLanguageCount1; ++index3)
                    {
                        if ((Int32)complexLanguages1[index3].scriptTag == (Int32)complexLanguages2[index2].scriptTag &&
                            (Int32)complexLanguages1[index3].langSysTag == (Int32)complexLanguages2[index2].langSysTag)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        if (index1 < index2)
                            complexLanguages2[index1] = complexLanguages2[index2];
                        ++index1;
                    }
                }

                complexLanguages = new WritingSystem[complexLanguageCount1 + index1];
                for (var index4 = 0; index4 < complexLanguageCount1; ++index4)
                    complexLanguages[index4] = complexLanguages1[index4];
                for (var index5 = 0; index5 < index1; ++index5)
                    complexLanguages[complexLanguageCount1 + index5] = complexLanguages2[index5];
                return OpenTypeLayoutResult.Success;
            }
            catch (FormatException ex)
            {
                complexLanguages = null;
                return OpenTypeLayoutResult.BadFontTable;
            }
        }
    }
}
