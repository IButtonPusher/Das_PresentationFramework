using System;
using System.Threading.Tasks;

namespace Das.ViewModels.ChangeTracking
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TriggersIsChangedAttribute : Attribute
    {
       public TriggersIsChangedAttribute(Boolean checkEquality = true)
        {
            CheckEquality = checkEquality;
        }

        public Boolean CheckEquality { get; set; }
    }
}
