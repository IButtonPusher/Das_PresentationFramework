namespace Das.Views.DataBinding
{
    public class ConverterBinding<TInput, TOutput> : BaseBinding<TOutput>
    {
        private readonly IDataBinding<TInput> _binding;
        private readonly IValueConverter<TInput, TOutput> _converter;

        public ConverterBinding(IDataBinding<TInput> binding,
            IValueConverter<TInput, TOutput> converter)
        {
            _binding = binding;
            _converter = converter;
        }

        public override TOutput GetValue(object dataContext)
        {
            var val = _binding.GetValue(dataContext);
            var converted = _converter.Convert(val);
            return converted;
        }

        public override IDataBinding<TOutput> DeepCopy()
        {
            var innerCopy = _binding.DeepCopy();
            //innerCopy.DataContext = null;
            var conv = new ConverterBinding<TInput, TOutput>(innerCopy, _converter);
            conv.DataContext = null;
            return conv;
        }

        public override IDataBinding ToSingleBinding()
            => throw new System.NotImplementedException();
    }
}