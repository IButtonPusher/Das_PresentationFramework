namespace Das.Views.Core.Writing
{
    public interface IFontProvider
    {
        IFontRenderer GetRenderer(IFont font);
    }
}
