using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding;

public interface IValueConverter<in TInput, TOutput>: IValueConverter
{
   TOutput Convert(TInput input);

   Task<TOutput> ConvertAsync(TInput input);
}

public interface IValueConverter
{
   Object? Convert(Object? input);
}