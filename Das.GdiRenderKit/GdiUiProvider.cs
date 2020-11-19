using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Core.Geometry;

namespace Das.Views.Gdi
{
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

        public override void BrowseToUri(Uri uri)
        {
            Process.Start(uri.AbsoluteUri);
        }

        public override ValueSize GetMainViewSize()
        {
            if (!(_visualHost is { } valid))
                return ValueSize.Empty;
            return valid.AvailableSize;
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
}
