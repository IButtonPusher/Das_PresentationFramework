using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    [Flags]
    public enum StyleSimulations
    {
        None = 0,
        BoldSimulation = 1,
        ItalicSimulation = 2,
        BoldItalicSimulation = ItalicSimulation | BoldSimulation, // 0x00000003
    }
}
