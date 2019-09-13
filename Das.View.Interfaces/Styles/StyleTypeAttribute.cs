using System;

namespace Das.Views.Styles
{
    public class StyleTypeAttribute : Attribute
    {
        public Type Type { get; set; }

        public StyleTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}