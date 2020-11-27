using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Writing
{
    public interface IFont
    {
        String FamilyName { get; }

        FontStyle FontStyle { get; }

        Double Size { get; }

        IFont Resize(Double newSize);
    }
}