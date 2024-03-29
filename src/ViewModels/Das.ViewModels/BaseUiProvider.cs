﻿using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Input;

namespace Das.ViewModels;

public abstract class BaseUiProvider : IUiProvider
{
   public virtual void BeginNotify(String text)
   {
      throw new NotImplementedException();
   }

   public virtual Task<Boolean> ConfirmAsync(String message, 
                                             String title)
   {
      throw new NotImplementedException();
   }

   public virtual Task<Boolean> ConfirmAsync(String message)
   {
      throw new NotImplementedException();
   }

   public virtual Boolean Confirm(String message,
                                  String title)
   {
      throw new NotImplementedException();
   }

   public virtual Boolean Confirm(String message)
   {
      throw new NotImplementedException();
   }

   public virtual Task<T?> ShowDialogAsync<T>(IModalVm<T> vm)
   {
      throw new NotImplementedException();
   }

   public virtual Task<Boolean?> ShowDialogAsync(INotifyPropertyChanged vm)
   {
      throw new NotImplementedException();
   }

   public virtual Task ShowAsync(INotifyPropertyChanged vm) => throw new NotImplementedException();

   public virtual Task CopyTextAsync(Func<String> getText)
   {
      throw new NotImplementedException();
   }

   public virtual Boolean TryGetFileToOpen(DirectoryInfo initialDirectory,
                                           OpenFileTypes fileType,
                                           out FileInfo file)
   {
      throw new NotImplementedException();
   }

   public virtual Boolean TryGetFileToSave(DirectoryInfo initialDirectory,
                                           OpenFileTypes fileType,
                                           out FileInfo file)
   {
      throw new NotImplementedException();
   }

   public virtual Task<FileInfo?> TryGetFileToSaveAsync(DirectoryInfo initialDirectory,
                                                        OpenFileTypes fileType)
   {
      throw new NotImplementedException();
   }

   //public virtual IObservableCommand GetCommand(Action action)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand GetCommand(Action action, 
   //                                             String description)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand GetCommand(Func<Task> action, 
   //                                             String description)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand<T> GetCommand<T>(Func<T, Task> action)
   //{
   //    return new BaseObservableCommand<T>(action);
   //}

   //public virtual IObservableCommand<T> GetCommand<T>(Action<T> action)
   //{
   //    return new ObservableActionCommand<T>(action, this);
   //}

   //public virtual IObservableCommand<T> GetCommand<T>(Func<T, Task> action,
   //                                                   String description)
   //{
   //    return new ObservableAsyncCommand<T>(action, this) { Description = description };
   //}

   //public virtual IObservableCommand<T> GetCommand<T>(Func<IEnumerable<T>, Task> action, 
   //                                                   String description)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand<T> GetCommand<T>(Func<T[], Task> action)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand<T> GetCommand<T>(Func<T[], Task> action, 
   //                                                   String description)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand GetCommand(Action action, 
   //                                             INotifyPropertyChanged viewModel, 
   //                                             String canExecuteProperty)
   //{
   //    throw new NotImplementedException();
   //}

   //public virtual IObservableCommand GetCommand(Func<Task> action)
   //{
   //    return new BaseObservableCommand(action, this);
   //}

   public virtual Task HandleErrorAsync(String wasDoing, Exception ex)
   {
      throw new NotImplementedException();
   }

   public abstract void Notify(String text);

   public virtual void Notify(String text, String title)
   {
      throw new NotImplementedException();
   }

   public virtual Task NotifyAsync(String text)
   {
      throw new NotImplementedException();
   }

   public abstract Task NotifyAsync(String text,
                                    String title);

   public virtual void NotifyError(String message)
   {
      throw new NotImplementedException();
   }

   public virtual Task NotifyErrorAsync(String message)
   {
      throw new NotImplementedException();
   }

   public virtual Task SetCursor(MousePointers cursor)
   {
      throw new NotImplementedException();
   }

   public abstract void BrowseToUri(Uri uri);

   //public abstract ValueSize GetMainViewSize();

   public virtual void BeginInvoke(Action action)
   {
      throw new NotImplementedException();
   }

   public abstract void Invoke(Action action);

   public virtual void Invoke(Action action, Int32 priority)
   {
      throw new NotImplementedException();
   }

   public abstract Task InvokeAsync(Func<Task> action);

   public virtual T Invoke<T>(Func<T> action)
   {
      throw new NotImplementedException();
   }

   public virtual Task InvokeAsync(Action action)
   {
      throw new NotImplementedException();
   }

   public virtual Task<T> InvokeAsync<T>(Func<T> action)
   {
      throw new NotImplementedException();
   }

   public virtual Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input, 
                                                             Func<TInput, TOutput> action)
   {
      throw new NotImplementedException();
   }

   public virtual Task InvokeAsync<TInput>(TInput input, Func<TInput, Task> action)
   {
      throw new NotImplementedException();
   }

   public virtual Task<T> InvokeAsync<T>(Func<Task<T>> action)
   {
      throw new NotImplementedException();
   }
}