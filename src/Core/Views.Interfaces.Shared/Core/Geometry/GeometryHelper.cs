using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Rendering;

namespace Das.Views.Core.Geometry
{
   public static class GeometryHelper
   {
      public static Boolean AreSizesEqual<TSize>(TSize left,
                                                 ISize? other)
         where TSize : ISize
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(left, other)) return true;

         return left.Width.AreEqualEnough(other.Width) &&
                left.Height.AreEqualEnough(other.Height);
      }

      public static Boolean IsRectangleContains<TRect>(TRect rectangle,
                                                       IPoint2D point)
         where TRect : IRectangle
      {
         return point.X >= rectangle.Left
                && point.X <= rectangle.Right
                && point.Y >= rectangle.Top
                && point.Y <= rectangle.Bottom;
      }

      public static Boolean IsRectangleContains<TRect>(TRect rectangle,
                                                       Int32 x,
                                                       Int32 y)
         where TRect : IRectangle
      {
         return x >= rectangle.Left
                && x <= rectangle.Right
                && y >= rectangle.Top
                && y <= rectangle.Bottom;
      }

      public static Boolean IsRectanglesIntersect<TRect>(TRect rectangle,
                                                         IRectangle other)
         where TRect : IRectangle
      {
         return IsRectangleContains(rectangle, other.TopLeft) ||
                IsRectangleContains(rectangle, other.TopRight) ||
                IsRectangleContains(rectangle, other.BottomLeft) ||
                IsRectangleContains(rectangle, other.BottomRight);
      }

      public static Boolean IsRectangleContains<TRect>(TRect rectangle,
                                                       Double x,
                                                       Double y)
         where TRect : IRectangle
      {
         return x >= rectangle.Left
                && x <= rectangle.Right
                && y >= rectangle.Top
                && y <= rectangle.Bottom;
      }

      public static ISize Minus(ISize original,
                                ISize takeAway)
      {
         return new ValueSize(original.Width - takeAway.Width,
            original.Height - takeAway.Height);
      }

      public static ISize PlusVertical(ISize original,
                                       ISize takeAway)
      {
         return new ValueSize(Math.Max(original.Width, takeAway.Width),
            original.Height + takeAway.Height);
      }

      public static ValueSize PlusVertical(ValueSize original,
                                           ISize takeAway)
      {
         return new ValueSize(Math.Max(original.Width, takeAway.Width),
            original.Height + takeAway.Height);
      }

      public static IRenderSize PlusRenderVertical(IRenderSize original,
                                                   ISize takeAway)
      {
         return new ValueRenderSize(Math.Max(original.Width, takeAway.Width),
            original.Height + takeAway.Height, original.Offset);
      }

      public static IRenderSize MinusVertical(IRenderSize original,
                                              ISize takeAway)
      {
         return new ValueRenderSize(Math.Max(original.Width, takeAway.Width),
            original.Height - takeAway.Height, original.Offset);
      }

      public static IRenderSize Minus(IRenderSize original,
                                      ISize takeAway)
      {
         return new ValueRenderSize(original.Width - takeAway.Width,
            original.Height - takeAway.Height, original.Offset);
      }


      public static ValueSize ToValueSize(IRenderSize rect)
      {
         return new ValueSize(rect.Width, rect.Height);
      }


      public static Boolean DoContains<T>(this IMinMax<T> me,
                                          IMinMax<T> item)
         where T : IComparable, IConvertible
      {
         return me.Min.CompareTo(item.Min) <= 0 && me.Max.CompareTo(item.Max) >= 0;
      }

      /// <summary>
      ///    Assumes me is greater than item so
      ///    me.min is less than item.min or
      ///    me.max is greater than item.max
      ///    but not both
      /// </summary>
      public static IMinMax<T> DoMinus<T>(this IMinMax<T> me,
                                          IMinMax<T> item,
                                          Func<T, T, IMinMax<T>> ctor)
         where T : IComparable, IConvertible
      {
         if (me.Min.CompareTo(item.Min) == 0)
         {
            // min values are equal so take me.max - item.max

            if (me.Max.CompareTo(item.Max) > 0)
               return me.Empty;

            var newman = IsInteger<T>() ? item.Max.AddInteger(1) : item.Max;

            return ctor(newman, me.Max);
         }

         if (me.Max.CompareTo(item.Max) > 0)
         {
            //me == item so subtraction is empty 
            return me.Empty;
         }

         // max values are equal so go from me.min to item.min
         var newsmax = IsInteger<T>() ? item.Min.AddInteger(-1) : item.Min;

         return ctor(me.Min, newsmax);
      }

      public static T AddInteger<T>(this T value,
                                    Int32 add)
         where T : IConvertible
      {
         var res = value.ToInt32(NumberFormatInfo.InvariantInfo) + add;
         return (T) Convert.ChangeType(res, typeof(T));
      }

      public static Boolean IsInteger<T>()
      {
         if (!typeof(T).IsPrimitive)
            return false;

         switch (Type.GetTypeCode(typeof(T)))
         {
            case TypeCode.Int32:
            case TypeCode.Int16:
            case TypeCode.Int64:
            case TypeCode.Single:
               return true;

            default:
               return false;
         }
      }

      public static Boolean DoOverlaps<T>(this IMinMax<T> me,
                                          IMinMax<T> mm)
         where T : IComparable, IConvertible
      {
         switch (me.Min.CompareTo(mm.Min))
         {
            case 0:
               return true;

            case 1 when me.Max.CompareTo(mm.Max) < 0:
               return true;

            case -1 when me.Max.CompareTo(mm.Max) > 0:
               return true;

            default:
               return false;
         }
      }

      public static Boolean DoContains<T>(this IMinMax<T> me,
                                          T item)
         where T : IComparable, IConvertible
      {
         return me.Min.CompareTo(item) <= 0 && me.Max.CompareTo(item) >= 0;
      }


      public static ValueIntRectangle GetUnion<TRoundedRectangle>(TRoundedRectangle a,
                                                                  IRoundedRectangle b)
         where TRoundedRectangle : IRoundedRectangle
      {
         var x1 = Math.Min(a.X, b.X);
         var x2 = Math.Max(a.X + a.Width, b.X + b.Width);
         var y1 = Math.Min(a.Y, b.Y);
         var y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
         return new ValueIntRectangle(x1, y1, x2 - x1, y2 - y1);
      }

      public static ValueIntRectangle GetUnion<TRoundedRectangle>(TRoundedRectangle me,
                                                                  IEnumerable<IRoundedRectangle> others)
         where TRoundedRectangle : IRoundedRectangle
      {
         //var me = this as IRoundedRectangle;

         var x1 = me.X;
         var x2 = me.X + me.Width;
         var y1 = me.Y;
         var y2 = me.Y + me.Height;

         foreach (var b in others)
         {
            x1 = Math.Min(x1, b.X);
            x2 = Math.Max(x2, b.X + b.Width);
            y1 = Math.Min(y1, b.Y);
            y2 = Math.Max(y2, b.Y + b.Height);
         }

         return new ValueIntRectangle(x1, y1, x2 - x1, y2 - y1);
      }

      public static Boolean AreRectsEqual<TRect>(TRect left,
                                                 IRectangle right)
         where TRect : IRectangle
      {
         if (ReferenceEquals(left, right))
            return true;

         return left.Width.AreEqualEnough(right.Width) &&
                left.Height.AreEqualEnough(right.Height) &&
                left.Left.AreEqualEnough(right.Left) &&
                left.Top.AreEqualEnough(right.Top);
      }

      public static Boolean AreRectsEqual<TRect1, TRect2>(TRect1 left,
                                                          TRect2 right)
         where TRect1 : IRectangle
         where TRect2 : IRectangle
      {
         if (ReferenceEquals(left, right))
            return true;

         return left.Width.AreEqualEnough(right.Width) &&
                left.Height.AreEqualEnough(right.Height) &&
                left.Left.AreEqualEnough(right.Left) &&
                left.Top.AreEqualEnough(right.Top);
      }

      public static Boolean ArePoints2DEqual<TPoint>(TPoint left,
                                                     IPoint2D right)
         where TPoint : IPoint2D
      {
         return left.X.AreEqualEnough(right.X) && left.Y.AreEqualEnough(right.Y);
      }

      public static Boolean AreRenderRectsEquals<TRect>(TRect left,
                                                        IRenderRectangle right)
         where TRect : IRenderRectangle
      {
         return AreRectsEqual(left, right) && left.Size.Offset.Equals(right.Size.Offset);
      }

      public static Int32 BuildRectHash<TRect>(TRect me)
         where TRect : IRectangle
      {
         var x = Convert.ToInt32(me.X);
         var y = Convert.ToInt32(me.Y);
         var w = Double.IsNaN(me.Width) ? 0 : Convert.ToInt32(me.Width);
         var h = Double.IsNaN(me.Height) ? 0 : Convert.ToInt32(me.Height);

         return x ^ ((y << 13) | (Int32) ((UInt32) y >> 19)) ^ ((w << 26) | (Int32) ((UInt32) w >> 6)) ^
                ((h << 7) | (Int32) ((UInt32) h >> 25));
      }

      public static Double CenterX(ISize outer,
                                   ISize inner)
      {
         if (inner.Width > outer.Width)
            return 0;

         return (outer.Width - inner.Width) / 2;
      }

      public static Double CenterY(ISize outer,
                                   ISize inner)
      {
         if (inner.Height > outer.Height)
            return 0;

         return (outer.Height - inner.Height) / 2;
      }

      public static IPoint2D Offset(IPoint2D point,
                                    Double pct)
      {
         return new ValuePoint2D(point.X - point.X * pct,
            point.Y - point.Y * pct);
      }

      public static ISize Divide(ISize size,
                                 Double pct)
      {
         return new ValueSize(size.Width - size.Width * pct,
            size.Height - size.Height * pct);
      }
   }
}
