using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Input;

namespace Das.Views
{
    public interface IUiProvider : ISingleThreadedInvoker
    {
        void BeginNotify(String text);

        void BrowseToUri(Uri uri);

        Task<Boolean> ConfirmAsync(String message,
                                   String title);

        Task<Boolean> ConfirmAsync(String message);

        Boolean Confirm(String message,
                        String title);

        Boolean Confirm(String message);

        Task<T?> ShowDialogAsync<T>(IModalVm<T> vm);

        Task<Boolean?> ShowDialogAsync(INotifyPropertyChanged vm);

        Task ShowAsync(INotifyPropertyChanged vm);

        Task CopyTextAsync(Func<String> getText);

        Boolean TryGetFileToOpen(DirectoryInfo initialDirectory,
                                 OpenFileTypes fileType,
                                 out FileInfo file);

        Boolean TryGetFileToSave(DirectoryInfo initialDirectory,
                                 OpenFileTypes fileType,
                                 out FileInfo file);

        Task<FileInfo?> TryGetFileToSaveAsync(DirectoryInfo initialDirectory,
                                        OpenFileTypes fileType);

        Task HandleErrorAsync(String wasDoing,
                              Exception ex);

        void Notify(String text);

        void Notify(String text,
                    String title);

        Task NotifyAsync(String text);

        Task NotifyAsync(String text,
                         String title);

        void NotifyError(String message);

        Task NotifyErrorAsync(String message);

        Task SetCursor(MousePointers cursor);
    }
}
