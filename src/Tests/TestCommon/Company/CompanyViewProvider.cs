using System;
using System.Collections.Generic;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Styles;

namespace TestCommon.Company
{
    public class CompanyViewProvider
    {
        public IView<CompanyViewModel> GetCompanyView(IStyleContext styleContext)
        {
            var companyVm = typeof(CompanyViewModel);
            var employeeVm = typeof(EmployeeViewModel);

            var companyView = new View<CompanyViewModel>(styleContext);

            var employeesBinding = new DeferredPropertyBinding<IEnumerable<EmployeeViewModel>>
                (companyVm, nameof(CompanyViewModel.Employees));
            var employeesView = new RepeaterPanel<EmployeeViewModel>(employeesBinding);

            var employeeView = new StackPanel<EmployeeViewModel>
                { Orientation = Orientations.Horizontal };

            var lblFirst = new Label(new DeferredPropertyBinding<string>(employeeVm, 
                nameof(EmployeeViewModel.FirstName)));

            var lblLast = new Label(new DeferredPropertyBinding<string>(employeeVm,
                nameof(EmployeeViewModel.LastName)));

            employeeView.AddChild(lblFirst);
            employeeView.AddChild(lblLast);

            var labelStyle0 = new StyleForLabel(lblFirst, 12, FontStyle.Regular, Color.White);
            styleContext.RegisterStyle(labelStyle0);

            var labelStyle = new StyleForLabel(lblLast, 18 ,FontStyle.Bold, Color.Orange);
            styleContext.RegisterStyle(labelStyle);

            employeesView.Content = employeeView;
            companyView.Content = employeesView;

            return companyView;
        }
    }
}
