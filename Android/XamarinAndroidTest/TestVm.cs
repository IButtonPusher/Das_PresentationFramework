using System;
using Das.ViewModels;

namespace XamarinAndroidTest
{
    public class TestVm : BaseViewModel
    {
        private String _name;

        public String Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

       
    }
}