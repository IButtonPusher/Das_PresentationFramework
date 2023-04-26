using System;
using System.IO;
using System.Threading.Tasks;

namespace Das.Views;

public interface IViewProvider
{
   Task<IVisualElement> GetView(FileInfo file);
}