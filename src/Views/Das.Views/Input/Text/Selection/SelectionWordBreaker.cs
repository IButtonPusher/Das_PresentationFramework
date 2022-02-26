using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Selection
{
    internal static class SelectionWordBreaker
    {
        internal static Boolean IsAtWordBoundary(Char[] text,
                                                 Int32 position,
                                                 LogicalDirection insideWordDirection)
        {
            var classes = GetClasses(text);
            if (insideWordDirection == LogicalDirection.Backward)
            {
                if (position == text.Length)
                    return true;
                if (position == 0 || IsWhiteSpace(text[position - 1], classes[position - 1]))
                    return false;
            }
            else
            {
                if (position == 0)
                    return true;
                if (position == text.Length || IsWhiteSpace(text[position], classes[position]))
                    return false;
            }

            throw new NotImplementedException();
            //ushort[] numArray = new ushort[2];
            //SafeNativeMethods.GetStringTypeEx(0U, 4U, new char[2]
            //{
            //  text[position - 1],
            //  text[position]
            //}, 2, numArray);
            //if (SelectionWordBreaker.IsWordBoundary(text[position - 1], text[position]))
            //  return true;
            //return !SelectionWordBreaker.IsSameClass(numArray[0], classes[position - 1], numArray[1], classes[position]) && !SelectionWordBreaker.IsMidLetter(text, position - 1, classes) && !SelectionWordBreaker.IsMidLetter(text, position, classes);
        }

        private static Boolean IsWordBoundary(Char previousChar,
                                              Char followingChar)
        {
            var flag = false;
            if (followingChar == '\r')
                flag = true;
            return flag;
        }

        private static Boolean IsMidLetter(Char[] text,
                                           Int32 index,
                                           CharClass[] classes)
        {
            Invariant.Assert(text.Length == classes.Length);
            if (text[index] != '\'' && text[index] != '’' && text[index] != '\u00AD' || index <= 0 ||
                index + 1 >= classes.Length)
                return false;
            if (classes[index - 1] == CharClass.Alphanumeric && classes[index + 1] == CharClass.Alphanumeric)
                return true;
            return text[index] == '"' && IsHebrew(text[index - 1]) && IsHebrew(text[index + 1]);
        }

        private static Boolean IsIdeographicCharType(UInt16 charType3)
        {
            return (charType3 & 304U) > 0U;
        }

        private static Boolean IsSameClass(UInt16 preceedingType3,
                                           CharClass preceedingClass,
                                           UInt16 followingType3,
                                           CharClass followingClass)
        {
            var flag = false;
            if (IsIdeographicCharType(preceedingType3) && IsIdeographicCharType(followingType3))
            {
                var num = (UInt16)((preceedingType3 & 496) ^ (followingType3 & 496));
                flag = (preceedingType3 & 240) != 0 && (num == 0 || num == 128 || num == 32 || num == 160);
            }
            else if (!IsIdeographicCharType(preceedingType3) && !IsIdeographicCharType(followingType3))
                flag = (preceedingClass & CharClass.WBF_CLASS) == (followingClass & CharClass.WBF_CLASS);

            return flag;
        }

        private static Boolean IsWhiteSpace(Char ch,
                                            CharClass charClass)
        {
            return (charClass & CharClass.WBF_CLASS) == CharClass.Blank && ch != '￼';
        }

        private static CharClass[] GetClasses(Char[] text)
        {
            var classes = new CharClass[text.Length];
            for (var index = 0; index < text.Length; ++index)
            {
                var ch = text[index];
                CharClass charClass;
                if (ch < 'Ā')
                    charClass = (CharClass)_latinClasses[ch];
                else if (IsKorean(ch))
                    charClass = CharClass.Alphanumeric;
                else if (IsThai(ch))
                    charClass = CharClass.Alphanumeric;
                else if (ch == '￼')
                {
                    charClass = CharClass.Blank | CharClass.WBF_BREAKAFTER;
                }
                else
                {
                    throw new NotImplementedException();
                    //ushort[] numArray = new ushort[1];
                    //SafeNativeMethods.GetStringTypeEx(0U, 1U, new char[1]
                    //{
                    //  ch
                    //}, 1, numArray);
                    //charClass = ((int) numArray[0] & 8) == 0 ? (((int) numArray[0] & 16) == 0 || SelectionWordBreaker.IsDiacriticOrKashida(ch) ? SelectionWordBreaker.CharClass.Alphanumeric : SelectionWordBreaker.CharClass.Punctuation) : (((int) numArray[0] & 64) == 0 ? SelectionWordBreaker.CharClass.WhiteSpace | SelectionWordBreaker.CharClass.WBF_ISWHITE : SelectionWordBreaker.CharClass.Blank | SelectionWordBreaker.CharClass.WBF_ISWHITE);
                }
                //classes[index] = charClass;
            }

            return classes;
        }

        private static Boolean IsDiacriticOrKashida(Char ch)
        {
            throw new NotImplementedException();
            //ushort[] numArray = new ushort[1];
            //SafeNativeMethods.GetStringTypeEx(0U, 4U, new char[1]
            //{
            //  ch
            //}, 1, numArray);
            //return ((uint) numArray[0] & 519U) > 0U;
        }

        private static Boolean IsInRange(UInt32 lower,
                                         Char ch,
                                         UInt32 upper)
        {
            return lower <= ch && ch <= upper;
        }

        private static Boolean IsKorean(Char ch)
        {
            return IsInRange(44032U, ch, 55295U);
        }

        private static Boolean IsThai(Char ch)
        {
            return IsInRange(3584U, ch, 3711U);
        }

        private static Boolean IsHebrew(Char ch)
        {
            return IsInRange(1488U, ch, 1522U);
        }

        internal static Int32 MinContextLength => 2;

        private const Char LineFeedChar = '\n';
        private const Char CarriageReturnChar = '\r';
        private const Char QuotationMarkChar = '"';
        private const Char ApostropheChar = '\'';
        private const Char SoftHyphenChar = '\u00AD';
        private const Char RightSingleQuotationChar = '’';
        private const Char ObjectReplacementChar = '￼';

        private static readonly Byte[] _latinClasses = new Byte[256]
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            20,
            0,
            19,
            20,
            20,
            20,
            20,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            50,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            65,
            1,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            1,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            18,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };

        [Flags]
        private enum CharClass : byte
        {
            Alphanumeric = 0,
            Punctuation = 1,
            Blank = 2,
            WhiteSpace = 4,
            WBF_CLASS = 15, // 0x0F
            WBF_ISWHITE = 16, // 0x10
            WBF_BREAKAFTER = 64, // 0x40
        }
    }
}
