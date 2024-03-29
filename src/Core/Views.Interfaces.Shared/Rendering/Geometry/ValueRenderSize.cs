﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Core.Geometry;

public readonly struct ValueRenderSize : IRenderSize
{
   [DebuggerStepThrough]
   [DebuggerHidden]
   public ValueRenderSize(Double width,
                          Double height,
                          IPoint2D offset)
   {
      Height = height;
      Width = width;
      Offset = offset;

      HasInfiniteDimension = Double.IsInfinity(Width) || Double.IsInfinity(Height);
   }

   [DebuggerStepThrough]
   [DebuggerHidden]
   public ValueRenderSize(ISize size,
                          IPoint2D offset,
                          ISize padDown)
   {
      if (padDown == null || padDown.IsEmpty)
      {
         Height = size.Height;
         Width = size.Width;
      }
      else
      {
         Height = size.Height - padDown.Height;
         Width = size.Width - padDown.Width;
      }

      Offset = offset;
      HasInfiniteDimension = Double.IsInfinity(Width) || Double.IsInfinity(Height);
   }

   public ValueRenderSize(Double width,
                          Double height)
      : this(width, height, ValuePoint2D.Empty)
   {
   }

   public ValueRenderSize(ISize size)
      : this(size.Width, size.Height, ValuePoint2D.Empty)
   {
   }

   public Boolean Equals(ISize other)
   {
      return GeometryHelper.AreSizesEqual(this, other);
   }

       

   public ValueRenderRectangle ToFullRectangle()
   {
      return new ValueRenderRectangle(ValuePoint2D.Empty, this, Offset);
   }

   public ValueSize ToValueSize()
   {
      return GeometryHelper.ToValueSize(this);
   }

   //ISize IDeepCopyable<ISize>.DeepCopy()
   //{
   //    return DeepCopy();
   //}

   //public IRenderSize DeepCopy()
   //{
   //    return new ValueRenderSize(Width, Height, Offset);
   //}

   public Double Height { get; }

   public Boolean IsEmpty => Width.IsZero() && Height.IsZero();

   public Double Width { get; }

   public Boolean HasInfiniteDimension { get; }

   //ISize ISize.Reduce(Thickness padding)
   //{
   //    return Reduce(padding);
   //}

   public IRenderSize MinusVertical(ISize subtract)
   {
      return GeometryHelper.MinusVertical(this, subtract);
   }

   public ISize PlusVertical(ISize adding)
   {
      return GeometryHelper.PlusVertical(this, adding);
   }

   public IRenderSize Minus(ISize subtract)
   {
      return GeometryHelper.Minus(this, subtract);
   }

   public IPoint2D Offset { get; }

   public Boolean Equals(IRenderSize other)
   {
      return false;
   }

   public override String ToString()
   {
      return "Width: " + Width + " Height: " + Height;
   }


   public static readonly ValueRenderSize Empty = new ValueRenderSize(0, 0);

   public static readonly ValueRenderSize Infinite = new ValueRenderSize(Double.PositiveInfinity,
      Double.PositiveInfinity);
}