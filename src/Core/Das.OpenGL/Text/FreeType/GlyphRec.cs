﻿using Das.Views.Core.Geometry;
using System;
using System.Runtime.InteropServices;
// ReSharper disable All
#pragma warning disable 8618

namespace Das.OpenGL.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
public class GlyphRec
{
   public IntPtr library;
   public IntPtr clazz;
   public Int32 format;
   public Vector advance;
}