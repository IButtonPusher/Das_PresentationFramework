using System;
using System.Threading.Tasks;
using Das.Views.Text;
using Das.Views.Text.Fonts;
using Das.Views.Text.Fonts.Open;

namespace Das.Views.Layout
{
    public static class LayoutEngine
    {
        public static unsafe void ApplyFeatures(IOpenTypeFont Font,
                                                OpenTypeLayoutWorkspace workspace,
                                                OpenTypeTags TableTag,
                                                FontTable Table,
                                                LayoutMetrics Metrics,
                                                LangSysTable LangSys,
                                                FeatureList Features,
                                                LookupList Lookups,
                                                Feature[] FeatureSet,
                                                Int32 featureCount,
                                                Int32 featureSetOffset,
                                                Int32 CharCount,
                                                UshortList Charmap,
                                                GlyphInfoList GlyphInfo,
                                                Int32* Advances,
                                                LayoutOffset* Offsets)
        {
            UpdateGlyphFlags(Font, GlyphInfo, 0, GlyphInfo.Length, false, GlyphFlags.Unassigned);
            if (workspace == null)
                workspace = new OpenTypeLayoutWorkspace();
            var lookupCount = Lookups.LookupCount(Table);
            CompileFeatureSet(FeatureSet, featureCount, featureSetOffset, CharCount, Table, LangSys, Features,
                lookupCount, workspace);
            OpenTypeLayoutCache.InitCache(Font, TableTag, GlyphInfo, workspace);
            for (UInt16 lookupIndex = 0; lookupIndex < lookupCount; ++lookupIndex)
            {
                if (workspace.IsAggregatedFlagSet(lookupIndex))
                {
                    var FirstChar = 0;
                    var AfterLastChar = 0;
                    var firstGlyph = 0;
                    var AfterLastGlyph = 0;
                    OpenTypeLayoutCache.FindNextLookup(workspace, GlyphInfo, lookupIndex, out lookupIndex,
                        out firstGlyph);
                    if (lookupIndex >= lookupCount)
                        break;
                    if (workspace.IsAggregatedFlagSet(lookupIndex))
                    {
                        var Lookup = Lookups.Lookup(Table, lookupIndex);
                        UInt32 Parameter = 0;
                        var isLookupReversal = IsLookupReversal(TableTag, Lookup.LookupType());
                        while (firstGlyph < GlyphInfo.Length)
                        {
                            if (!OpenTypeLayoutCache.FindNextGlyphInLookup(workspace, lookupIndex, isLookupReversal,
                                    ref firstGlyph, ref AfterLastGlyph))
                                firstGlyph = AfterLastGlyph;
                            if (firstGlyph < AfterLastGlyph)
                            {
                                var length = GlyphInfo.Length;
                                var num = length - AfterLastGlyph;
                                Int32 NextGlyph;
                                if (ApplyLookup(Font, TableTag, Table, Metrics, Lookup, CharCount, Charmap, GlyphInfo,
                                        Advances, Offsets, firstGlyph, AfterLastGlyph, Parameter, 0, out NextGlyph))
                                {
                                    if (!isLookupReversal)
                                    {
                                        OpenTypeLayoutCache.OnGlyphsChanged(workspace, GlyphInfo, length, firstGlyph,
                                            NextGlyph);
                                        AfterLastGlyph = GlyphInfo.Length - num;
                                        firstGlyph = NextGlyph;
                                    }
                                    else
                                    {
                                        OpenTypeLayoutCache.OnGlyphsChanged(workspace, GlyphInfo, length, NextGlyph,
                                            AfterLastGlyph);
                                        AfterLastGlyph = NextGlyph;
                                    }
                                }
                                else if (isLookupReversal)
                                    AfterLastGlyph = NextGlyph;
                                else
                                    firstGlyph = NextGlyph;
                            }
                            else
                                GetNextEnabledGlyphRange(FeatureSet, featureCount, featureSetOffset, Table, workspace,
                                    LangSys, Features, lookupIndex, CharCount, Charmap, AfterLastChar, AfterLastGlyph,
                                    GlyphInfo.Length, out FirstChar, out AfterLastChar, out firstGlyph,
                                    out AfterLastGlyph, out Parameter);
                        }
                    }
                }
            }
        }

        public static unsafe Boolean ApplyLookup(IOpenTypeFont Font,
                                                 OpenTypeTags TableTag,
                                                 FontTable Table,
                                                 LayoutMetrics Metrics,
                                                 LookupTable Lookup,
                                                 Int32 CharCount,
                                                 UshortList Charmap,
                                                 GlyphInfoList GlyphInfo,
                                                 Int32* Advances,
                                                 LayoutOffset* Offsets,
                                                 Int32 FirstGlyph,
                                                 Int32 AfterLastGlyph,
                                                 UInt32 Parameter,
                                                 Int32 nestingLevel,
                                                 out Int32 NextGlyph)
        {
            var LookupType1 = Lookup.LookupType();
            var LookupFlags = Lookup.LookupFlags();
            var num1 = Lookup.SubTableCount();
            var flag = false;
            NextGlyph = FirstGlyph + 1;
            if (!IsLookupReversal(TableTag, LookupType1))
                FirstGlyph = GetNextGlyphInLookup(Font, GlyphInfo, FirstGlyph, LookupFlags, 1);
            else
                AfterLastGlyph = GetNextGlyphInLookup(Font, GlyphInfo, AfterLastGlyph - 1, LookupFlags, -1) + 1;
            if (FirstGlyph >= AfterLastGlyph)
                return flag;
            var num2 = LookupType1;
            for (UInt16 Index = 0; !flag && Index < num1; ++Index)
            {
                var LookupType2 = num2;
                var Offset = Lookup.SubtableOffset(Table, Index);
                switch (TableTag)
                {
                    case OpenTypeTags.GPOS:
                        if (LookupType2 == 9)
                        {
                            var extensionLookupTable = new ExtensionLookupTable(Offset);
                            LookupType2 = extensionLookupTable.LookupType(Table);
                            Offset = extensionLookupTable.LookupSubtableOffset(Table);
                        }

                        switch (LookupType2 - 1)
                        {
                            case 0:
                                flag = new SinglePositioningSubtable(Offset).Apply(Table, Metrics, GlyphInfo, Advances,
                                    Offsets, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 1:
                                flag = new PairPositioningSubtable(Offset).Apply(Font, Table, Metrics, GlyphInfo,
                                    LookupFlags, Advances, Offsets, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 2:
                                new CursivePositioningSubtable(Offset).Apply(Font, Table, Metrics, GlyphInfo,
                                    LookupFlags, Advances, Offsets, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 3:
                                flag = new MarkToBasePositioningSubtable(Offset).Apply(Font, Table, Metrics, GlyphInfo,
                                    LookupFlags, Advances, Offsets, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 4:
                                flag = new MarkToLigaturePositioningSubtable(Offset).Apply(Font, Table, Metrics,
                                    GlyphInfo, LookupFlags, CharCount, Charmap, Advances, Offsets, FirstGlyph,
                                    AfterLastGlyph, out NextGlyph);
                                break;
                            case 5:
                                flag = new MarkToMarkPositioningSubtable(Offset).Apply(Font, Table, Metrics, GlyphInfo,
                                    LookupFlags, Advances, Offsets, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 6:
                                flag = new ContextSubtable(Offset).Apply(Font, TableTag, Table, Metrics, CharCount,
                                    Charmap, GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph,
                                    Parameter, nestingLevel, out NextGlyph);
                                break;
                            case 7:
                                flag = new ChainingSubtable(Offset).Apply(Font, TableTag, Table, Metrics, CharCount,
                                    Charmap, GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph,
                                    Parameter, nestingLevel, out NextGlyph);
                                break;
                            case 8:
                                NextGlyph = FirstGlyph + 1;
                                break;
                            default:
                                NextGlyph = FirstGlyph + 1;
                                break;
                        }

                        if (flag)
                        {
                            UpdateGlyphFlags(Font, GlyphInfo, FirstGlyph, NextGlyph, false, GlyphFlags.Positioned);
                        }

                        break;
                    case OpenTypeTags.GSUB:
                        if (LookupType2 == 7)
                        {
                            var extensionLookupTable = new ExtensionLookupTable(Offset);
                            LookupType2 = extensionLookupTable.LookupType(Table);
                            Offset = extensionLookupTable.LookupSubtableOffset(Table);
                        }

                        switch (LookupType2 - 1)
                        {
                            case 0:
                                flag = new SingleSubstitutionSubtable(Offset).Apply(Table, GlyphInfo, FirstGlyph,
                                    out NextGlyph);
                                break;
                            case 1:
                                flag = new MultipleSubstitutionSubtable(Offset).Apply(Font, Table, CharCount, Charmap,
                                    GlyphInfo, LookupFlags, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 2:
                                flag = new AlternateSubstitutionSubtable(Offset).Apply(Table, GlyphInfo, Parameter,
                                    FirstGlyph, out NextGlyph);
                                break;
                            case 3:
                                flag = new LigatureSubstitutionSubtable(Offset).Apply(Font, Table, CharCount, Charmap,
                                    GlyphInfo, LookupFlags, FirstGlyph, AfterLastGlyph, out NextGlyph);
                                break;
                            case 4:
                                flag = new ContextSubtable(Offset).Apply(Font, TableTag, Table, Metrics, CharCount,
                                    Charmap, GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph,
                                    Parameter, nestingLevel, out NextGlyph);
                                break;
                            case 5:
                                flag = new ChainingSubtable(Offset).Apply(Font, TableTag, Table, Metrics, CharCount,
                                    Charmap, GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph,
                                    Parameter, nestingLevel, out NextGlyph);
                                break;
                            case 6:
                                NextGlyph = FirstGlyph + 1;
                                break;
                            case 7:
                                flag = new ReverseChainingSubtable(Offset).Apply(Font, TableTag, Table, Metrics,
                                    CharCount, Charmap, GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph,
                                    AfterLastGlyph, Parameter, out NextGlyph);
                                break;
                            default:
                                NextGlyph = FirstGlyph + 1;
                                break;
                        }

                        if (flag)
                        {
                            if (!IsLookupReversal(TableTag, LookupType2))
                            {
                                UpdateGlyphFlags(Font, GlyphInfo, FirstGlyph, NextGlyph, true, GlyphFlags.Substituted);
                                break;
                            }

                            UpdateGlyphFlags(Font, GlyphInfo, NextGlyph, AfterLastGlyph, true, GlyphFlags.Substituted);
                        }

                        break;
                }
            }

            return flag;
        }

        public static Int32 GetNextGlyphInLookup(IOpenTypeFont Font,
                                                 GlyphInfoList GlyphInfo,
                                                 Int32 FirstGlyph,
                                                 UInt16 LookupFlags,
                                                 Int32 Direction)
        {
            FontTable Table = null;
            var classDefTable = ClassDefTable.InvalidClassDef;
            if (LookupFlags == 0)
                return FirstGlyph;
            if ((LookupFlags & 65280) != 0)
            {
                Table = Font.GetFontTable(OpenTypeTags.GDEF);
                if (Table.IsPresent)
                    classDefTable = new GDEFHeader(0).GetMarkAttachClassDef(Table);
            }

            var glyphFlags = GlyphInfo.GlyphFlags;
            var num = (UInt16)((LookupFlags & 65280) >> 8);
            var length = GlyphInfo.Length;
            var index = FirstGlyph;
            while (index < length && index >= 0 && ((LookupFlags & 2) != 0 && (glyphFlags[index] & 7) == 1 ||
                                                    (LookupFlags & 8) != 0 && (glyphFlags[index] & 7) == 3 ||
                                                    (LookupFlags & 4) != 0 && (glyphFlags[index] & 7) == 2 ||
                                                    num != 0 && (glyphFlags[index] & 7) == 3 &&
                                                    !classDefTable.IsInvalid &&
                                                    num != classDefTable.GetClass(Table,
                                                        GlyphInfo.Glyphs[index])))
                index += Direction;
            return index;
        }

        public static void GetComplexLanguageList(OpenTypeTags tableTag,
                                                  FontTable table,
                                                  UInt32[] featureTagsList,
                                                  UInt32[] glyphBits,
                                                  UInt16 minGlyphId,
                                                  UInt16 maxGlyphId,
                                                  out WritingSystem[] complexLanguages,
                                                  out Int32 complexLanguageCount)
        {
            var scriptList = new ScriptList(0);
            var featureList = new FeatureList(0);
            var lookupList = new LookupList(0);
            switch (tableTag)
            {
                case OpenTypeTags.GPOS:
                    var gposHeader = new GPOSHeader(0);
                    scriptList = gposHeader.GetScriptList(table);
                    featureList = gposHeader.GetFeatureList(table);
                    lookupList = gposHeader.GetLookupList(table);
                    break;
                case OpenTypeTags.GSUB:
                    var gsubHeader = new GSUBHeader(0);
                    scriptList = gsubHeader.GetScriptList(table);
                    featureList = gsubHeader.GetFeatureList(table);
                    lookupList = gsubHeader.GetLookupList(table);
                    break;
            }

            var scriptCount = (Int32)scriptList.GetScriptCount(table);
            var num1 = (Int32)featureList.FeatureCount(table);
            var num2 = (Int32)lookupList.LookupCount(table);
            var lookupBits = new UInt32[(num2 + 31) >> 5];
            for (var index = 0; index < (num2 + 31) >> 5; ++index)
                lookupBits[index] = 0U;
            for (UInt16 Index1 = 0; Index1 < num1; ++Index1)
            {
                var num3 = featureList.FeatureTag(table, Index1);
                var flag = false;
                for (var index = 0; index < featureTagsList.Length; ++index)
                {
                    if ((Int32)featureTagsList[index] == (Int32)num3)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    var featureTable = featureList.FeatureTable(table, Index1);
                    var num4 = featureTable.LookupCount(table);
                    for (UInt16 Index2 = 0; Index2 < num4; ++Index2)
                    {
                        var num5 = featureTable.LookupIndex(table, Index2);
                        if (num5 >= num2)
                            throw new FormatException();
                        lookupBits[num5 >> 5] |= (UInt32)(1 << (num5 % 32));
                    }
                }
            }

            for (UInt16 Index3 = 0; Index3 < num2; ++Index3)
            {
                if ((lookupBits[Index3 >> 5] & (1 << (Index3 % 32))) != 0L)
                {
                    var lookupTable = lookupList.Lookup(table, Index3);
                    var num6 = lookupTable.LookupType();
                    var num7 = lookupTable.SubTableCount();
                    var flag = false;
                    var num8 = num6;
                    for (UInt16 Index4 = 0; !flag && Index4 < num7; ++Index4)
                    {
                        var num9 = num8;
                        var Offset = lookupTable.SubtableOffset(table, Index4);
                        switch (tableTag)
                        {
                            case OpenTypeTags.GPOS:
                                if (num9 == 9)
                                {
                                    var extensionLookupTable = new ExtensionLookupTable(Offset);
                                    num9 = extensionLookupTable.LookupType(table);
                                    Offset = extensionLookupTable.LookupSubtableOffset(table);
                                }

                                switch (num9 - 1)
                                {
                                    case 0:
                                        flag = new SinglePositioningSubtable(Offset).IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    case 1:
                                        flag = new PairPositioningSubtable(Offset).IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    case 2:
                                        flag = CursivePositioningSubtable.IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    case 3:
                                        flag = MarkToBasePositioningSubtable.IsLookupCovered(table,
                                            glyphBits, minGlyphId, maxGlyphId);
                                        continue;
                                    case 4:
                                        flag = MarkToLigaturePositioningSubtable.IsLookupCovered(table,
                                            glyphBits, minGlyphId, maxGlyphId);
                                        continue;
                                    case 5:
                                        flag = MarkToMarkPositioningSubtable.IsLookupCovered(table,
                                            glyphBits, minGlyphId, maxGlyphId);
                                        continue;
                                    case 6:
                                        flag = new ContextSubtable(Offset).IsLookupCovered(table, glyphBits, minGlyphId,
                                            maxGlyphId);
                                        continue;
                                    case 7:
                                        flag = new ChainingSubtable(Offset).IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    case 8:
                                        continue;
                                    default:
                                        flag = true;
                                        continue;
                                }
                            case OpenTypeTags.GSUB:
                                if (num9 == 7)
                                {
                                    var extensionLookupTable = new ExtensionLookupTable(Offset);
                                    num9 = extensionLookupTable.LookupType(table);
                                    Offset = extensionLookupTable.LookupSubtableOffset(table);
                                }

                                switch (num9 - 1)
                                {
                                    case 0:
                                        flag = new SingleSubstitutionSubtable(Offset).IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    case 1:
                                        flag = new MultipleSubstitutionSubtable(Offset).IsLookupCovered(table,
                                            glyphBits, minGlyphId, maxGlyphId);
                                        continue;
                                    case 2:
                                        flag = new AlternateSubstitutionSubtable(Offset).IsLookupCovered(table,
                                            glyphBits, minGlyphId, maxGlyphId);
                                        continue;
                                    case 3:
                                        flag = new LigatureSubstitutionSubtable(Offset).IsLookupCovered(table,
                                            glyphBits, minGlyphId, maxGlyphId);
                                        continue;
                                    case 4:
                                        flag = new ContextSubtable(Offset).IsLookupCovered(table, glyphBits, minGlyphId,
                                            maxGlyphId);
                                        continue;
                                    case 5:
                                        flag = new ChainingSubtable(Offset).IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    case 6:
                                        continue;
                                    case 7:
                                        flag = ReverseChainingSubtable.IsLookupCovered(table, glyphBits,
                                            minGlyphId, maxGlyphId);
                                        continue;
                                    default:
                                        flag = true;
                                        continue;
                                }
                        }
                    }

                    if (!flag)
                        lookupBits[Index3 >> 5] &= (UInt32)~(1 << (Index3 % 32));
                }
            }

            var flag1 = false;
            for (var index = 0; index < (num2 + 31) >> 5; ++index)
            {
                if (lookupBits[index] != 0U)
                {
                    flag1 = true;
                    break;
                }
            }

            if (!flag1)
            {
                complexLanguages = null;
                complexLanguageCount = 0;
            }
            else
            {
                complexLanguages = new WritingSystem[10];
                complexLanguageCount = 0;
                for (UInt16 Index5 = 0; Index5 < scriptCount; ++Index5)
                {
                    var scriptTable = scriptList.GetScriptTable(table, Index5);
                    var scriptTag = scriptList.GetScriptTag(table, Index5);
                    var langSysCount = scriptTable.GetLangSysCount(table);
                    if (scriptTable.IsDefaultLangSysExists(table))
                        AppendLangSys(scriptTag, 1684434036U, scriptTable.GetDefaultLangSysTable(table), featureList,
                            table, featureTagsList, lookupBits, ref complexLanguages, ref complexLanguageCount);
                    for (UInt16 Index6 = 0; Index6 < langSysCount; ++Index6)
                    {
                        var langSysTag = scriptTable.GetLangSysTag(table, Index6);
                        AppendLangSys(scriptTag, langSysTag, scriptTable.GetLangSysTable(table, Index6), featureList,
                            table, featureTagsList, lookupBits, ref complexLanguages, ref complexLanguageCount);
                    }
                }
            }
        }

        private static Boolean IsLookupReversal(OpenTypeTags TableTag,
                                                UInt16 LookupType)
        {
            return TableTag == OpenTypeTags.GSUB && LookupType == 8;
        }

        private static void CompileFeatureSet(Feature[] FeatureSet,
                                              Int32 featureCount,
                                              Int32 featureSetOffset,
                                              Int32 charCount,
                                              FontTable Table,
                                              LangSysTable LangSys,
                                              FeatureList Features,
                                              Int32 lookupCount,
                                              OpenTypeLayoutWorkspace workspace)
        {
            workspace.InitLookupUsageFlags(lookupCount, featureCount);
            var featureTable = LangSys.RequiredFeature(Table, Features);
            if (!featureTable.IsNull)
            {
                var num = (Int32)featureTable.LookupCount(Table);
                for (UInt16 Index = 0; Index < num; ++Index)
                    workspace.SetRequiredFeatureFlag(featureTable.LookupIndex(Table, Index));
            }

            for (var featureIndex = 0; featureIndex < featureCount; ++featureIndex)
            {
                var feature1 = FeatureSet[featureIndex];
                if (feature1.Parameter != 0U && feature1.StartIndex < featureSetOffset + charCount &&
                    feature1.StartIndex + feature1.Length > featureSetOffset)
                {
                    var feature2 = LangSys.FindFeature(Table, Features, feature1.Tag);
                    if (!feature2.IsNull)
                    {
                        var num = (Int32)feature2.LookupCount(Table);
                        for (UInt16 Index = 0; Index < num; ++Index)
                            workspace.SetFeatureFlag(feature2.LookupIndex(Table, Index), featureIndex);
                    }
                }
            }
        }

        private static void GetNextEnabledGlyphRange(Feature[] FeatureSet,
                                                     Int32 featureCount,
                                                     Int32 featureSetOffset,
                                                     FontTable Table,
                                                     OpenTypeLayoutWorkspace workspace,
                                                     LangSysTable LangSys,
                                                     FeatureList Features,
                                                     UInt16 lookupIndex,
                                                     Int32 CharCount,
                                                     UshortList Charmap,
                                                     Int32 StartChar,
                                                     Int32 StartGlyph,
                                                     Int32 GlyphRunLength,
                                                     out Int32 FirstChar,
                                                     out Int32 AfterLastChar,
                                                     out Int32 FirstGlyph,
                                                     out Int32 AfterLastGlyph,
                                                     out UInt32 Parameter)
        {
            FirstChar = Int32.MaxValue;
            AfterLastChar = Int32.MaxValue;
            FirstGlyph = StartGlyph;
            AfterLastGlyph = GlyphRunLength;
            Parameter = 0U;
            if (workspace.IsRequiredFeatureFlagSet(lookupIndex))
            {
                FirstChar = StartChar;
                AfterLastChar = CharCount;
                FirstGlyph = StartGlyph;
                AfterLastGlyph = GlyphRunLength;
            }
            else
            {
                for (var featureIndex = 0; featureIndex < featureCount; ++featureIndex)
                {
                    if (workspace.IsFeatureFlagSet(lookupIndex, featureIndex))
                    {
                        var feature = FeatureSet[featureIndex];
                        var num1 = feature.StartIndex - featureSetOffset;
                        if (num1 < 0)
                            num1 = 0;
                        var num2 = feature.StartIndex + feature.Length - featureSetOffset;
                        if (num2 > CharCount)
                            num2 = CharCount;
                        if (num2 > StartChar && (num1 < FirstChar || num1 == FirstChar && num2 >= AfterLastChar))
                        {
                            FirstChar = num1;
                            AfterLastChar = num2;
                            Parameter = feature.Parameter;
                        }
                    }
                }

                if (FirstChar == Int32.MaxValue)
                {
                    FirstGlyph = GlyphRunLength;
                    AfterLastGlyph = GlyphRunLength;
                }
                else
                {
                    FirstGlyph = StartGlyph <= Charmap[FirstChar] ? Charmap[FirstChar] : StartGlyph;
                    if (AfterLastChar < CharCount)
                        AfterLastGlyph = Charmap[AfterLastChar];
                    else
                        AfterLastGlyph = GlyphRunLength;
                }
            }
        }

        private static void UpdateGlyphFlags(IOpenTypeFont Font,
                                             GlyphInfoList GlyphInfo,
                                             Int32 FirstGlyph,
                                             Int32 AfterLastGlyph,
                                             Boolean DoAll,
                                             GlyphFlags FlagToSet)
        {
            UInt16 num1 = 7;
            var fontTable = Font.GetFontTable(OpenTypeTags.GDEF);
            if (!fontTable.IsPresent)
            {
                for (var index = FirstGlyph; index < AfterLastGlyph; ++index)
                {
                    var glyphFlag = (Int32)GlyphInfo.GlyphFlags[index];
                }
            }
            else
            {
                var glyphClassDef = new GDEFHeader(0).GetGlyphClassDef(fontTable);
                for (var index = FirstGlyph; index < AfterLastGlyph; ++index)
                {
                    var num2 = (UInt16)((GlyphFlags)GlyphInfo.GlyphFlags[index] | FlagToSet);
                    if ((num2 & num1) == 7 || FlagToSet != GlyphFlags.Unassigned)
                    {
                        var glyph = GlyphInfo.Glyphs[index];
                        var num3 = (UInt16)(num2 & (UInt32)~num1);
                        var num4 = (Int32)glyphClassDef.GetClass(fontTable, glyph);
                        GlyphInfo.GlyphFlags[index] = (UInt16)(num3 | (num4 == -1 ? 0 : (UInt16)num4));
                    }
                }
            }
        }

        private static void AppendLangSys(UInt32 scriptTag,
                                          UInt32 langSysTag,
                                          LangSysTable langSysTable,
                                          FeatureList featureList,
                                          FontTable table,
                                          UInt32[] featureTagsList,
                                          UInt32[] lookupBits,
                                          ref WritingSystem[] complexLanguages,
                                          ref Int32 complexLanguageCount)
        {
            var num1 = langSysTable.FeatureCount(table);
            var flag1 = false;
            for (UInt16 Index1 = 0; !flag1 && Index1 < num1; ++Index1)
            {
                var featureIndex = langSysTable.GetFeatureIndex(table, Index1);
                var num2 = featureList.FeatureTag(table, featureIndex);
                var flag2 = false;
                for (var index = 0; !flag1 && index < featureTagsList.Length; ++index)
                {
                    if ((Int32)featureTagsList[index] == (Int32)num2)
                    {
                        flag2 = true;
                        break;
                    }
                }

                if (flag2)
                {
                    var featureTable = featureList.FeatureTable(table, featureIndex);
                    var num3 = featureTable.LookupCount(table);
                    for (UInt16 Index2 = 0; Index2 < num3; ++Index2)
                    {
                        var num4 = featureTable.LookupIndex(table, Index2);
                        if ((lookupBits[num4 >> 5] & (1 << (num4 % 32))) != 0L)
                        {
                            flag1 = true;
                            break;
                        }
                    }
                }
            }

            if (!flag1)
                return;
            if (complexLanguages.Length == complexLanguageCount)
            {
                var writingSystemArray = new WritingSystem[complexLanguages.Length * 3 / 2];
                for (var index = 0; index < complexLanguages.Length; ++index)
                    writingSystemArray[index] = complexLanguages[index];
                complexLanguages = writingSystemArray;
            }

            complexLanguages[complexLanguageCount].scriptTag = scriptTag;
            complexLanguages[complexLanguageCount].langSysTag = langSysTag;
            ++complexLanguageCount;
        }

        public const UInt16 LookupFlagRightToLeft = 1;
        public const UInt16 LookupFlagIgnoreBases = 2;
        public const UInt16 LookupFlagIgnoreLigatures = 4;
        public const UInt16 LookupFlagIgnoreMarks = 8;
        public const UInt16 LookupFlagMarkAttachmentTypeMask = 65280;
        public const UInt16 LookupFlagFindBase = 8;
        public const Int32 LookForward = 1;
        public const Int32 LookBackward = -1;
    }
}
