using System;
using System.Threading.Tasks;

namespace Common.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TriggersIsChangedAttribute : Attribute
    {
    }
}
