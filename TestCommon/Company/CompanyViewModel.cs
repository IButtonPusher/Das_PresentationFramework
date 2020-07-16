using System;
using System.Collections.Generic;
using Das.ViewModels;
using Das.Views;
using Das.Views.Charting;
using Das.Views.Extended;
using Das.Views.Extended.Core;
using Das.Views.Extended.Runtime;

namespace TestCommon.Company
{
    public class CompanyViewModel : BaseViewModel, ICompanyViewModel
    {
        public CompanyViewModel(IScene scene, ISingleThreadedInvoker staInvoker) 
            : base(staInvoker)
        {
            Employees  = new List<EmployeeViewModel>();
            _scene = scene;
            Camera = new Camera(new Vector3(0, 0, 10.0f), Vector3.Zero, Vector3.Zero, scene);
            SalesReport = new SalesReportPie();
        }

        private readonly IScene _scene;
        public ICompanyViewModel Self => this;
        public List<EmployeeViewModel> Employees { get; set; }
        public IPieData<String, double> SalesReport { get; }

        //public override void AcceptChanges()
        //{
            
        //}

        private Boolean _isChanged;

        //public override bool IsChanged => _isChanged;
        public ICamera Camera { get; }
        public void Update()
        {
            foreach (var element in _scene.VisualElements)
                element.Rotate(0.025f, 0.025f, 0);

            _isChanged = true;
        }
    }
}
