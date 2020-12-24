using System;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public class StyleTypeAttribute : Attribute
    {
        public StyleTypeAttribute(Type type, 
                                  Boolean isCrossTypeInheritable)
        {
            Type = type;
            IsCrossTypeInheritable = isCrossTypeInheritable;
        }

        public Type Type { get; set; }

        public Boolean IsCrossTypeInheritable { get; }
    }
}