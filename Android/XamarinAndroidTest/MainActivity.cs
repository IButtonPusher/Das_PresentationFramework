using System;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Das.Views;
using Das.Views.Panels;
using Das.Xamarin.Android;

namespace XamarinAndroidTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : DasMainActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;

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

        protected override Task<IView> GetMainViewAsync(IRenderKit renderKit, 
                                                        IUiProvider uiProvider)
        {
            var vm = new TestVm();
            vm.Name = "hello world";
            IView view = new TestView();
            view.SetBoundValue(vm);
            return Task.FromResult(view);
        }

        protected override Func<Task<Boolean>> BackButtonCommand => () => Task.FromResult(false);

        public override void OnRequestPermissionsResult(Int32 requestCode,
                                                        String[] permissions, 
                                                        [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public Boolean OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    textMessage.SetText(Resource.String.title_home);
                    return true;
                case Resource.Id.navigation_dashboard:
                    textMessage.SetText(Resource.String.title_dashboard);
                    return true;
                case Resource.Id.navigation_notifications:
                    textMessage.SetText(Resource.String.title_notifications);
                    return true;
            }
            return false;
        }
    }
}

