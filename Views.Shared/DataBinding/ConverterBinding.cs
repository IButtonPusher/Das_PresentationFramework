using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class ConverterBinding<TInput, TOutput> : BaseBinding<TOutput>
    {
        public ConverterBinding(IDataBinding<TInput> binding,
            IValueConverter<TInput, TOutput> converter)
        {
            _binding = binding;
            _converter = converter;
        }

        public override IDataBinding<TOutput> DeepCopy()
        {
            var innerCopy = _binding.DeepCopy();
            var conv = new ConverterBinding<TInput, TOutput>(innerCopy, _converter) {DataContext = null!};
            return conv;
        }

        public override TOutput GetValue(Object dataContext)
        {
            var val = _binding.GetValue(dataContext);
            var converted = _converter.Convert(val);
            return converted;
        }

        public override async Task<TOutput> GetValueAsync(Object dataContext)
        {
            var val = await _binding.GetValueAsync(dataContext);
            var converted = await _converter.ConvertAsync(val);
            return converted;
        }

        public override IDataBinding ToSingleBinding()
        {
            throw new NotImplementedException();
        }

        private readonly IDataBinding<TInput> _binding;
        private readonly IValueConverter<TInput, TOutput> _converter;
    }
}