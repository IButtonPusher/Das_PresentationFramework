using System;

namespace TestCommon
{
    public class EmployeeViewModel : IEmployee
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
    }

    public interface IEmployee
    {
        String FirstName { get; }
        String LastName { get; }


    }
}
