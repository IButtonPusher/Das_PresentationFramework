using System;
using System.Threading.Tasks;
using System.Windows;

namespace Das.Views.Wpf
{
    public class ModalWindow : StateRestrictedWindow
    {
        private static void OnInteractionResultChanged(DependencyObject d,
                                                       DependencyPropertyChangedEventArgs e)
        {
            if (d is ModalWindow sw)
                sw.DialogResult = e.NewValue as Boolean?;
        }

        public Boolean? InteractionResult
        {
            get => (Boolean?)GetValue(InteractionResultProperty);
            set => SetValue(InteractionResultProperty, value);
        }

        public static readonly DependencyProperty InteractionResultProperty =
            DependencyProperty.Register(
                nameof(InteractionResult),
                typeof(Boolean?),
                typeof(ModalWindow),
                new FrameworkPropertyMetadata(default(Boolean?),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnInteractionResultChanged));
    }
}
