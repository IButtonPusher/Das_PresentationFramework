using System;
using Das.ViewModels;
using Das.Views.Charting;
using Das.Views.Extended;

namespace TestCommon.Company
{
    public interface ICompanyViewModel : ISceneViewModel
    {
        ICompanyViewModel Self { get; }

        ObservableRangeCollection<EmployeeViewModel> Employees { get; set; }

        EmployeeViewModel SelectedEmployee { get; set; }

        IPieData<String, Double> SalesReport { get; }

        String Name { get; }
    }
}
