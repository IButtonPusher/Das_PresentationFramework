using System;
using System.Collections.ObjectModel;
using Das.ViewModels;



namespace TestCommon.Company
{
    public interface ICompanyViewModel : //ISceneViewModel,
                                         IEquatable<ICompanyViewModel>
    {
        ICompanyViewModel Self { get; }

        ObservableCollection<EmployeeViewModel> Employees { get; set; }

        EmployeeViewModel SelectedEmployee { get; set; }

        //IPieData<String, Double> SalesReport { get; }

        String Name { get; }
    }
}
