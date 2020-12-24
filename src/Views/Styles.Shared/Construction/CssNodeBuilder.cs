using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Construction
{
    public static class CssNodeBuilder
    {
        public static IEnumerable<CssMarkupNode> GetMarkupNodes(String css)
        {
            var i = 0;
            var sb = new StringBuilder();

            CssMarkupNode? currentNode;

            do
            {
                currentNode = GetNextNode(css, ref i, sb);
                if (currentNode != null)
                    yield return currentNode;
            } while (currentNode != null);

        }

        private static CssMarkupNode? GetNextNode(String css,
                                                  ref Int32 i,
                                                  StringBuilder sb)
        {
            for (; i < css.Length; i++)
            {
                var currentChar = css[i];

                switch (currentChar)
                {
                    case '/':
                        if (css[i + 1] == '*')
                        {
                            // comment

                            i++;
                            while (i < css.Length)
                            {
                                currentChar = css[++i];
                                if (currentChar == '*' && css[i + 1] == '/')
                                {
                                    i++;
                                    break;
                                }
                            }

                        }
                        break;
                    
                    
                    case '{':
                        var currentNode = new CssMarkupNode(sb.ToString().Trim(), null);
                        sb.Clear();
                        i++;
                        AddAttributes(css, ref i, sb, currentNode);
                        return currentNode;
                        
                    
                    case '}':
                        break;
                    
                    default:
                        sb.Append(currentChar);
                        break;
                }
            }

            return null;
        }

        private static void AddAttributes(String css,
                                          ref Int32 i,
                                          StringBuilder sb,
                                          CssMarkupNode currentNode)
        {
            for (; i < css.Length; i++)
            {
                var currentChar = css[i];

                switch (currentChar)
                {
                    case ':':
                        var key = sb.ToString();
                        
                        sb.Clear();
                        i++;
                        
                        var value = GetAttributeValue(css, ref i, sb);
                        if (value != null)
                            currentNode.AddAttribute(key, value);
                        break;
                    
                    case '}':
                        return;
                    
                    case ';':
                        throw new NotImplementedException();
                    
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        break;
                    
                    default:
                        sb.Append(currentChar);
                        break;
                }
                
            }
        }

        private static String? GetAttributeValue(String css,
                                                 ref Int32 i,
                                                 StringBuilder sb)
        {
            var singleQuoteCounter = 0;
            var doubleQuoteCounter = 0;
            
            var outerQuote = '\t';
            var curlyIndex = 0;

            for (; i < css.Length; i++)
            {
                var next = css[i];

                var amInQuote = outerQuote == '\''
                    ? singleQuoteCounter % 2 != 0
                    : doubleQuoteCounter % 2 != 0;
                
                switch (next)
                {
                    case '"':
                        doubleQuoteCounter++;
                        sb.Append(next);

                        if (!amInQuote)
                            outerQuote = '"';

                        break;

                    case '\'':
                        singleQuoteCounter++;
                        sb.Append(next);

                        if (!amInQuote)
                            outerQuote = '\'';

                        break;


                    case ';':

                        if (amInQuote || curlyIndex > 0)
                            goto default;
                        else goto setAttributeValue;

                    case '{':
                        curlyIndex++;
                        goto default;

                    case '}':
                        curlyIndex--;
                        goto default;

                    default:
                        sb.Append(next);

                        break;
                }
            }
            
            setAttributeValue:
            if (sb.Length == 0)
                return null;

            var val = sb.ToString().Trim();
            sb.Clear();
            return val;
        }
        
    }
}
