using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IViewProvider
    {
        Task<IVisualElement> GetView(FileInfo file);
    }
}