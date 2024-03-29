﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Das.Views
{
   public enum LogLevel
   {
      Lavel0 = 0,
      Level1 = 1,
      Level2 = 2,
      Level3 = 3,
      Level4 = 4
   }

   public static class UILogger
   {
      public static void Log(String msg,
                             LogLevel level)
      {
         if (level < LogLevel)
            return;

         Debug.WriteLine("[UI " + Thread.CurrentThread.ManagedThreadId + "][" +
                         _sw.ElapsedMilliseconds + "] " + msg);
      }

      public static LogLevel LogLevel { get; set; }

      private static readonly Stopwatch _sw = Stopwatch.StartNew();
   }
}
