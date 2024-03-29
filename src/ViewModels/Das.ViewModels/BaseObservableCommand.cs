﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Das.Views;
using Das.Views.Mvvm;

namespace Das.ViewModels;

public class BaseObservableCommand : NotifyPropertyChangedBase,
                                     IObservableCommand,
                                     ICommand
{
   public BaseObservableCommand(Action execute,
                                IUiProvider uiThread)
      : this(execute, _ => true, uiThread)
   {
   }


   public BaseObservableCommand(Func<Task> execute,
                                IUiProvider uiThread)
      : this(execute, _ => true, uiThread)
   {
      _uiProvider = uiThread;
      _executeAsync = execute;
      _canExecute = CanIExecute;
   }

   public BaseObservableCommand(Action execute,
                                Predicate<Object> canExecute,
                                IUiProvider ui)
   {
      _description = String.Empty;
      _execute = execute;
      _uiProvider = ui;
      _canExecute = canExecute ?? CanIExecute;
      _canExecuteVal = true;
   }

   public BaseObservableCommand(Func<Task> execute,
                                Predicate<Object> canExecute,
                                IUiProvider ui)
   {
      _description = String.Empty;
      _executeAsync = execute;
      _uiProvider = ui;
      _canExecute = canExecute ?? CanIExecute;
      _canExecuteVal = true;
   }

   public Boolean IsExecutable
   {
      get => _canExecuteVal;
      set => SetValue(ref _canExecuteVal, value, OnCanExecuteChanged);
   }

   public String? Description
   {
      get => _description;
      set => SetValue(ref _description, value);
   }

   public async Task ExecuteAsync()
   {
      if (_executeAsync is {} valid)
         await valid();
      else if (_execute is {} goodEnough)
         goodEnough();
      else throw new NullReferenceException();
   }

   Task IObservableCommand.ExecuteAsync(Object paramValue)
   {
      return ExecuteAsync();
   }

   public Boolean Equals(IObservableCommand other)
   {
      return ReferenceEquals(other, this);
   }

   public override void Dispose()
   {
      base.Dispose();
      CanExecuteChanged = null;
   }


   public Boolean CanExecute(Object parameter)
   {
      return _runCounter == 0 &&
             _canExecute != null && _canExecute(parameter);
   }

   public virtual async void Execute(Object parameter)
   {
      var chenged = CanExecuteChanged;

      Interlocked.Increment(ref _runCounter);

      if (chenged != null)
      {
         chenged.Invoke(this, EventArgs.Empty);
         await Task.Yield();
      }

      try
      {
         if (_executeAsync != null)
            await _executeAsync();
         else if (_execute is {} goodEnough)
            goodEnough();
         else throw new NullReferenceException();
      }
      finally
      {
         Interlocked.Decrement(ref _runCounter);
         chenged?.Invoke(this, EventArgs.Empty);
      }
   }

   private Boolean CanIExecute(Object obj)
   {
      return _canExecuteVal;
   }

   private async Task OnCanExecuteChanged(Boolean canExecute)
   {
      var can = CanExecuteChanged;
      if (can == null)
         return;

      await _uiProvider.InvokeAsync(() => { CanExecuteChanged?.Invoke(canExecute, EventArgs.Empty); });
   }

   public event EventHandler? CanExecuteChanged;

   /// <summary>
   ///     Encapsulated the representation for the validation of the execute method
   /// </summary>
   private readonly Predicate<Object> _canExecute;

   /// <summary>
   ///     Encapsulated the execute action
   /// </summary>
   private readonly Action? _execute;

   private readonly Func<Task>? _executeAsync;
   private readonly IUiProvider _uiProvider;


   private Boolean _canExecuteVal;
   private String? _description;
   protected Int32 _runCounter;
}