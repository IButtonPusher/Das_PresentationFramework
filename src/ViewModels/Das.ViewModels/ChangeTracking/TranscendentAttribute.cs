using System;
using System.Threading.Tasks;

namespace Das.ViewModels.ChangeTracking
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Assembly)]
    public class TranscendentAttribute : Attribute
    {
    }
}
