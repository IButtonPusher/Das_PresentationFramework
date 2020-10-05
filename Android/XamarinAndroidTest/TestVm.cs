using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Das.ViewModels;

namespace XamarinAndroidTest
{
    public class TestVm : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private String _name;

        public String Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private static void SetValue(ref String name, String value)
        {
            name = value;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}