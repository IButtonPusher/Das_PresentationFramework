using System;
using System.Collections.Generic;
using Das.Views.Controls;
using Das.Views.Core.Enums;
using Das.Views.DataBinding;
using Das.Views.Panels;

namespace TestCommon.Company
{
    public class CompanyView : View<ICompanyViewModel>
    {
        public CompanyView()
        {
            Content = new StackPanel<ICompanyViewModel>
            {
                Orientation = Orientations.Horizontal,
                Children =
                {
                    new RepeaterPanel<EmployeeViewModel>
                    {
                        Binding = DynamicBinding.Get<IEnumerable<EmployeeViewModel>>("Employees"),
                        Content = new StackPanel<IEmployee>
                        {
                            Orientation = Orientations.Horizontal,
                            Children =
                            {
                                new Label
                                {
                          //          Binding = 
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
