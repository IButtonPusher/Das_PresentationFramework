using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XamarinAndroidNativeTest
{
    public static class BadLog
    {
        private static Stopwatch _running = Stopwatch.StartNew();

        public static void WriteLine(String str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {}

            System.Diagnostics.Debug.WriteLine("[OKYN @ " + _running.ElapsedMilliseconds + 
                                               " ] " + str);

        }
    }
}