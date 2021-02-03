using System;

namespace Das.Views.Resources
{
    public class EmbeddedResourceBinding : IPropertyBinding
    {
        public EmbeddedResourceBinding(String path,
                                       Object value)
        {
            Path = path;
            Value = value;
        }

        public String Path { get; }
        public Object Value { get; }
    }
}
