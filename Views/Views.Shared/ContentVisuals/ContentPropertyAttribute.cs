using System;

namespace Das.Views
{
    public class ContentPropertyAttribute : Attribute
    {
        public ContentPropertyAttribute(String name)
        {
            Name = name;
        }

        public String Name { get; }
    }
}
