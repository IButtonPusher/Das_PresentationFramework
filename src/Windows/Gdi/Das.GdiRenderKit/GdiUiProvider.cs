using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.ViewModels;

namespace Das.Views.Gdi;

public class GdiUiProvider : BaseUiProvider
{
   private readonly IWindowProvider<IVisualHost> _windowProvider;
   private IVisualHost? _visualHost;

   public GdiUiProvider(IWindowProvider<IVisualHost> windowProvider)
   {
      windowProvider.WindowShown += OnWindowShown;
      _windowProvider = windowProvider;
   }

   private void OnWindowShown(IVisualHost visualHost)
   {
      _visualHost = visualHost;
      _windowProvider.WindowShown -= OnWindowShown;
   }

   public override void Notify(String text)
   {
      Invoke(() => NotifyImpl(text, String.Empty));
   }

   public override async Task NotifyAsync(String text,
                                          String title)
   {
      await InvokeAsync(() => NotifyImpl(text, title));
   }

   private static void NotifyImpl(String text,
                                  String title)
   {
      MessageBox.Show(text, title);
   }

   public override void BrowseToUri(Uri uri)
   {
      Process.Start(uri.AbsoluteUri);
   }

   //public override ValueSize GetMainViewSize()
   //{
   //    if (!(_visualHost is { } valid))
   //        return ValueSize.Empty;
   //    return valid.AvailableSize;
   //}

   public override void Invoke(Action action)
   {
      if (_visualHost is { } host)
         host.Invoke(action);
      else
         action();
   }

   public override void BeginInvoke(Action action)
   {
      if (_visualHost is { } host)
         host.BeginInvoke(action);
      else
         action();
   }

   public override Task InvokeAsync(Func<Task> action)
   {
      throw new NotImplementedException();
   }

   public override T Invoke<T>(Func<T> action)
   {
      if (_visualHost is {} host)
         return host.Invoke(action);

      return action();
   }

   public override async Task<T> InvokeAsync<T>(Func<T> action)
   {
      if (_visualHost is {} host)
         return await host.InvokeAsync(action);

      return action();
   }

   public override async Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input,
                                                                    Func<TInput, TOutput> action)
   {
      if (_visualHost is {} host)
         return await host.InvokeAsync(input, action);

      return action(input);
   }

   public override async Task InvokeAsync(Action action)
   {
      if (_visualHost is {} host)
         await host.InvokeAsync(action);
      else
         action();
   }
}