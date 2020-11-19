using System;

namespace TestCommon
{
    public class EmployeeViewModel : IEmployee
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }

        public override String ToString()
        {
            return FirstName + " " + LastName;
        }
    }

    public interface IEmployee
    {
        String FirstName { get; }
        String LastName { get; }


    }
}
