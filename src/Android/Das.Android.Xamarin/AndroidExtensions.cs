using System;
using System.Threading.Tasks;
using Android.Graphics;
using Das.Views.Core.Drawing;

namespace Das.Xamarin.Android
{
    public static class AndroidExtensions
    {
        public static void SetBackgroundColor(this Paint paint,
                                              IBrush brush)
        {
            paint.SetStyle(Paint.Style.Fill);

            switch (brush)
            {
                case SolidColorBrush scb:
                    paint.SetARGB(scb.Color.A, scb.Color.R, scb.Color.G, scb.Color.B);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetStrokeColor(this Paint paint,
                                              IColor different)
        {
            paint.SetStyle(Paint.Style.Stroke);
            paint.SetARGB(different.A, different.R, different.G, different.B);
        }
    }
}
