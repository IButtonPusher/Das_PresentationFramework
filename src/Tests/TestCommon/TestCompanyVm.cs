using System;
using System.Collections.Generic;

using TestCommon.Company;

namespace TestCommon
{
    public class TestCompanyVm : CompanyViewModel,
                                 IEquatable<TestCompanyVm>
    {
        public TestCompanyVm() 
            : base("ACME Inc.")//, MakeAScene())
        {
           var address1 = new AddressVm
            {
                HouseNumber = "11280",
                Street = "NW 35th Court",
                City = "Coral Springs",
                State = "FL",
                ZipCode = 33065
            };

            var address2 = new AddressVm
            {
                HouseNumber = "8546",
                Street = "NW 28th Court",
                City = "North Lauderdale",
                State = "Florida",
                ZipCode = 33068
            };

            var addies = new List<AddressVm> {address1, address2};
            
            var larry = new EmployeeViewModel { FirstName = "larry", LastName = "larry", Addresses = addies};
            var curly = new EmployeeViewModel { FirstName = "curly", LastName = "sue" };
            var moe = new EmployeeViewModel { FirstName = "moe", LastName = "money", Addresses = addies };
            var bob = new EmployeeViewModel { FirstName = "bob", LastName = "ooo" };
            var john = new EmployeeViewModel { FirstName = "john", LastName = "doe", Addresses = addies };
            var bobJr = new EmployeeViewModel { FirstName = "bob", LastName = "jones" };

            var allMyEmployees = new List<EmployeeViewModel> { larry, curly, moe, bob, john, bobJr };
            foreach (var employee in allMyEmployees)
                Employees.Add(employee);
            //Employees.AddRange(new List<EmployeeViewModel>{ larry, curly, moe, bob, john, bobJr });
        }

        //private static IScene MakeAScene()
        //{
        //    //var fileName = @"E:\src\master_clones\ThirdParty\FbxWriter-master\FbxTest\bin\Debug\kettle.fbx";
        //    //var fileName = @"E:\src\master_clones\ThirdParty\FbxWriter-master\FbxTest\bin\Debug\cube.fbx";

        //    //var lodr = new CoreFbxLoader();
        //    //var fi = new FileInfo(fileName);
        //    //var cube = lodr.LoadModelAsync(fi).Result;

        //    var cube2 = new TestCube();
        //    return new CoreScene(new List<IMesh> { cube2 });

        //    //return new CoreScene(cube.Meshes);

            
            
        //}

        public Boolean Equals(TestCompanyVm other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
