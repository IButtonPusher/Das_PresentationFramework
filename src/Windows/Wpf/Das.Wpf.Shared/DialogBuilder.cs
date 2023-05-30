using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Das.ViewModels;
using Das.Views.Wpf.Windows;

namespace Das.Views.Wpf;

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

   public override async Task ShowAsync(INotifyPropertyChanged vm)
   {
      await InvokeAsync(() =>
      {
         var window = GetWindow(vm, true);

         if (vm is IDisposable dvm)
         {
            window.Closed += delegate
            {
               dvm.Dispose();
            };
         }

         window.Show();
      });
   }

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

   private static Boolean TryGetWindowDisplayFromAttribute(Object? windowContents,
                                                        out Size size,
                                                        out Boolean? showInTaskBar)
   {
      if (windowContents != null &&
          windowContents.GetType().GetCustomAttribute<WindowDisplayAttribute>() is { } sattr &&
          sattr.Width > 0 && sattr.Height > 0)
      {
         size = new Size(sattr.Width, sattr.Height);
         showInTaskBar = sattr.ShowInTaskBar;
         return true;
      }

      size = Size.Empty;
      showInTaskBar = default;
      return false;
   }

   private Window GetWindow(INotifyPropertyChanged vm,
                            Boolean showInTaskbar)
   {
      var isSizeWindow = false;
      var windowSize = Size.Empty;
      Boolean? attrShowInTaskBar = null;

      Object? windowContents = vm;

      if (_cachedContents.TryGetValue(vm.GetType(), out var cached))
      {
         isSizeWindow = TryGetWindowDisplayFromAttribute(cached, 
            out windowSize,
            out attrShowInTaskBar);
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
         isSizeWindow = TryGetWindowDisplayFromAttribute(windowContents, 
            out windowSize,
            out attrShowInTaskBar);

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

      if (attrShowInTaskBar.HasValue)
         showInTaskbar = attrShowInTaskBar.Value;

      var ww = new TModalWindow
      {
         Content = windowContents,
         DataContext = vm,
         SizeToContent = stc,
         ShowInTaskbar = showInTaskbar,
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
      var window = GetWindow(vm, false);
      if (GetModalOwner() is { } pwner)
         window.Owner = pwner;
      
      return window.ShowDialog();
   }

   private readonly Dictionary<Type, FrameworkElement> _cachedContents;
}