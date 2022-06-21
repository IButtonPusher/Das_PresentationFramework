using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Das.ViewModels;
using Microsoft.Win32;

namespace Das.Views.Wpf
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TModalWindow">The type of window created to show modal content</typeparam>
    public partial class WpfUiProvider<TModalWindow> : BaseUiProvider
        where TModalWindow : ModalWindow, new()
    {
        public WpfUiProvider() : this(Application.Current.Resources)
        {
        }

        public WpfUiProvider(ResourceDictionary appResources)
        {
            _resources = appResources;
            _cachedContents = new Dictionary<Type, FrameworkElement>();
        }

        public override void Notify(String text)
        {
            Notify(text, String.Empty);
        }

        public override async Task NotifyAsync(String text,
                                               String title)
        {
            await InvokeAsync(() => NotifyImpl(text, title));
        }

        public override void BrowseToUri(Uri uri)
        {
            Process.Start(uri.AbsoluteUri);
        }

        public override Boolean Confirm(String message)
        {
            return Confirm(message, String.Empty);
        }

        public override Boolean Confirm(String message,
                                        String title)
        {
            return Invoke(() => MessageBox.Show(message, title, MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes);
        }

        public override void Invoke(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                Application.Current.Dispatcher.Invoke(action);
        }

        public override async Task<T> InvokeAsync<T>(Func<T> action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                return action();
            
            return await Application.Current.Dispatcher.InvokeAsync(action);
        }

        public override T Invoke<T>(Func<T> action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                return action();
            
            return Application.Current.Dispatcher.Invoke(action);
        }

        public override async Task InvokeAsync(Func<Task> action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                await action();
            else
                await Application.Current.Dispatcher.InvokeAsync(action).Task.Unwrap();
        }

        public override void BeginInvoke(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                Application.Current.Dispatcher.BeginInvoke(action);
        }

        public override async Task InvokeAsync(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                await Application.Current.Dispatcher.InvokeAsync(action);
        }

        public override Boolean TryGetFileToOpen(DirectoryInfo initialDirectory,
                                                 OpenFileTypes fileType,
                                                 out FileInfo file)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = initialDirectory.FullName,
                Filter = GetExtensionsFilterText(fileType)
            };

            return TryGetFileImpl(ofd, out file);
        }

        public override async Task CopyTextAsync(Func<String> getText)
        {
            await InvokeAsync(() =>
            {
                Clipboard.SetText(getText());
            });
        }

        public override Boolean TryGetFileToSave(DirectoryInfo initialDirectory,
                                                 OpenFileTypes fileType,
                                                 out FileInfo file)
        {
            var ofd = new SaveFileDialog
            {
                InitialDirectory = initialDirectory.FullName,
                Filter = GetExtensionsFilterText(fileType)
            };

            return TryGetFileImpl(ofd, out file);
        }

        public override async Task<FileInfo?> TryGetFileToSaveAsync(DirectoryInfo initialDirectory,
                                                                    OpenFileTypes fileType)
        {
            return await InvokeAsync(() =>
            {
                var ofd = new SaveFileDialog
                {
                    InitialDirectory = initialDirectory.FullName,
                    Filter = GetExtensionsFilterText(fileType)
                };

                if (TryGetFileImpl(ofd, out var file))
                    return file;

                return default;
            });
        }

        private static void NotifyImpl(String text,
                                       String subject)
        {
            var pwner = GetModalOwner();
            if (pwner != null)
                MessageBox.Show(pwner, text, subject);
            else
                MessageBox.Show(text, subject);
        }

        private static String GetExtensionsFilterText(OpenFileTypes openType)
        {
            switch (openType)
            {
                case OpenFileTypes.Text:
                    return "Text Files|*.txt";
                case OpenFileTypes.Xml:
                    return "Xml Files|*.xml";
                case OpenFileTypes.PngOrAny:
                    return "PNG Files (.png)|*.png|All Files (*.*)|*.*";
                case OpenFileTypes.Zip:
                    return "NoteCaddy coaching packages|*.zip";
                default:
                    throw new NotImplementedException();
            }
        }

        private static Boolean? ShowCommonDialog(CommonDialog dialog)
        {
            if (GetModalOwner() is { } pwner)
                return dialog.ShowDialog(pwner);
            return dialog.ShowDialog();
        }

        private static Boolean TryGetFileImpl(FileDialog ofd,
                                              out FileInfo file)
        {
            if (ShowCommonDialog(ofd) == true)
            {
                file = new FileInfo(ofd.FileName);
                return true;
            }

            file = default!;

            return false;
        }

        private static Window? GetModalOwner()
        {
            var res = ActiveWindow ?? Application.Current.MainWindow;
            return res?.IsVisible == true ? res : default;
        }

        private static Window? ActiveWindow =>
            Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive) ??
            Application.Current.Windows.OfType<Window>()
                       .SingleOrDefault(x => x.IsVisible && x.Owner == Application.Current.MainWindow);

        private readonly ResourceDictionary _resources;
    }
}
