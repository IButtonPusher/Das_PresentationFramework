using System;
using System.Collections.Generic;
using System.Text;

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
