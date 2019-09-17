using System;

namespace Das.Views.Core.Writing
{
    public interface IFont
    {
        Double Size { get; }
        String FamilyName { get; }
        FontStyle FontStyle { get; }
    }
}