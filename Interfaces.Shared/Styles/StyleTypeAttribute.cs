using System;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public class StyleTypeAttribute : Attribute
    {
        public StyleTypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}