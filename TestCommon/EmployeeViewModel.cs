using System;

namespace TestCommon
{
    public class EmployeeViewModel : IEmployee,
                                     IEquatable<EmployeeViewModel>
    {
        public Int32 Id { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }

        public Boolean Equals(EmployeeViewModel other)
        {
            return other?.Id == Id;
        }

        public override String ToString()
        {
            return FirstName + " " + LastName;
        }
    }

    public interface IEmployee
    {
        Int32 Id { get; }

        String FirstName { get; }
        String LastName { get; }


    }
}
