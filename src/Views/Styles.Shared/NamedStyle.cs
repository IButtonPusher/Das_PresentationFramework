using System;

namespace Das.Views.Styles
{
    public class NamedStyle : StyleSheet
    {
        public NamedStyle(String name)
        {
            Name = name;
        }

        public String Name { get; }
        
    }
}
