using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;

namespace Das.Views.Converters
{
    public abstract class BaseConverter<TFrom, TTo> : IValueConverter<TFrom, TTo>
    {
        public abstract TTo Convert(TFrom input);

        public virtual Task<TTo> ConvertAsync(TFrom input)
        {
            var res = Convert(input);
            
            #if NET40
            
            return TaskEx.FromResult(res);
            
            #else
            
            return Task.FromResult(res);
            
            #endif
            
            
        }

        Object? IValueConverter.Convert(Object? input)
        {
            if (input is TFrom { } str)
                return Convert(str);
            
            throw new InvalidCastException();
        }
    }
}
