using System;
using System.Collections.Generic;
using Das.Views.Extended;
using Das.Views.Extended.Runtime;
using TestCommon.Company;

namespace TestCommon
{
    public class TestCompanyVm : CompanyViewModel,
                                 IEquatable<TestCompanyVm>
    {
        public TestCompanyVm() 
            : base("ACME Inc.", MakeAScene())
        {
            var larry = new EmployeeViewModel { FirstName = "larry", LastName = "larry" };
            var curly = new EmployeeViewModel { FirstName = "curly", LastName = "sue" };
            var moe = new EmployeeViewModel { FirstName = "moe", LastName = "money" };
            var bob = new EmployeeViewModel { FirstName = "bob", LastName = "ooo" };
            var john = new EmployeeViewModel { FirstName = "john", LastName = "doe" };
            var bobJr = new EmployeeViewModel { FirstName = "bob", LastName = "jones" };

            Employees.AddRange(new List<EmployeeViewModel>{ larry, curly, moe, bob, john, bobJr });
        }

        private static IScene MakeAScene()
        {
            //var fileName = @"E:\src\master_clones\ThirdParty\FbxWriter-master\FbxTest\bin\Debug\kettle.fbx";
            //var fileName = @"E:\src\master_clones\ThirdParty\FbxWriter-master\FbxTest\bin\Debug\cube.fbx";

            //var lodr = new CoreFbxLoader();
            //var fi = new FileInfo(fileName);
            //var cube = lodr.LoadModelAsync(fi).Result;

            var cube2 = new TestCube();
            return new CoreScene(new List<IMesh> { cube2 });

            //return new CoreScene(cube.Meshes);

            
            
        }

        public Boolean Equals(TestCompanyVm other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
