using System.Collections.Generic;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Extended.Runtime;
using TestCommon.Company;

namespace TestCommon
{
    public class TestCompanyVm : CompanyViewModel
    {
        public TestCompanyVm(ISingleThreadedInvoker staInvoker) 
            : base(MakeAScene(), staInvoker)
        {
            var larry = new EmployeeViewModel { FirstName = "larry", LastName = "larry" };
            var bob = new EmployeeViewModel { FirstName = "bob", LastName = "ooo" };
            var john = new EmployeeViewModel { FirstName = "john", LastName = "doe" };
            var bobJr = new EmployeeViewModel { FirstName = "bob", LastName = "jones" };

            Employees.AddRange(new List<EmployeeViewModel>{ larry, bob, john, bobJr });
        }

        private static IScene MakeAScene()
        {
            var cube = new TestCube();

            return new CoreScene(new List<IVisual3dElement> { cube });
        }
    }
}
