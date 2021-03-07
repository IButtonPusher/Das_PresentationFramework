using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Xamarin.Essentials;

namespace XamarinAndroidNativeTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public override Boolean OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override Boolean OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_settings)
                return true;

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(Int32 requestCode,
                                                        String[] permissions,
                                                        [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            //SetContentView(Resource.Layout.activity_main);
            SetContentView(new MainView(Application.Context, Resources.DisplayMetrics));

            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            //var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;
        }

        private void FabOnClick(Object sender,
                                EventArgs eventArgs)
        {
            var view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                    .SetAction("Action", (View.IOnClickListener) null).Show();
        }
    }
}
