using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Das.ViewModels;

namespace Das.Views.Wpf;

public class EscapeModalCanceller
{
   public EscapeModalCanceller(Window window,
                               IModalVm viewModel)
   {
      _window = window;
      _viewModel = viewModel;
      window.Closed += OnWindowClosed;
      window.PreviewKeyDown += OnWindowPreviewKeyDown;
   }

   private void OnWindowPreviewKeyDown(Object sender,
                                       KeyEventArgs e)
   {
      if (e.Key != Key.Escape || !_viewModel.CancelCommand.CanExecute(default))
         return;

      _viewModel.CancelCommand.Execute(default);
   }

   private void OnWindowClosed(Object? sender,
                               EventArgs e)
   {
      _window.Closed -= OnWindowClosed;
      _window.PreviewKeyDown -= OnWindowPreviewKeyDown;
   }

   private readonly IModalVm _viewModel;
   private readonly Window _window;
}