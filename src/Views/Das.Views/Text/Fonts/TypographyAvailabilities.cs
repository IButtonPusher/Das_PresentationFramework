using System;
using System.Threading.Tasks;

namespace Das.Views.Text
{
    [Flags]
    public enum TypographyAvailabilities
    {
        None = 0,
        Available = 1,
        IdeoTypographyAvailable = 2,
        FastTextTypographyAvailable = 4,
        FastTextMajorLanguageLocalizedFormAvailable = 8,
        FastTextExtraLanguageLocalizedFormAvailable = 16, // 0x00000010
    }
}
