using System;
using System.Collections;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    /// <summary>
    ///     Implements OpenType layout services cache logic at both caching and using time
    /// </summary>
    public static class OpenTypeLayoutCache
    {
        /// <SecurityNote>
        ///     Critical: Calls critical code and has unsafe code blocks
        ///     TreatAsSafe: Pointers accessed are checked by probe.
        /// </SecurityNote>
        [SecurityCritical]
        [SecurityTreatAsSafe]
        public static void InitCache(IOpenTypeFont font,
                                     OpenTypeTags tableTag,
                                     GlyphInfoList glyphInfo,
                                     OpenTypeLayoutWorkspace workspace)
        {
            Debug.Assert(tableTag == OpenTypeTags.GSUB || tableTag == OpenTypeTags.GPOS);

            var cacheArray = font.GetTableCache(tableTag);

            if (cacheArray == null)
            {
                workspace.TableCacheData = null;
            }
            else
            {
                workspace.TableCacheData = cacheArray;

                workspace.AllocateCachePointers(glyphInfo.Length);
                RenewPointers(glyphInfo, workspace, 0, glyphInfo.Length);
            }
        }

        /// <SecurityNote>
        ///     Critical: Calls critical code
        /// </SecurityNote>
        [SecurityCritical]
        public static void OnGlyphsChanged(OpenTypeLayoutWorkspace workspace,
                                           GlyphInfoList glyphInfo,
                                           Int32 oldLength,
                                           Int32 firstGlyphChanged,
                                           Int32 afterLastGlyphChanged)
        {
            if (workspace.TableCacheData == null)
            {
                return;
            }

            workspace.UpdateCachePointers(oldLength, glyphInfo.Length, firstGlyphChanged, afterLastGlyphChanged);
            RenewPointers(glyphInfo, workspace, firstGlyphChanged, afterLastGlyphChanged);
        }

        /// <summary>
        ///     Find next glyph in lookup. Depending on search direction,
        ///     it will update either firstGlyph or afterLastGlyph
        /// </summary>
        /// <param name="workspace">In: Storage for buffers we need</param>
        /// <param name="glyphInfo">In: Glyph run</param>
        /// <param name="firstLookupIndex">In: Minimal lookup index to search for.</param>
        /// <param name="lookupIndex">Out: Lookup index found</param>
        /// <param name="firstGlyph">Out: First applicable glyph for this lookup</param>
        /// <returns>True if any lookup found, false otherwise</returns>
        /// <SecurityNote>
        ///     Critical:  Accesses font cache pointers
        /// </SecurityNote>
        [SecurityCritical]
        public static void FindNextLookup(OpenTypeLayoutWorkspace workspace,
                                          GlyphInfoList glyphInfo,
                                          UInt16 firstLookupIndex,
                                          out UInt16 lookupIndex,
                                          out Int32 firstGlyph)
        {
            if (firstLookupIndex >= GetCacheLookupCount(workspace))
            {
                // For lookups that did not fit into cache, just say we should always try it
                lookupIndex = firstLookupIndex;
                firstGlyph = 0;
                return;
            }

            var cachePointers = workspace.CachePointers;
            var glyphCount = glyphInfo.Length;

            lookupIndex = 0xffff;
            firstGlyph = 0;

            for (var i = 0; i < glyphCount; i++)
            {
                // Sync up inside the list up to the minimal lookup requested
                // No additional boundary checks are necessary, because every list terminates with 0xffff
                while (cachePointers[i] < firstLookupIndex) cachePointers[i]++;
                //Now we know that our index is higher or equal than firstLookup index

                if (cachePointers[i] < lookupIndex)
                {
                    // We now have new minimum
                    lookupIndex = cachePointers[i];
                    firstGlyph = i;
                }
            }

            if (lookupIndex == 0xffff)
            {
                // We can't just say we are done, there may be lookups that did not fit into cache
                lookupIndex = GetCacheLookupCount(workspace);
                firstGlyph = 0;
            }
        }

        /// <summary>
        ///     Find next glyph in lookup. Depending on search direction,
        ///     it will update either firstGlyph or afterLastGlyph
        /// </summary>
        /// <param name="workspace">Storage for buffers we need</param>
        /// <param name="lookupIndex">Current lookup in processing</param>
        /// <param name="isLookupReversal">Do we go forward or backwards</param>
        /// <param name="firstGlyph">first glyph of search range</param>
        /// <param name="afterLastGlyph">position after last glyph</param>
        /// <returns>True if any glyph found, false otherwise</returns>
        /// <SecurityNote>
        ///     Critical: unsafe pointer operations
        /// </SecurityNote>
        [SecurityCritical]
        public static Boolean FindNextGlyphInLookup(OpenTypeLayoutWorkspace workspace,
                                                    UInt16 lookupIndex,
                                                    Boolean isLookupReversal,
                                                    ref Int32 firstGlyph,
                                                    ref Int32 afterLastGlyph)
        {
            if (lookupIndex >= GetCacheLookupCount(workspace))
            {
                return true;
            }

            var cachePointers = workspace.CachePointers;

            if (!isLookupReversal)
            {
                for (var i = firstGlyph; i < afterLastGlyph; i++)
                {
                    if (cachePointers[i] == lookupIndex)
                    {
                        firstGlyph = i;
                        return true;
                    }
                }

                return false;
            }

            for (var i = afterLastGlyph - 1; i >= firstGlyph; i--)
            {
                if (cachePointers[i] == lookupIndex)
                {
                    afterLastGlyph = i + 1;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets number of lookups that fit into table cache
        /// </summary>
        /// <param name="workspace">In: Storage for buffers we need</param>
        /// <returns>Number of lookups in cache</returns>
        /// <SecurityNote>
        ///     Critical:  Accesses font cache pointers
        /// </SecurityNote>
        [SecurityCritical]
        private static unsafe UInt16 GetCacheLookupCount(OpenTypeLayoutWorkspace workspace)
        {
            // If there is no chache, just exit
            if (workspace.TableCacheData == null)
            {
                return 0;
            }

            fixed (Byte* pCacheByte = &workspace.TableCacheData[0])
            {
                var pCache = (UInt16*)pCacheByte;

                return pCache[2];
            }
        }

        /// <SecurityNote>
        ///     Critical: unsafe pointer operations
        /// </SecurityNote>
        [SecurityCritical]
        private static unsafe void RenewPointers(GlyphInfoList glyphInfo,
                                                 OpenTypeLayoutWorkspace workspace,
                                                 Int32 firstGlyph,
                                                 Int32 afterLastGlyph)
        {
            fixed (Byte* pCache = &workspace.TableCacheData[0])
            {
                // If there is no chache, just exit
                if (pCache == null)
                {
                    return;
                }

                var cachePointers = workspace.CachePointers;

                for (var i = firstGlyph; i < afterLastGlyph; i++)
                {
                    var glyph = glyphInfo.Glyphs[i];

                    // If glyph is not there, we will point to the constant 0xFFFF in the cache
                    var listOffset = 2;

                    //Find glyph entry in the cache
                    Int32 glyphCount = *((UInt16*)pCache + 3);
                    var pGlyphs = (UInt16*)pCache + 4;
                    Int32 low = 0, high = glyphCount;

                    while (low < high)
                    {
                        var mid = (low + high) >> 1;
                        var midGlyph = pGlyphs[mid * 2];

                        if (glyph < midGlyph)
                        {
                            high = mid;
                            continue;
                        }

                        if (glyph > midGlyph)
                        {
                            low = mid + 1;
                            continue;
                        }

                        // Found it!
                        listOffset = pGlyphs[mid * 2 + 1];
                        break;
                    }

                    // Whether we found glyph in the cache or not,
                    // Pointer will be set to the list, but it may be empty.
                    cachePointers[i] = *(UInt16*)(pCache + listOffset);
                }
            }
        }

        #region Cache filling

        /// <SecurityNote>
        ///     Critical: Calls critical code
        /// </SecurityNote>
        [SecurityCritical]
        public static void CreateCache(IOpenTypeFont font,
                                       Int32 maxCacheSize)
        {
            if (maxCacheSize > UInt16.MaxValue)
            {
                // Data structures do not support cache sizes more than 64K.
                maxCacheSize = UInt16.MaxValue;
            }

            Int32 tableCacheSize;
            var totalSize = 0;

            CreateTableCache(font, OpenTypeTags.GSUB, maxCacheSize - totalSize, out tableCacheSize);
            totalSize += tableCacheSize;
            Debug.Assert(totalSize <= maxCacheSize);

            CreateTableCache(font, OpenTypeTags.GPOS, maxCacheSize - totalSize, out tableCacheSize);
            totalSize += tableCacheSize;
            Debug.Assert(totalSize <= maxCacheSize);
        }

        /// <SecurityNote>
        ///     Critical: calling FillTableCache to change cache content in unmanaged memory
        /// </SecurityNote>
        [SecurityCritical]
        private static void CreateTableCache(IOpenTypeFont font,
                                             OpenTypeTags tableTag,
                                             Int32 maxCacheSize,
                                             out Int32 tableCacheSize)
        {
            // Initialize all computed values
            tableCacheSize = 0;
            var cacheSize = 0;
            var recordCount = 0;
            var glyphCount = 0;
            var lastLookupAdded = -1;
            GlyphLookupRecord[] records = null;

            try
            {
                ComputeTableCache(
                    font,
                    tableTag,
                    maxCacheSize,
                    ref cacheSize,
                    ref records,
                    ref recordCount,
                    ref glyphCount,
                    ref lastLookupAdded
                );
            }
            catch (FormatException)
            {
                cacheSize = 0;
            }

            if (cacheSize > 0)
            {
                tableCacheSize = FillTableCache(
                    font,
                    tableTag,
                    cacheSize,
                    records,
                    recordCount,
                    glyphCount,
                    lastLookupAdded
                );
            }
        }


        /// <SecurityNote>
        ///     Critical: Accessing raw font table
        /// </SecurityNote>
        [SecurityCritical]
        private static void ComputeTableCache(IOpenTypeFont font,
                                              OpenTypeTags tableTag,
                                              Int32 maxCacheSize,
                                              ref Int32 cacheSize,
                                              ref GlyphLookupRecord[] records,
                                              ref Int32 recordCount,
                                              ref Int32 glyphCount,
                                              ref Int32 lastLookupAdded)
        {
            var table = font.GetFontTable(tableTag);

            if (!table.IsPresent)
            {
                return;
            }

            FeatureList featureList;
            LookupList lookupList;

            Debug.Assert(tableTag == OpenTypeTags.GSUB || tableTag == OpenTypeTags.GPOS);

            switch (tableTag)
            {
                case OpenTypeTags.GSUB:
                {
                    var header = new GSUBHeader();
                    featureList = header.GetFeatureList(table);
                    lookupList = header.GetLookupList(table);
                    break;
                }
                case OpenTypeTags.GPOS:
                {
                    var header = new GPOSHeader();
                    featureList = header.GetFeatureList(table);
                    lookupList = header.GetLookupList(table);
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    featureList = new FeatureList(0);
                    lookupList = new LookupList(0);
                    break;
                }
            }

            // Estimate number of records that can fit into cache using ratio of approximately 
            // 4 bytes of cache per actual record. Most of fonts will fit into this value, except 
            // some tiny caches and big EA font that can have ratio of around 5 (theoretical maximum is 8).
            //
            // If actual ratio for particluar font will be larger than 4, we will remove records 
            // from the end to fit into cache.
            //
            // If ratio is less than 4 we actually can fit more lookups, but for the speed and because most fonts
            // will fit into cache completely anyway we do not do anything about this here.
            var maxRecordCount = maxCacheSize / 4;

            // For now, we will just allocate array of maximum size.
            // Given heuristics above, it wont be greater than max cache size.
            // 
            records = new GlyphLookupRecord[maxRecordCount];

            //
            // Now iterate through lookups and subtables, filling in lookup-glyph pairs list
            //
            Int32 lookupCount = lookupList.LookupCount(table);
            var recordCountAfterLastLookup = 0;

            //
            // Not all lookups can be invoked from feature directly,
            // they are actions from contextual lookups.
            // We are not interested in those, because they will
            // never work from high level, so do not bother adding them to the cache.
            //
            // Filling array of lookup usage bits, to skip those not mapped to any lookup
            //
            var lookupUsage = new BitArray(lookupCount);

            for (UInt16 feature = 0; feature < featureList.FeatureCount(table); feature++)
            {
                var featureTable = featureList.FeatureTable(table, feature);

                for (UInt16 lookup = 0; lookup < featureTable.LookupCount(table); lookup++)
                {
                    var lookupIndex = featureTable.LookupIndex(table, lookup);

                    if (lookupIndex >= lookupCount)
                    {
                        // This must be an invalid font. Just igonoring this lookup here.
                        continue;
                    }

                    lookupUsage[lookupIndex] = true;
                }
            }
            // Done with lookup usage bits

            for (UInt16 lookupIndex = 0; lookupIndex < lookupCount; lookupIndex++)
            {
                if (!lookupUsage[lookupIndex])
                {
                    continue;
                }

                var firstLookupRecord = recordCount;
                var maxLookupGlyph = -1;
                var cacheIsFull = false;

                var lookup = lookupList.Lookup(table, lookupIndex);
                var lookupType = lookup.LookupType();
                var subtableCount = lookup.SubTableCount();

                for (UInt16 subtableIndex = 0; subtableIndex < subtableCount; subtableIndex++)
                {
                    var subtableOffset = lookup.SubtableOffset(table, subtableIndex);

                    var coverage = GetSubtablePrincipalCoverage(table, tableTag, lookupType, subtableOffset);

                    if (coverage.IsInvalid) continue;

                    cacheIsFull = !AppendCoverageGlyphRecords(table, lookupIndex, coverage, records, ref recordCount,
                        ref maxLookupGlyph);

                    if (cacheIsFull) break;
                }

                if (cacheIsFull) break;

                lastLookupAdded = lookupIndex;
                recordCountAfterLastLookup = recordCount;
            }

            // We may hit storage overflow in the middle of lookup. Throw this partial lookup away
            recordCount = recordCountAfterLastLookup;

            if (lastLookupAdded == -1)
            {
                // We did not succeed adding even single lookup.
                return;
            }

            // We now have glyph records for (may be not all) lookups in the table.
            // Cache structures should be sorted by glyph, then by lookup index.
            Array.Sort(records, 0, recordCount);

            cacheSize = -1;
            glyphCount = -1;

            // It may happen, that records do not fit into cache, even using our heuristics. 
            // We will remove lookups one by one from the end until it fits.
            while (recordCount > 0)
            {
                CalculateCacheSize(records, recordCount, out cacheSize, out glyphCount);

                if (cacheSize <= maxCacheSize)
                {
                    // Fine, we now fit into max cache size
                    break;
                }

                // Find last lookup index
                var lastLookup = -1;
                for (var i = 0; i < recordCount; i++)
                {
                    Int32 lookup = records[i].Lookup;

                    if (lastLookup < lookup)
                    {
                        lastLookup = lookup;
                    }
                }

                Debug.Assert(lastLookup >= 0); // There are lookups, so there was an index

                // Remove it
                var currentRecord = 0;
                for (var i = 0; i < recordCount; i++)
                {
                    if (records[i].Lookup == lastLookup) continue;

                    if (currentRecord == i) continue;

                    records[currentRecord] = records[i];
                    currentRecord++;
                }

                recordCount = currentRecord;

                // Do not forget update lastLookupAdded variable
                lastLookupAdded = lastLookup - 1;
            }

            if (recordCount == 0)
            {
                // We can't fit even single lookup into the cache
                return;
            }

            Debug.Assert(cacheSize > 0); // We've calcucalted them at least ones, and 
            Debug.Assert(glyphCount > 0); // if there is no records, we already should've exited
        }


        /// <SecurityNote>
        ///     Critical: unsafe pointer operations
        /// </SecurityNote>
        [SecurityCritical]
        private static Int32 FillTableCache(IOpenTypeFont font,
                                            OpenTypeTags tableTag,
                                            Int32 cacheSize,
                                            GlyphLookupRecord[] records,
                                            Int32 recordCount,
                                            Int32 glyphCount,
                                            Int32 lastLookupAdded)
        {
            // Fill the cache.

            // We are using basically the same code to fill the cache 
            // that had been used to calculate the size. So pList pointer
            // moving through cache memory should not overrun allocated space.
            // Asserts are set to chek that at every place where we write to cache
            // and at the end where we check that we filled exactly the same amount.

            unsafe
            {
                var cache = font.AllocateTableCache(tableTag, cacheSize);
                if (cache == null)
                {
                    // We failed to allocate cache of requested size, 
                    // exit without created cache.
                    return 0;
                }

                fixed (Byte* pCacheByte = &cache[0])
                {
                    var pCache = (UInt16*)pCacheByte;

                    pCache[0] = (UInt16)cacheSize; // Cache size
                    pCache[1] = 0xFFFF; // 0xFFFF constants
                    pCache[2] = (UInt16)(lastLookupAdded + 1); // Number of lookups that fit into the cache
                    pCache[3] = (UInt16)glyphCount; // Glyph count

                    var pGlyphs = pCache + 4;
                    var pList = pGlyphs + glyphCount * 2;
                    UInt16* pPrevList = null;

                    Int32 prevListIndex = -1, prevListLength = 0;
                    Int32 curListIndex = 0, curListLength = 1;
                    var curGlyph = records[0].Glyph;

                    for (var i = 1; i < recordCount; i++)
                    {
                        if (records[i].Glyph != curGlyph)
                        {
                            // We've found another list. Compare it with previous
                            if (prevListLength != curListLength || // Fast check to avoid full comparison
                                !CompareGlyphRecordLists(records,
                                    recordCount,
                                    prevListIndex,
                                    curListIndex)
                               )
                            {
                                // New list. Remember position in pPrevList and write list down
                                pPrevList = pList;

                                for (var j = curListIndex; j < i; j++)
                                {
                                    Debug.Assert((pList - pCache) * sizeof(UInt16) < cacheSize);
                                    *pList = records[j].Lookup;
                                    pList++;
                                }

                                Debug.Assert((pList - pCache) * sizeof(UInt16) < cacheSize);
                                *pList = 0xFFFF;
                                pList++;
                            }
                            // Now pPrevList points at the first element of the correct list.

                            *pGlyphs = curGlyph; // Write down glyph id
                            pGlyphs++;
                            *pGlyphs = (UInt16)((pPrevList - pCache) * sizeof(UInt16)); // Write down list offset
                            pGlyphs++;

                            prevListIndex = curListIndex;
                            prevListLength = curListLength;

                            curGlyph = records[i].Glyph;
                            curListIndex = i;
                            curListLength = 1;
                        }
                    }

                    // And we need to check the last list we missed in the loop
                    if (prevListLength != curListLength || // Fast check to avoid full comparison
                        !CompareGlyphRecordLists(records,
                            recordCount,
                            prevListIndex,
                            curListIndex)
                       )
                    {
                        // New list. Remember position in pPrevList and write list down
                        pPrevList = pList;

                        for (var j = curListIndex; j < recordCount; j++)
                        {
                            Debug.Assert((pList - pCache) * sizeof(UInt16) < cacheSize);
                            *pList = records[j].Lookup;
                            pList++;
                        }

                        Debug.Assert((pList - pCache) * sizeof(UInt16) < cacheSize);
                        *pList = 0xFFFF;
                        pList++;
                    }
                    // Now pPrevList points at the first element of the correct list.

                    *pGlyphs = curGlyph; // Write down glyph id
                    pGlyphs++;
                    *pGlyphs = (UInt16)((pPrevList - pCache) * sizeof(UInt16)); // Write down list offset
                    pGlyphs++;

                    // We are done with the cache
                    Debug.Assert((pList - pCache) * sizeof(UInt16) == cacheSize); // We exactly filled up the cache
                    Debug.Assert((pGlyphs - pCache) * sizeof(UInt16) ==
                                 (4 + glyphCount * 2) * sizeof(UInt16)); // Glyphs ended where lists start.
                }
            }

            return cacheSize;
        }

        private static void CalculateCacheSize(GlyphLookupRecord[] records,
                                               Int32 recordCount,
                                               out Int32 cacheSize,
                                               out Int32 glyphCount)
        {
            // Calc cache size
            glyphCount = 1;
            var listCount = 0;
            var entryCount = 0;

            Int32 prevListIndex = -1, prevListLength = 0;
            Int32 curListIndex = 0, curListLength = 1;
            var curGlyph = records[0].Glyph;

            for (var i = 1; i < recordCount; i++)
            {
                if (records[i].Glyph != curGlyph)
                {
                    ++glyphCount;

                    // We've found another list. Compare it with previous
                    if (prevListLength != curListLength || // Fast check to avoid full comparison
                        !CompareGlyphRecordLists(records,
                            recordCount,
                            prevListIndex,
                            curListIndex)
                       )
                    {
                        listCount++;
                        entryCount += curListLength;
                    }

                    prevListIndex = curListIndex;
                    prevListLength = curListLength;

                    curGlyph = records[i].Glyph;
                    curListIndex = i;
                    curListLength = 1;
                }
                else
                {
                    ++curListLength;
                }
            }

            // And we need to check the last list we missed in the loop
            if (prevListLength != curListLength || // Fast check to avoid full comparison
                !CompareGlyphRecordLists(records,
                    recordCount,
                    prevListIndex,
                    curListIndex)
               )
            {
                listCount++;
                entryCount += curListLength;
            }

            cacheSize = sizeof(UInt16) *
                        (1 + // TotalCacheSize
                         1 + // Constant 0xFFFF, so we can point to it from glyphs that are not there
                         1 + // Number of lookups that fit into the cache
                         1 + // glyph count
                         glyphCount * 2 + // {glyphId; listOffset} per glyph
                         entryCount + // Each entry has lookup index
                         listCount // Plus, terminator entry for each list
                        );
        }

        private static Boolean CompareGlyphRecordLists(GlyphLookupRecord[] records,
                                                       Int32 recordCount,
                                                       Int32 glyphListIndex1,
                                                       Int32 glyphListIndex2)
        {
            var listGlyph1 = records[glyphListIndex1].Glyph;
            var listGlyph2 = records[glyphListIndex2].Glyph;

            while (true)
            {
                UInt16 glyph1, glyph2;
                UInt16 lookup1, lookup2;

                if (glyphListIndex1 != recordCount)
                {
                    glyph1 = records[glyphListIndex1].Glyph;
                    lookup1 = records[glyphListIndex1].Lookup;
                }
                else
                {
                    // Just emulate something that will be never in the real input
                    glyph1 = 0xffff;
                    lookup1 = 0xffff;
                }

                if (glyphListIndex2 != recordCount)
                {
                    glyph2 = records[glyphListIndex2].Glyph;
                    lookup2 = records[glyphListIndex2].Lookup;
                }
                else
                {
                    // Just emulate something that will be never in the real input.
                    glyph2 = 0xffff;
                    lookup2 = 0xffff;
                }

                if (glyph1 != listGlyph1 && glyph2 != listGlyph2)
                {
                    // Both lists are ended at the same time.
                    return true;
                }

                if (glyph1 != listGlyph1 || glyph2 != listGlyph2)
                {
                    // One list is ended, another does not.
                    return false;
                }

                if (lookup1 != lookup2)
                {
                    // We have different lookups on the lists.
                    return false;
                }

                //Lists match so far, move further
                ++glyphListIndex1;
                ++glyphListIndex2;
            }
        }

        /// <SecurityNote>
        ///     Critical: Calls critical code
        /// </SecurityNote>
        [SecurityCritical]
        private static CoverageTable GetSubtablePrincipalCoverage(FontTable table,
                                                                  OpenTypeTags tableTag,
                                                                  UInt16 lookupType,
                                                                  Int32 subtableOffset)
        {
            Debug.Assert(tableTag == OpenTypeTags.GSUB || tableTag == OpenTypeTags.GPOS);

            var coverage = CoverageTable.InvalidCoverage;

            switch (tableTag)
            {
                case OpenTypeTags.GSUB:
                    if (lookupType == 7)
                    {
                        var extension =
                            new ExtensionLookupTable(subtableOffset);

                        lookupType = extension.LookupType(table);
                        subtableOffset = extension.LookupSubtableOffset(table);
                    }

                    switch (lookupType)
                    {
                        case 1: //SingleSubst
                            var singleSubst =
                                new SingleSubstitutionSubtable(subtableOffset);

                            return singleSubst.GetPrimaryCoverage(table);

                        case 2: //MultipleSubst 
                            var multipleSub =
                                new MultipleSubstitutionSubtable(subtableOffset);
                            return multipleSub.GetPrimaryCoverage(table);

                        case 3: //AlternateSubst
                            var alternateSub =
                                new AlternateSubstitutionSubtable(subtableOffset);
                            return alternateSub.GetPrimaryCoverage(table);

                        case 4: //Ligature subst
                            var ligaSub =
                                new LigatureSubstitutionSubtable(subtableOffset);
                            return ligaSub.GetPrimaryCoverage(table);

                        case 5: //ContextualSubst
                            var contextSub =
                                new ContextSubtable(subtableOffset);
                            return contextSub.GetPrimaryCoverage(table);

                        case 6: //ChainingSubst
                            var chainingSub =
                                new ChainingSubtable(subtableOffset);
                            return chainingSub.GetPrimaryCoverage(table);

                        case 7: //Extension lookup
                            // Ext.Lookup processed earlier. It can't contain another ext.lookups in it
                            break;

                        case 8: //ReverseCahiningSubst
                            var reverseChainingSub =
                                new ReverseChainingSubtable(subtableOffset);
                            return reverseChainingSub.GetPrimaryCoverage(table);
                    }

                    break;

                case OpenTypeTags.GPOS:
                    if (lookupType == 9)
                    {
                        var extension =
                            new ExtensionLookupTable(subtableOffset);

                        lookupType = extension.LookupType(table);
                        subtableOffset = extension.LookupSubtableOffset(table);
                    }

                    switch (lookupType)
                    {
                        case 1: //SinglePos
                            var singlePos =
                                new SinglePositioningSubtable(subtableOffset);
                            return singlePos.GetPrimaryCoverage(table);

                        case 2: //PairPos
                            var pairPos =
                                new PairPositioningSubtable(subtableOffset);
                            return pairPos.GetPrimaryCoverage(table);

                        case 3: // CursivePos
                            var cursivePos =
                                new CursivePositioningSubtable(subtableOffset);
                            return cursivePos.GetPrimaryCoverage(table);

                        case 4: //MarkToBasePos
                            var markToBasePos =
                                new MarkToBasePositioningSubtable(subtableOffset);
                            return markToBasePos.GetPrimaryCoverage(table);

                        case 5: //MarkToLigaturePos
                            // Under construction
                            var markToLigaPos =
                                new MarkToLigaturePositioningSubtable(subtableOffset);
                            return markToLigaPos.GetPrimaryCoverage(table);

                        case 6: //MarkToMarkPos
                            var markToMarkPos =
                                new MarkToMarkPositioningSubtable(subtableOffset);
                            return markToMarkPos.GetPrimaryCoverage(table);

                        case 7: // Contextual
                            var contextPos =
                                new ContextSubtable(subtableOffset);
                            return contextPos.GetPrimaryCoverage(table);

                        case 8: // Chaining
                            var chainingPos =
                                new ChainingSubtable(subtableOffset);
                            return chainingPos.GetPrimaryCoverage(table);

                        case 9: //Extension lookup
                            // Ext.Lookup processed earlier. It can't contain another ext.lookups in it
                            break;
                    }

                    break;
            }

            return CoverageTable.InvalidCoverage;
        }

        /// <summary>
        ///     Append lookup coverage table to the list.
        /// </summary>
        /// <param name="table">Font table</param>
        /// <param name="lookupIndex">Lookup index</param>
        /// <param name="coverage">Lookup principal coverage</param>
        /// <param name="records">Record array</param>
        /// <param name="recordCount">Real number of records in record array</param>
        /// <param name="maxLookupGlyph">Highest glyph index that we saw in this lookup</param>
        /// <returns>Returns false if we are out of list space</returns>
        /// <SecurityNote>
        ///     Critical: Calls critical code
        /// </SecurityNote>
        [SecurityCritical]
        private static Boolean AppendCoverageGlyphRecords(FontTable table,
                                                          UInt16 lookupIndex,
                                                          CoverageTable coverage,
                                                          GlyphLookupRecord[] records,
                                                          ref Int32 recordCount,
                                                          ref Int32 maxLookupGlyph)
        {
            switch (coverage.Format(table))
            {
                case 1:
                    var glyphCount = coverage.Format1GlyphCount(table);

                    for (UInt16 i = 0; i < glyphCount; i++)
                    {
                        var glyph = coverage.Format1Glyph(table, i);

                        if (!AppendGlyphRecord(glyph, lookupIndex, records, ref recordCount, ref maxLookupGlyph))
                        {
                            // We've failed to add another record.
                            return false;
                        }
                    }

                    break;

                case 2:

                    var rangeCount = coverage.Format2RangeCount(table);

                    for (UInt16 i = 0; i < rangeCount; i++)
                    {
                        var firstGlyph = coverage.Format2RangeStartGlyph(table, i);
                        var lastGlyph = coverage.Format2RangeEndGlyph(table, i);

                        for (Int32 glyph = firstGlyph; glyph <= lastGlyph; glyph++)
                        {
                            if (!AppendGlyphRecord((UInt16)glyph, lookupIndex, records, ref recordCount,
                                    ref maxLookupGlyph))
                            {
                                // We've failed to add another record.
                                return false;
                            }
                        }
                    }

                    break;
            }

            return true;
        }

        /// <summary>
        ///     Append record to the list, but first check if we have duplicate.
        /// </summary>
        /// <param name="glyph">Glyph</param>
        /// <param name="lookupIndex">Lookup index</param>
        /// <param name="records">Record array</param>
        /// <param name="recordCount">Real number of records in record array</param>
        /// <param name="maxLookupGlyph">Highest glyph index that we saw in this lookup</param>
        /// <returns>Returns false if we are out of list space</returns>
        private static Boolean AppendGlyphRecord(UInt16 glyph,
                                                 UInt16 lookupIndex,
                                                 GlyphLookupRecord[] records,
                                                 ref Int32 recordCount,
                                                 ref Int32 maxLookupGlyph)
        {
            if (glyph == maxLookupGlyph)
            {
                // It is exactly max, which means we already've seen it before.
                return true;
            }

            if (glyph > maxLookupGlyph)
            {
                // This should be very common - coverage tables are ordered by glyph index.
                maxLookupGlyph = glyph;
            }
            else
            {
                // We will go through records to check for duplicate.
                Debug.Assert(recordCount > 0); // Otherwise, we would go into (glyph > maxGlyphLookup);
                for (var i = recordCount - 1; i >= 0; i--)
                {
                    if (records[i].Lookup != lookupIndex)
                    {
                        // We've iterated through all lookup records
                        // (and haven't found duplicate)
                        break;
                    }

                    if (records[i].Glyph == glyph)
                    {
                        // We found duplicate, no need to do anything. 
                        return true;
                    }
                }
            }

            // Now, we need to add new record

            if (recordCount == records.Length)
            {
                // There is no space for new record.
                return false;
            }

            records[recordCount] = new GlyphLookupRecord(glyph, lookupIndex);
            recordCount++;

            return true;
        }

        private class GlyphLookupRecord : IComparable<GlyphLookupRecord>
        {
            public GlyphLookupRecord(UInt16 glyph,
                                     UInt16 lookup)
            {
                _glyph = glyph;
                _lookup = lookup;
            }

            // Records will be sorted by glyph, then by lookup index
            public Int32 CompareTo(GlyphLookupRecord value)
            {
                if (_glyph < value._glyph) return -1;
                if (_glyph > value._glyph) return 1;

                if (_lookup < value._lookup) return -1;
                if (_lookup > value._lookup) return 1;

                return 0;
            }

            public Boolean Equals(GlyphLookupRecord value)
            {
                return _glyph == value._glyph &&
                       _lookup == value._lookup;
            }

            public static Boolean operator ==(GlyphLookupRecord value1,
                                              GlyphLookupRecord value2)
            {
                return value1.Equals(value2);
            }

            public static Boolean operator !=(GlyphLookupRecord value1,
                                              GlyphLookupRecord value2)
            {
                return !value1.Equals(value2);
            }

            public override Boolean Equals(Object value)
            {
                return Equals((GlyphLookupRecord)value);
            }

            public override Int32 GetHashCode()
            {
                return _glyph << (16 + _lookup);
            }

            public UInt16 Glyph => _glyph;

            public UInt16 Lookup => _lookup;

            private readonly UInt16 _glyph;
            private readonly UInt16 _lookup;
        }

        #endregion Cache filling
    }
}
