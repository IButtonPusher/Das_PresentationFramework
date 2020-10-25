using System;
using System.Collections.Generic;
using Das.Views.Charting;
using Das.Views.Extended;

namespace TestCommon.Company
{
    public interface ICompanyViewModel : ISceneViewModel
    {
        ICompanyViewModel Self { get; }

        List<EmployeeViewModel> Employees { get; set; }

        IPieData<String, Double> SalesReport { get; }

        String Name { get; }
    }
}
