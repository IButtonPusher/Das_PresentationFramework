using System.IO;
using System.Threading.Tasks;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IViewProvider
    {
        Task<IView> GetView(FileInfo file);
    }
}
