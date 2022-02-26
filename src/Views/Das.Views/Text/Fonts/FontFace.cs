//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Das.Views.Text.Fonts
//{
//    public sealed class FontFace : IDisposable
//  {
//    private NativeIUnknownWrapper\u003CMS\u003A\u003AInternal\u003A\u003AText\u003A\u003ATextInterface\u003A\u003ANative\u003A\u003AIDWriteFontFace\u003E _fontFace;
//    private FontMetrics _fontMetrics;
//    private int _refCount;

//    internal unsafe FontFace(IDWriteFontFace* fontFace) => this._fontFace = new NativeIUnknownWrapper\u003CMS\u003A\u003AInternal\u003A\u003AText\u003A\u003ATextInterface\u003A\u003ANative\u003A\u003AIDWriteFontFace\u003E((IUnknown*) fontFace);

//    internal unsafe IDWriteFontFace* DWriteFontFaceNoAddRef => this._fontFace.Value;

//    internal unsafe IntPtr DWriteFontFaceAddRef
//    {
//      get
//      {
//        IDWriteFontFace* idWriteFontFacePtr = this._fontFace.Value;
//        int num = (int) __calli((__FnPtr<uint (IntPtr)>) *(long*) (*(long*) idWriteFontFacePtr + 8L))((IntPtr) idWriteFontFacePtr);
//        GC.KeepAlive((object) this);
//        return (IntPtr) (void*) this._fontFace.Value;
//      }
//    }

//    internal unsafe FontFaceType Type
//    {
//      get
//      {
//        IDWriteFontFace* idWriteFontFacePtr = this._fontFace.Value;
//        int fontFaceType = (int) __calli((__FnPtr<DWRITE_FONT_FACE_TYPE (IntPtr)>) *(long*) (*(long*) idWriteFontFacePtr + 24L))((IntPtr) idWriteFontFacePtr);
//        GC.KeepAlive((object) this._fontFace);
//        GC.KeepAlive((object) this);
//        return DWriteTypeConverter.Convert((DWRITE_FONT_FACE_TYPE) fontFaceType);
//      }
//    }

//    internal unsafe uint Index
//    {
//      get
//      {
//        IDWriteFontFace* idWriteFontFacePtr = this._fontFace.Value;
//        int index = (int) __calli((__FnPtr<uint (IntPtr)>) *(long*) (*(long*) idWriteFontFacePtr + 40L))((IntPtr) idWriteFontFacePtr);
//        GC.KeepAlive((object) this._fontFace);
//        GC.KeepAlive((object) this);
//        return (uint) index;
//      }
//    }

//    internal unsafe FontSimulations SimulationFlags
//    {
//      get
//      {
//        IDWriteFontFace* idWriteFontFacePtr = this._fontFace.Value;
//        int fontSimulations = (int) __calli((__FnPtr<DWRITE_FONT_SIMULATIONS (IntPtr)>) *(long*) (*(long*) idWriteFontFacePtr + 48L))((IntPtr) idWriteFontFacePtr);
//        GC.KeepAlive((object) this._fontFace);
//        GC.KeepAlive((object) this);
//        return DWriteTypeConverter.Convert((DWRITE_FONT_SIMULATIONS) fontSimulations);
//      }
//    }

//    internal unsafe bool IsSymbolFont
//    {
//      get
//      {
//        IDWriteFontFace* idWriteFontFacePtr = this._fontFace.Value;
//        int num = __calli((__FnPtr<int (IntPtr)>) *(long*) (*(long*) idWriteFontFacePtr + 56L))((IntPtr) idWriteFontFacePtr);
//        GC.KeepAlive((object) this._fontFace);
//        GC.KeepAlive((object) this);
//        return num != 0;
//      }
//    }

//    internal unsafe FontMetrics Metrics
//    {
//      get
//      {
//        if (this._fontMetrics == null)
//        {
//          IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//          IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//          DWRITE_FONT_METRICS dwriteFontMetrics;
//          ref DWRITE_FONT_METRICS local = ref dwriteFontMetrics;
//          __calli((__FnPtr<void (IntPtr, DWRITE_FONT_METRICS*)>) *(long*) (*(long*) idWriteFontFacePtr1 + 64L))((DWRITE_FONT_METRICS*) idWriteFontFacePtr2, (IntPtr) ref local);
//          GC.KeepAlive((object) this._fontFace);
//          this._fontMetrics = DWriteTypeConverter.Convert(dwriteFontMetrics);
//        }
//        GC.KeepAlive((object) this);
//        return this._fontMetrics;
//      }
//    }

//    internal unsafe ushort GlyphCount
//    {
//      get
//      {
//        IDWriteFontFace* idWriteFontFacePtr = this._fontFace.Value;
//        ushort glyphCount = __calli((__FnPtr<ushort (IntPtr)>) *(long*) (*(long*) idWriteFontFacePtr + 72L))((IntPtr) idWriteFontFacePtr);
//        GC.KeepAlive((object) this._fontFace);
//        GC.KeepAlive((object) this);
//        return glyphCount;
//      }
//    }

//    internal unsafe FontFile GetFileZero()
//    {
//      uint num1 = 0;
//      IDWriteFontFile* fontFile = (IDWriteFontFile*) 0L;
//      IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//      IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//      ref uint local1 = ref num1;
//      // ISSUE: cast to a function pointer type
//      // ISSUE: function pointer call
//      Util.ConvertHresultToException(__calli((__FnPtr<int (IntPtr, uint*, IDWriteFontFile**)>) *(long*) (*(long*) idWriteFontFacePtr1 + 32L))((IDWriteFontFile**) idWriteFontFacePtr2, (uint*) ref local1, IntPtr.Zero));
//      if (num1 > 0U)
//      {
//        IDWriteFontFile** block = (IDWriteFontFile**) \u003CModule\u003E.@new((ulong) num1 * 8UL);
//        try
//        {
//          IDWriteFontFace* idWriteFontFacePtr3 = this._fontFace.Value;
//          IDWriteFontFace* idWriteFontFacePtr4 = idWriteFontFacePtr3;
//          ref uint local2 = ref num1;
//          IDWriteFontFile** idWriteFontFilePtr1 = block;
//          // ISSUE: cast to a function pointer type
//          // ISSUE: function pointer call
//          Util.ConvertHresultToException(__calli((__FnPtr<int (IntPtr, uint*, IDWriteFontFile**)>) *(long*) (*(long*) idWriteFontFacePtr3 + 32L))((IDWriteFontFile**) idWriteFontFacePtr4, (uint*) ref local2, (IntPtr) idWriteFontFilePtr1));
//          fontFile = (IDWriteFontFile*) *(long*) block;
//          for (uint index = 1; index < num1; ++index)
//          {
//            IDWriteFontFile** idWriteFontFilePtr2 = (IDWriteFontFile**) ((long) index * 8L + (IntPtr) block);
//            long num2 = *(long*) idWriteFontFilePtr2;
//            // ISSUE: cast to a function pointer type
//            // ISSUE: function pointer call
//            int num3 = (int) __calli((__FnPtr<uint (IntPtr)>) *(long*) (*(long*) num2 + 16L))((IntPtr) num2);
//            *(long*) idWriteFontFilePtr2 = 0L;
//          }
//        }
//        finally
//        {
//          \u003CModule\u003E.delete((void*) block);
//        }
//      }
//      GC.KeepAlive((object) this._fontFace);
//      GC.KeepAlive((object) this);
//      return num1 > 0U ? new FontFile(fontFile) : (FontFile) null;
//    }

//    internal void AddRef() => Interlocked.Increment(ref this._refCount);

//    internal void Release()
//    {
//      if (-1 != Interlocked.Decrement(ref this._refCount) || this == null)
//        return;
//      this.Dispose();
//    }

//    internal unsafe void GetDesignGlyphMetrics(
//      ushort* pGlyphIndices,
//      uint glyphCount,
//      GlyphMetrics* pGlyphMetrics)
//    {
//      IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//      IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//      ushort* numPtr = pGlyphIndices;
//      int num1 = (int) glyphCount;
//      long num2 = (long) pGlyphMetrics;
//      // ISSUE: cast to a function pointer type
//      // ISSUE: function pointer call
//      int hr = __calli((__FnPtr<int (IntPtr, ushort*, uint, DWRITE_GLYPH_METRICS*, int)>) *(long*) (*(long*) idWriteFontFacePtr1 + 80L))((int) idWriteFontFacePtr2, (DWRITE_GLYPH_METRICS*) numPtr, (uint) num1, (ushort*) num2, IntPtr.Zero);
//      GC.KeepAlive((object) this._fontFace);
//      Util.ConvertHresultToException(hr);
//      GC.KeepAlive((object) this);
//    }

//    internal unsafe void GetDisplayGlyphMetrics(
//      ushort* pGlyphIndices,
//      uint glyphCount,
//      GlyphMetrics* pGlyphMetrics,
//      float emSize,
//      bool useDisplayNatural,
//      bool isSideways,
//      float pixelsPerDip)
//    {
//      IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//      IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//      double num1 = (double) emSize;
//      double num2 = (double) pixelsPerDip;
//      int num3 = useDisplayNatural ? 1 : 0;
//      ushort* numPtr = pGlyphIndices;
//      int num4 = (int) glyphCount;
//      long num5 = (long) pGlyphMetrics;
//      int num6 = isSideways ? 1 : 0;
//      // ISSUE: cast to a function pointer type
//      // ISSUE: function pointer call
//      int hr = __calli((__FnPtr<int (IntPtr, float, float, DWRITE_MATRIX*, int, ushort*, uint, DWRITE_GLYPH_METRICS*, int)>) *(long*) (*(long*) idWriteFontFacePtr1 + 136L))((int) idWriteFontFacePtr2, (DWRITE_GLYPH_METRICS*) num1, (uint) num2, (ushort*) 0L, num3, (DWRITE_MATRIX*) numPtr, (float) num4, (float) num5, (IntPtr) num6);
//      GC.KeepAlive((object) this._fontFace);
//      Util.ConvertHresultToException(hr);
//      GC.KeepAlive((object) this);
//    }

//    internal unsafe void GetArrayOfGlyphIndices(
//      uint* pCodePoints,
//      uint glyphCount,
//      ushort* pGlyphIndices)
//    {
//      fixed (uint* numPtr1 = &*pCodePoints)
//        fixed (ushort* numPtr2 = &*pGlyphIndices)
//        {
//          IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//          IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//          // ISSUE: cast to a reference type
//          // ISSUE: variable of a reference type
//          uint* local1 = (uint*) numPtr1;
//          int num = (int) glyphCount;
//          // ISSUE: cast to a reference type
//          // ISSUE: variable of a reference type
//          ushort* local2 = (ushort*) numPtr2;
//          // ISSUE: cast to a function pointer type
//          // ISSUE: function pointer call
//          int hr = __calli((__FnPtr<int (IntPtr, uint*, uint, ushort*)>) *(long*) (*(long*) idWriteFontFacePtr1 + 88L))((ushort*) idWriteFontFacePtr2, (uint) local1, (uint*) num, (IntPtr) local2);
//          GC.KeepAlive((object) this._fontFace);
//          Util.ConvertHresultToException(hr);
//          GC.KeepAlive((object) this);
//        }
//    }

//    internal unsafe bool TryGetFontTable(OpenTypeTableTag openTypeTableTag, out byte[] tableData)
//    {
//      uint length = 0;
//      int num1 = 0;
//      tableData = (byte[]) null;
//      IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//      IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//      int num2 = (int) openTypeTableTag;
//      void* voidPtr1;
//      ref void* local1 = ref voidPtr1;
//      ref uint local2 = ref length;
//      void* voidPtr2;
//      ref void* local3 = ref voidPtr2;
//      ref int local4 = ref num1;
//      // ISSUE: cast to a function pointer type
//      // ISSUE: function pointer call
//      Util.ConvertHresultToException(__calli((__FnPtr<int (IntPtr, uint, void**, uint*, void**, int*)>) *(long*) (*(long*) idWriteFontFacePtr1 + 96L))((int*) idWriteFontFacePtr2, (void**) num2, (uint*) ref local1, (void**) ref local2, (uint) ref local3, (IntPtr) ref local4));
//      if (num1 != 0)
//      {
//        tableData = new byte[(int) length];
//        uint index = 0;
//        if (0U < length)
//        {
//          do
//          {
//            tableData[(int) index] = *(byte*) ((long) index + (IntPtr) voidPtr1);
//            ++index;
//          }
//          while (index < length);
//        }
//        IDWriteFontFace* idWriteFontFacePtr3 = this._fontFace.Value;
//        IDWriteFontFace* idWriteFontFacePtr4 = idWriteFontFacePtr3;
//        void* voidPtr3 = voidPtr2;
//        // ISSUE: cast to a function pointer type
//        // ISSUE: function pointer call
//        __calli((__FnPtr<void (IntPtr, void*)>) *(long*) (*(long*) idWriteFontFacePtr3 + 104L))((void*) idWriteFontFacePtr4, (IntPtr) voidPtr3);
//      }
//      GC.KeepAlive((object) this._fontFace);
//      GC.KeepAlive((object) this);
//      return num1 != 0;
//    }

//    internal unsafe bool ReadFontEmbeddingRights(out ushort fsType)
//    {
//      uint num1 = 0;
//      int num2 = 0;
//      fsType = (ushort) 0;
//      IDWriteFontFace* idWriteFontFacePtr1 = this._fontFace.Value;
//      IDWriteFontFace* idWriteFontFacePtr2 = idWriteFontFacePtr1;
//      void* voidPtr1;
//      ref void* local1 = ref voidPtr1;
//      ref uint local2 = ref num1;
//      void* voidPtr2;
//      ref void* local3 = ref voidPtr2;
//      ref int local4 = ref num2;
//      // ISSUE: cast to a function pointer type
//      // ISSUE: function pointer call
//      Util.ConvertHresultToException(__calli((__FnPtr<int (IntPtr, uint, void**, uint*, void**, int*)>) *(long*) (*(long*) idWriteFontFacePtr1 + 96L))((int*) idWriteFontFacePtr2, (void**) 841962319, (uint*) ref local1, (void**) ref local2, (uint) ref local3, (IntPtr) ref local4));
//      bool flag = false;
//      if (num2 != 0)
//      {
//        if (num1 >= 9U)
//        {
//          byte* numPtr = (byte*) ((IntPtr) voidPtr1 + 8L);
//          fsType = (ushort) ((int) *numPtr * 256 + (int) numPtr[1L]);
//          flag = true;
//        }
//        IDWriteFontFace* idWriteFontFacePtr3 = this._fontFace.Value;
//        IDWriteFontFace* idWriteFontFacePtr4 = idWriteFontFacePtr3;
//        void* voidPtr3 = voidPtr2;
//        // ISSUE: cast to a function pointer type
//        // ISSUE: function pointer call
//        __calli((__FnPtr<void (IntPtr, void*)>) *(long*) (*(long*) idWriteFontFacePtr3 + 104L))((void*) idWriteFontFacePtr4, (IntPtr) voidPtr3);
//      }
//      GC.KeepAlive((object) this._fontFace);
//      GC.KeepAlive((object) this);
//      return flag;
//    }

//    private void \u007EFontFace()
//    {
//      NativeIUnknownWrapper\u003CMS\u003A\u003AInternal\u003A\u003AText\u003A\u003ATextInterface\u003A\u003ANative\u003A\u003AIDWriteFontFace\u003E fontFace = this._fontFace;
//      if (fontFace == null)
//        return;
//      fontFace.Dispose();
//      this._fontFace = (NativeIUnknownWrapper\u003CMS\u003A\u003AInternal\u003A\u003AText\u003A\u003ATextInterface\u003A\u003ANative\u003A\u003AIDWriteFontFace\u003E) null;
//    }

//    protected void Dispose(bool A_0)
//    {
//      if (A_0)
//      {
//        this.\u007EFontFace();
//      }
//      else
//      {
//        // ISSUE: explicit finalizer call
//        this.Finalize();
//      }
//    }

//    public virtual void Dispose()
//    {
//      this.Dispose(true);
//      GC.SuppressFinalize((object) this);
//    }
//  }
//}
