using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Das.Views;
using Das.Views.Panels;
using Das.Xamarin.Android;
using Xamarin.Essentials;

namespace XamarinAndroidTest
{
   [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
   public class MainActivity : DasMainActivity, BottomNavigationView.IOnNavigationItemSelectedListener
   {
      public Boolean OnNavigationItemSelected(IMenuItem item)
      {
         //    switch (item.ItemId)
         //    {
         //        case Resource.Id.navigation_home:
         //            textMessage.SetText(Resource.String.title_home);
         //            return true;
         //        case Resource.Id.navigation_dashboard:
         //            textMessage.SetText(Resource.String.title_dashboard);
         //            return true;
         //        case Resource.Id.navigation_notifications:
         //            textMessage.SetText(Resource.String.title_notifications);
         //            return true;
         //    }
         return false;
      }

      public override void OnRequestPermissionsResult(Int32 requestCode,
                                                      String[] permissions,
                                                      [GeneratedEnum] Permission[] grantResults)
      {
         Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }
      //TextView textMessage;

      //protected override void OnCreate(Bundle savedInstanceState)
      //{
      //    base.OnCreate(savedInstanceState);
      //    Xamarin.Essentials.Platform.Init(this, savedInstanceState);

      //    var vm = new TestVm();
      //    vm.Name = "hello world";
      //    var bob = new TestView();
      //    bob.SetBoundValue(vm);
      //    var v = new AndroidView(bob, this, WindowManager ?? throw new NullReferenceException());

      //    //var wv = new WebView(this);

      //    SetContentView(v);

      //    //SetContentView(Resource.Layout.activity_main);

      //    textMessage = FindViewById<TextView>(Resource.Id.message);
      //    //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
      //    //navigation.SetOnNavigationItemSelectedListener(this);
      //}

      protected override Task<IVisualElement> GetMainViewAsync(IRenderKit renderKit,
                                                               IUiProvider uiProvider)
      {
         var vm = new TestVm();
         vm.Name = "hello world";
         IView view = new TestView(renderKit.VisualBootstrapper, renderKit.ImageProvider);
         view.DataContext = vm;

         Task.Factory.StartNew(() => DoDaBlaBla(uiProvider), TaskCreationOptions.LongRunning);

         return Task.FromResult<IVisualElement>(view);
      }

      private static async Task DoDaBlaBla(IUiProvider uiProvider)
      {
         /*
             Line 1525: [0:] [4] narf 1
            Line 1553: [0:] [9] narf 2
            Line 1554: [0:] [9] narf 5
            Line 1555: [0:] [1] narf 3
            Line 1556: [0:] [1] narf 4
            Line 1557: [0:] [8] narf 6       
          */

         Narf("narf 1");

         await Task.Delay(1000).ConfigureAwait(false);

         Narf("narf 2");

         var soFarAway = uiProvider.InvokeAsync(async () =>
         {
            await Task.Yield();

            Narf("narf 3");

            await Task.Delay(1000);

            Narf("narf 4");
         });

         Narf("narf 5");

         await soFarAway;

         Narf("narf 6");

         await Task.Run(() =>
         {
            uiProvider.Invoke(() =>
            {
               Narf("narf 7");
            });
            Narf("narf 8");
         }).ConfigureAwait(false);
      }

      private static void Narf(String str)
      {
         Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId + "] " + str);
      }

      protected override Func<Task<Boolean>> BackButtonCommand => () => Task.FromResult(false);
   }
}
