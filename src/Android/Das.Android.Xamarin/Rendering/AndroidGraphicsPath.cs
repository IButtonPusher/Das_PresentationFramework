using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Rendering;
using Das.Views.Transforms;

namespace Das.Xamarin.Android.Rendering;

public class AndroidGraphicsPath : GraphicsPathBase
{
   //private readonly IVisualContext _visualContext;

   public AndroidGraphicsPath()//IVisualContext visualContext)
   {
      // the visual context is too broad in scope for whatever this was intended for
      //_visualContext = visualContext;
      Path = new Path();
            
      _pointTypes = new List<PathPointType>();
      _points = new List<IPoint2F>();
   }

   public override void Dispose()
   {
            
   }

   public override void LineTo<TPoint>(TPoint p1) 
   {
      Path.LineTo(R4(p1.X), R4(p1.Y));
      //System.Diagnostics.Debug.WriteLine("path.LineTo(" + R4(p1.X) + "f, " + R4(p1.Y) + "f);");
      _pointTypes.Add(PathPointType.Line);
   }

   public override void AddLine<TPoint>(TPoint p1,
                                        TPoint p2)
   {
      if (TryAddConnectingPoint(p1.X, p1.Y, PathPointType.Line))
      {
         if (_points.Count > 1)
         {
            Path.Close();
         }

         Path.MoveTo(p1.X, p1.Y);
      }

      Path.LineTo(p2.X, p2.Y);

      AddPoint(p2, PathPointType.Line);
   }

       

   public override void AddArc<TRectangle>(TRectangle arc, 
                                           Single startAngle, 
                                           Single endAngle)
   {
      Path.AddArc(R4(arc.Left), R4(arc.Top), R4(arc.Right), R4(arc.Bottom),
         startAngle, endAngle);

      _pointTypes.AddRange(new[]
      {
         PathPointType.Bezier3, PathPointType.Bezier3,
         PathPointType.Bezier3
      });
   }

   public override void AddBezier(Single x1,
                                  Single y1,
                                  Single x2,
                                  Single y2,
                                  Single x3,
                                  Single y3,
                                  Single x4,
                                  Single y4)
   {
      if (TryAddConnectingPoint(x1, y1, PathPointType.Bezier3))
      {
         if (_points.Count > 1)
         {
            //System.Diagnostics.Debug.WriteLine("path.Close();");
            Path.Close();
         }

         //System.Diagnostics.Debug.WriteLine("path.MoveTo(" + R4(x1) + "f, " + R4(y1) + "f);");
         Path.MoveTo(x1, y1);
      }

      //System.Diagnostics.Debug.WriteLine("path.CubicTo(" + x2 + "f," + y2 + "f," + x3 + "f," + 
      //y3 + "f, " + x4 + "f, " + y4 + "f);");
      Path.CubicTo(x2, y2, x3, y3, x4, y4);

            
      AddPoint(x2,y2, PathPointType.Bezier3);
      AddPoint(x3,y3, PathPointType.Bezier3);
      AddPoint(x4,y4, PathPointType.Bezier3);
   }

   public override void AddBezier(IPoint2F p1,
                                  IPoint2F p2,
                                  IPoint2F p3,
                                  IPoint2F p4)
   {
      if (TryAddConnectingPoint(p1.X, p1.Y, PathPointType.Bezier3))
      {
         if (_points.Count > 1)
         {
            //System.Diagnostics.Debug.WriteLine("path.Close();");
            Path.Close();
         }

         //System.Diagnostics.Debug.WriteLine("path.MoveTo(" + R4(p1.X) + "f, " + R4(p1.Y) + "f);");
         Path.MoveTo(p1.X, p1.Y);
      }

      //System.Diagnostics.Debug.WriteLine("path.CubicTo(" + p2.X + "f," + p2.Y + "f," + p3.X + "f," + 
      //p3.Y + "f, " + p4.X + "f, " + p4.Y + "f);");
      Path.CubicTo(p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y);

            
      AddPoint(p2, PathPointType.Bezier3);
      AddPoint(p3, PathPointType.Bezier3);
      AddPoint(p4, PathPointType.Bezier3);
   }

   public override void StartFigure()
   {
      //System.Diagnostics.Debug.WriteLine("path.Reset();");
      _isPendingStart = true;
   }

   public override void CloseFigure()
   {
      Path.Close();
      //System.Diagnostics.Debug.WriteLine("path.Close();");
      _pointTypes.Add(PathPointType.CloseSubpath);
   }

   public override void Transform(TransformationMatrix matrix)
   {
      if (matrix.IsIdentity)
         return;

      var androidMatrix = new Matrix();
      if (matrix.HasSkew)
         androidMatrix.SetSkew(R4(matrix.SkewX), R4(matrix.SkewY));

      if (matrix.HasScale)
         androidMatrix.SetScale(R4(matrix.ScaleX), R4(matrix.ScaleY));

      if (matrix.HasOffset)
         androidMatrix.SetTranslate(R4(matrix.OffsetX), R4(matrix.OffsetY));

      Path.Transform(androidMatrix);
   }

   public override ValueSize Size
   {
      get
      {
         var pm = new PathMeasure(Path, false);
         return new ValueSize(pm.Length, pm.Length);
      }
   }

   public override IPathData PathData => new PathData(_points, _pointTypes.Cast<Byte>());

   public override T Unwrap<T>()
   {
      if (Path is T good)
         return good;

      throw new InvalidCastException();
   }

      

   public override IImage ToImage(Int32 width,
                                  Int32 height,
                                  IColor? stroke,
                                  IBrush? fill)
   { 
      var androidPath = Path;


      androidPath.SetFillType(Path.FillType.EvenOdd!);

      var bmp = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!)
                ?? throw new InvalidOperationException();

      var canvas = new Canvas(bmp);
            
      using (var paint = new Paint())
      {
         paint.AntiAlias = true;
         if (fill is { } set)
         {
            paint.SetBackgroundColor(set);

            canvas.DrawPath(androidPath, paint);
         }

         if (stroke is { } different)
         {
            paint.SetStrokeColor(different);
            canvas.DrawPath(androidPath, paint);
         }

      }

      return new AndroidBitmap(bmp, null);
   }


   private Boolean TryAddConnectingPoint(Single x,
                                         Single y,
                                         PathPointType pointType)
   {
      if (_points.Count > 0)
      {
         var lastPoint = _points[_points.Count - 1];
         if (lastPoint.X.AreEqualEnough(x) &&
             lastPoint.Y.AreEqualEnough(y))
            return false;
      }

      if (_points.Count == 0 || _isPendingStart)
      {
         _pointTypes.Add(PathPointType.Start);
      }
      else
         _pointTypes.Add(pointType);

      _points.Add(new ValuePoint2F(x,y));
      if (_isPendingStart)
      {
         _isPendingStart = false;
      }
      return true;
   }

   private void AddPoint(IPoint2F p,
                         PathPointType pointType)
   {
      _points.Add(p);
      _pointTypes.Add(pointType);
   }

   private void AddPoint(Single x,
                         Single y,
                         PathPointType pointType)
   {
      _points.Add(new ValuePoint2F(x,y));
      _pointTypes.Add(pointType);
   }

       

   public Path Path { get; }

   private readonly List<PathPointType> _pointTypes;
   private readonly List<IPoint2F> _points;
   private Boolean _isPendingStart;
}