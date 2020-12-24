using System;
using System.Threading.Tasks;

namespace Das.Views.Controls
{
    public interface IToggleButton : IButtonBase
    {
        Boolean? IsChecked { get; set; }
    }
}