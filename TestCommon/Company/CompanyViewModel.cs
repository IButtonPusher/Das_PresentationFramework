using System;
using System.Collections.Generic;
using Das.Views.Charting;
using Das.Views.Extended;
using Das.Views.Extended.Core;
using Das.Views.Extended.Runtime;

namespace TestCommon.Company
{
    public class CompanyViewModel : ICompanyViewModel
    {
        public CompanyViewModel(IScene scene)
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

        public void AcceptChanges()
        {
            
        }

        public bool IsChanged { get; private set; }
        public ICamera Camera { get; }
        public void Update()
        {
            foreach (var element in _scene.VisualElements)
                element.Rotate(0.025f, 0.025f, 0);

            IsChanged = true;
        }
    }
}
