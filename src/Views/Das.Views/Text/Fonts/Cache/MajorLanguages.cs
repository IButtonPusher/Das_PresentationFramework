using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Cache
{
    public static class MajorLanguages
    {
        internal static Boolean Contains(ScriptTags script,
                                         LanguageTags langSys)
        {
            for (var index = 0; index < majorLanguages.Length; ++index)
            {
                if (script == majorLanguages[index].Script &&
                    (langSys == LanguageTags.Default || langSys == majorLanguages[index].LangSys))
                    return true;
            }

            return false;
        }

        internal static Boolean Contains(CultureInfo culture)
        {
            if (culture == null)
                return false;
            if (culture == CultureInfo.InvariantCulture)
                return true;
            for (var index = 0; index < majorLanguages.Length; ++index)
            {
                if (majorLanguages[index].Culture.Equals(culture) ||
                    majorLanguages[index].Culture.Equals(culture.Parent))
                    return true;
            }

            return false;
        }

        private static readonly MajorLanguageDesc[] majorLanguages = new MajorLanguageDesc[4]
        {
            new MajorLanguageDesc(new CultureInfo("en"), ScriptTags.Latin, LanguageTags.English),
            new MajorLanguageDesc(new CultureInfo("de"), ScriptTags.Latin, LanguageTags.German),
            new MajorLanguageDesc(new CultureInfo("ja"), ScriptTags.CJKIdeographic, LanguageTags.Japanese),
            new MajorLanguageDesc(new CultureInfo("ja"), ScriptTags.Hiragana, LanguageTags.Japanese)
        };

        private struct MajorLanguageDesc
        {
            internal readonly CultureInfo Culture;
            internal readonly ScriptTags Script;
            internal readonly LanguageTags LangSys;

            internal MajorLanguageDesc(CultureInfo culture,
                                       ScriptTags script,
                                       LanguageTags langSys)
            {
                Culture = culture;
                Script = script;
                LangSys = langSys;
            }
        }
    }
}
