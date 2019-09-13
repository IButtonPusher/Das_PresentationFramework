using System;

namespace Das.Views.DataBinding
{
    public interface IValueConverter<in TInput, out TOutput>
    {
        TOutput Convert(TInput input);
    }
}