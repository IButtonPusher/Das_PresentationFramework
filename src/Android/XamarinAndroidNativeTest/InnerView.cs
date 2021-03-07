using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.Text;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace XamarinAndroidNativeTest
{
    public class InnerView : TextView
    {
        public InnerView(Context? context) : base(context)
        {
            var colors = new Int32[] {Color.Purple, Color.Pink, Color.Green};
            Background = new GradientDrawable(GradientDrawable.Orientation.TopBottom,
                colors); 
            LayoutParameters = new ActionBar.LayoutParams(3000, 5000);

            SetSomeText("abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz1234567890");

            
        }

        //public override Boolean OnGenericMotionEvent(MotionEvent? e)
        //{
        //    _testInput.OnGenericMotionEvent(e);
        //    return base.OnGenericMotionEvent(e);
        //}

        //public override Boolean OnInterceptTouchEvent(MotionEvent? ev)
        //{
        //    _testInput.OnTouchEvent(ev);
        //    return false;
        //}

        private void SetSomeText(String str)
        {
            var sb = new StringBuilder();
            for (var c = 0; c < str.Length; c++)
            {
                sb.Append(str[c]);
                sb.AppendLine();
            }

            Text = sb.ToString();
        }

        //private TestInputListener _testInput;
    }
}