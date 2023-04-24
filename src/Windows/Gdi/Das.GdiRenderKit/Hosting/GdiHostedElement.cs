using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views;
using WinForms.Shared;

namespace Das.Gdi.Controls;

/// <summary>
///     Control to show an IView in windows forms using GDI rendering
/// </summary>
public class GdiHostedElement : HostedViewControl,
                                IViewHost<Bitmap>
{
   public GdiHostedElement(IVisualElement view,
                           IVisualBootstrapper visualBootstrapper)
      : base(view, visualBootstrapper.ThemeProvider,  visualBootstrapper)
   {
      View = view;
      _lockBmp = new Object();
   }

   public override Boolean IsLoaded => true;

   public Bitmap Asset
   {
      get => BackingBitmap!;
      set => BackingBitmap = value;
   }

   //todo: base on IView after changing element to IView type _
   // ReSharper disable once ConditionIsAlwaysTrueOrFalse
   public override Boolean IsChanged => View != null && (_isChanged //|| View.IsChanged 
                                                         || base.IsChanged);

   public Bitmap? BackingBitmap
   {
      get => _backingBitmap;
      set
      {
         lock (_lockBmp)
         {
            //Debug.WriteLine("updating hosted element bmp " + value?.Width + " " + 
            //                 (_backingBitmap != value));

            if (_backingBitmap != value)
            {
               _backingBitmap?.Dispose();
               _backingBitmap = value;
            }
         }

         Invalidate();
      }
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      lock (_lockBmp)
      {
         if (BackingBitmap == null)
            return;

         _isChanged = false;

         AvailableSize.Width = Width;
         AvailableSize.Height = Height;

         e.Graphics.DrawImage(BackingBitmap, Point.Empty);
      }
   }

   private readonly Object _lockBmp;

   private Bitmap? _backingBitmap;
   private Boolean _isChanged;
}