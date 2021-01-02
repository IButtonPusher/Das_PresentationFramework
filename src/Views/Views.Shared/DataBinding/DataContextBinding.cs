using System;

namespace Das.Views.DataBinding
{
    public class DataContextBinding : BaseBinding
    {
        public String TargetPropertyName { get; }

        public IValueConverter? Converter { get; }

        public DataContextBinding(String targetPropertyName,
            IValueConverter? converter)
        {
            TargetPropertyName = targetPropertyName;
            Converter = converter;
        }

        public override Object? GetBoundValue(Object? dataContext)
        {
            if (Converter is { } converter)
                return converter.Convert(dataContext);

            return dataContext;

        }

        public override IDataBinding Update(Object? dataContext, 
                                            IVisualElement targetVisual)
        {
            var targetProp = GetObjectPropertyOrDie(targetVisual, TargetPropertyName);
            var setter = targetProp.GetSetMethod();

            var val = GetBoundValue(dataContext);
            
            
            setter.Invoke(targetVisual, new [] {val});

            return base.Update(dataContext, targetVisual);
        }

        public override Object Clone()
        {
            return new DataContextBinding(TargetPropertyName, Converter);
        }
    }
}
