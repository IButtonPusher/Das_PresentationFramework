using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public interface ITypefaceMetrics
    {
        Double XHeight { get; }

        Double CapsHeight { get; }

        Double UnderlinePosition { get; }

        Double UnderlineThickness { get; }

        Double StrikethroughPosition { get; }

        Double StrikethroughThickness { get; }

        Boolean Symbol { get; }

        StyleSimulations StyleSimulations { get; }

        //IDictionary<XmlLanguage, string> AdjustedFaceNames { get; }
    }
}
