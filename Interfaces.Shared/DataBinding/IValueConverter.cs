using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public interface IValueConverter<in TInput, TOutput>
    {
        TOutput Convert(TInput input);

        Task<TOutput> ConvertAsync(TInput input);
    }
}