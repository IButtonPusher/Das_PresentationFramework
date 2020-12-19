using System;
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

    public abstract class TypeStyle : CascadingStyleSheet
    {
        protected TypeStyle(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get;  }
    }
}