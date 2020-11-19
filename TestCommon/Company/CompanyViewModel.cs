using System;
using System.Diagnostics;
using Das.ViewModels;
using Das.Views.Charting;
using Das.Views.Extended;
using Das.Views.Extended.Runtime;

namespace TestCommon.Company
{
    public class CompanyViewModel : BaseViewModel, 
                                    ICompanyViewModel
    {
        public CompanyViewModel(String name, IScene scene)
        {
            Employees = new ObservableRangeCollection<EmployeeViewModel>();
            _scene = scene;
            Name = name;
            Camera = new WireframeCamera(new Vector3(0, 0, 10.0f), Vector3.Zero, Vector3.Zero, scene);
            SalesReport = new SalesReportPie();

            _running = Stopwatch.StartNew();
        }


        private EmployeeViewModel _selectedEmployee;

        public EmployeeViewModel SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetValue(ref _selectedEmployee, value);
        }

        private readonly IScene _scene;
        public ICompanyViewModel Self => this;
        public ObservableRangeCollection<EmployeeViewModel> Employees { get; set; }
        public IPieData<String, Double> SalesReport { get; }

        public String Name { get; }

        private readonly Stopwatch _running;

        //public override void AcceptChanges()
        //{
            
        //}

        //private Boolean _isChanged;

        //public override bool IsChanged => _isChanged;
        public ICamera Camera { get; }
        public void Update()
        {
            if (_running.ElapsedMilliseconds < 25)
                return;

            foreach (var element in _scene.VisualElements)
            {
                //element.Rotate(rot, rot, 0);
                element.Rotate(0.025f, 0.025f, 0);
            }

            _running.Restart();

            //_isChanged = true;
        }
    }
}
