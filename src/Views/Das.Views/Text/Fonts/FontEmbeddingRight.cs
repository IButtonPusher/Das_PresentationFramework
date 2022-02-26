using System;
using System.Threading.Tasks;

namespace Das.Views.Text
{
    public enum FontEmbeddingRight
    {
        Installable,
        InstallableButNoSubsetting,
        InstallableButWithBitmapsOnly,
        InstallableButNoSubsettingAndWithBitmapsOnly,
        RestrictedLicense,
        PreviewAndPrint,
        PreviewAndPrintButNoSubsetting,
        PreviewAndPrintButWithBitmapsOnly,
        PreviewAndPrintButNoSubsettingAndWithBitmapsOnly,
        Editable,
        EditableButNoSubsetting,
        EditableButWithBitmapsOnly,
        EditableButNoSubsettingAndWithBitmapsOnly,
    }
}
