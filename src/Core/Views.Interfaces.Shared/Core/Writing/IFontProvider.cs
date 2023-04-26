using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Writing;

public interface IFontProvider<out TRenderer> where TRenderer : IFontRenderer
{
   TRenderer GetRenderer(IFont font);
}

public interface IFontProvider
{
   IFontRenderer GetRenderer(IFont font);
}