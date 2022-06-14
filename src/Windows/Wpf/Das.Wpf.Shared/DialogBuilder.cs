using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Das.ViewModels;
using Das.Views.Wpf.Windows;

namespace Das.Views.Wpf
{
    public partial class WpfUiProvider<TModalWindow>
    {
        public override async Task<T?> ShowDialogAsync<T>(IModalVm<T> vm) where T : default
        {
            var windowsResult = await ShowBooleanDialogAsync(vm);
            if (windowsResult != true)
                return default;

            return vm.DialogResult;
        }

        public override Task<Boolean?> ShowDialogAsync(INotifyPropertyChanged vm) => ShowBooleanDialogAsync(vm);

        private async Task<Boolean?> ShowBooleanDialogAsync(INotifyPropertyChanged vm)
        {
            return await InvokeAsync(() => ShowBooleanDialogImpl(vm));
        }

        private static void UpdateWindow(ITitledVm titled,
                                         ModalWindow ww)
        {
            var titleBinding = new Binding(nameof(ITitledVm.Title))
            {
                Source = titled
            };
            BindingOperations.SetBinding(ww, Window.TitleProperty, titleBinding);

            if (titled is IModalVm modal)
            {
                ww.CanCloseWindow = modal.CanClose;
                ww.CanMaximizeWindow = modal.CanMaximize;
                ww.CanMinimizeWindow = modal.CanMinimize;

                if (!modal.CanResize)
                    ww.ResizeMode = ResizeMode.NoResize;

                var dialogResultBinding = new Binding(nameof(IModalVm.DialogCompleted))
                {
                    Source = modal
                };
                BindingOperations.SetBinding(ww, ModalWindow.InteractionResultProperty,
                    dialogResultBinding);

                var _ = new EscapeModalCanceller(ww, modal);
            }
        }

        private static Boolean TryGetWindowSizeFromAttribute(Object? windowContents,
                                                             out Size size)
        {
            if (windowContents is { } wc &&
                wc.GetType().GetCustomAttribute<DialogSizeAttribute>() is { } sattr &&
                sattr.Width > 0 && sattr.Height > 0)
            {
                size = new Size(sattr.Width, sattr.Height);
                return true;
            }

            size = Size.Empty;
            return false;
        }

        private Window GetWindowForDialog(INotifyPropertyChanged vm)
        {
            var isSizeWindow = false;
            var windowSize = Size.Empty;

            Object? windowContents = vm;

            if (_cachedContents.TryGetValue(vm.GetType(), out var cached))
            {
                isSizeWindow = TryGetWindowSizeFromAttribute(cached, out windowSize);
                cached.DataContext = vm;
                windowContents = cached;

                goto makeWindow;
            }

            var k = new DataTemplateKey(vm.GetType());
            var finded = _resources[k];


            if (finded is DataTemplate tf)
            {
                //found a data template or vm's type

                windowContents = tf.LoadContent();

                isSizeWindow = TryGetWindowSizeFromAttribute(windowContents, out windowSize);


                if (windowContents is Window templateWindow)
                {
                    //vm => window
                    templateWindow.DataContext = vm;
                    templateWindow.Resources = _resources;
                    return templateWindow;
                }

                if (windowContents is FrameworkElement fwe &&
                    windowContents.GetType().GetCustomAttribute<VisualShouldBeReusedAttribute>() is { })
                {
                    _cachedContents[vm.GetType()] = fwe;
                }
            }

            makeWindow:

            var stc = isSizeWindow
                ? SizeToContent.Manual
                : SizeToContent.WidthAndHeight;


            var ww = new TModalWindow
            {
                Content = windowContents,
                DataContext = vm,
                SizeToContent = stc,
                ShowInTaskbar = false,
                Resources = _resources,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (isSizeWindow)
            {
                ww.Height = windowSize.Height;
                ww.Width = windowSize.Width;
            }

            if (vm is ITitledVm titled)
                UpdateWindow(titled, ww);

            return ww;
        }

        private Boolean? ShowBooleanDialogImpl(INotifyPropertyChanged vm)
        {
            var window = GetWindowForDialog(vm);
            if (GetModalOwner() is { } pwner)
                window.Owner = pwner;

            return window.ShowDialog();
        }

        private readonly Dictionary<Type, FrameworkElement> _cachedContents;
    }
}
