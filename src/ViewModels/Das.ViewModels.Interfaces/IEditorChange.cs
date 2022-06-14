using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public interface IEditorChange : IChangeTracking
    {
        void UpdateIsChanged(Boolean value);
    }
}
