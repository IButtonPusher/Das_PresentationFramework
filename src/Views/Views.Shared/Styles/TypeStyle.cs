using System;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    // ReSharper disable once UnusedTypeParameter
    public class TypeStyle<T> : TypeStyle 
        where T : IVisualElement
    {
        public TypeStyle() : base(typeof(T))
        {
            
        }
    }

    public abstract class TypeStyle : StyleSheet
    {
        protected TypeStyle(Type targetType)
        : base(Enumerable.Empty<IStyleRule>())
        {
            TargetType = targetType;
        }

        public Type TargetType { get;  }
    }
}