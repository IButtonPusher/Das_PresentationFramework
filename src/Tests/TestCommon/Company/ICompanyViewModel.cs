using System;
using Das.ViewModels;



namespace TestCommon.Company
{
    public interface ICompanyViewModel : //ISceneViewModel,
                                         IEquatable<ICompanyViewModel>
    {
        ICompanyViewModel Self { get; }

        ObservableRangeCollection<EmployeeViewModel> Employees { get; set; }

        EmployeeViewModel SelectedEmployee { get; set; }

        //IPieData<String, Double> SalesReport { get; }

        String Name { get; }
    }
}
