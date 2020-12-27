using System;
using Das.Views.Controls;

namespace Das.Views.Converters
{
    public class StringToVisualConverter : BaseConverter<String, IVisualElement>
    {
        private readonly IVisualBootstrapper _visualBootstrapper;

        public StringToVisualConverter(IVisualBootstrapper visualBootstrapper)
        {
            _visualBootstrapper = visualBootstrapper;
        }

        public override IVisualElement Convert(String input)
        {
            return new Label(_visualBootstrapper) {Text = input};
        }
    }
}
