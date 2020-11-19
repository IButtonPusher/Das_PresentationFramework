using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.DataBinding;
// ReSharper disable All

namespace Das.Views
{
    public class ViewInflater
    {
        public IBindableElement<T> Inflate<T>(String xml)
        {
            var tag = GetHtmlTag(xml, false);
            throw new NotImplementedException();
        }

        public static MarkupNode? GetHtmlTag(String html,
                                             Boolean isSkipScripts)
        {
            var sbOuter = new StringBuilder();
            var sbInner = new StringBuilder();
            MarkupNode? currentTag = null;

            var i = 0;

            var str = html.Trim();

            for (; i < str.Length; i++)
            {
                var current = str[i];

                switch (current)
                {
                    case '<':

                        // open/close tag

                        if (i >= str.Length - 1)
                            break;

                        var next = str[++i];

                        if (next == '/')
                        {
                            if (sbOuter.Length > 0)
                            {
                                if (currentTag != null)
                                {
                                    currentTag.OuterText = sbOuter.ToString();
                                    currentTag.InnerText = sbInner.ToString();
                                }

                                sbOuter.Clear();
                                sbInner.Clear();
                            }

                            for (i++; i < str.Length; i++)
                            {
                                next = str[i];

                                if (str[i] == '>')
                                {
                                    var closingTag = sbOuter.ToString();
                                    sbOuter.Clear();
                                    sbInner.Clear();

                                    while (currentTag?.Parent != null
                                           && currentTag.Name != closingTag)
                                    {
                                        currentTag = currentTag.Parent;
                                    }


                                    sbOuter.Clear();
                                    sbInner.Clear();

                                    currentTag = currentTag?.GetParentOrSelf();

                                    break;
                                }

                                sbOuter.Append(next);
                                sbInner.Append(next);
                            }
                        }
                        else if (next == '!' && str[i + 1] == '-' && str[i + 2] == '-')
                        {
                            SkipCommentedLine(ref i, str);
                        }
                        else
                        {
                            // p isn't the only tag that can use inner markup
                            if (currentTag?.Name == "p") //|| sbInner.Length > 0)
                            {
                                i--;

                                AppendParagraphInlineMarkup(ref i, str,
                                    sbOuter, sbInner, currentTag);
                                break;
                            }
                            else if (sbInner.Length > 0 && currentTag != null)
                            {
                                currentTag.InnerText ??= String.Empty;

                                currentTag.InnerText += sbInner.ToString();

                                sbInner.Clear();
                                sbOuter.Clear();

                            }

                            // opening tag
                            GetTag(ref i, str, sbOuter, sbInner, ref currentTag, isSkipScripts);
                        }


                        break;


                    case ' ':
                        if (sbOuter.Length > 0)
                        {
                            sbOuter.Append(' ');
                            sbInner.Append(' ');
                        }

                        break;

                    case '\n':
                    case '\r':
                    case '\t':
                        break;

                    default:
                        sbOuter.Append(current);
                        sbInner.Append(current);


                        break;
                }
            }

            while (currentTag?.Parent != null)
                currentTag = currentTag.Parent;


            return currentTag;
        }

        private static void SkipCommentedLine(ref Int32 i, String str)
        {
            for (i++; i < str.Length - 5; i++)
                if (str[i] != '!' &&
                    str[i + 1] == '-' &&
                    str[++i] == '-' &&
                    str[i + 2] == '>')
                {
                    i += 2;
                    return;
                }
        }

        private static void AppendParagraphInlineMarkup(ref Int32 i,
                                                        String str,
                                                        StringBuilder sbOuter,
                                                        StringBuilder sbInner,
                                                        MarkupNode currentTag)
        {
            var tagStack = 0;
            var tagIndex = 0;
            var isSingleQuoteOpen = false;
            var isDoubleQuoteOpen = false;
            var startedAt = i;

            for (; i < str.Length; i++)
            {
                var current = str[i];
                sbOuter.Append(current);

                if (tagIndex == 0 && current != '<')
                    sbInner.Append(current);

                var isSomeQuoteOpen = isDoubleQuoteOpen || isSingleQuoteOpen;

                switch (current)
                {
                    case '<' when !isSomeQuoteOpen:
                        tagIndex++;

                        current = str[++i];
                        sbOuter.Append(current);
                        //sbInner.Append(current);
                        if (current == '/')
                        {
                            tagStack--;
                        }
                        else
                        {
                            tagStack++;
                            currentTag.TextTagsCount++;
                        }

                        break;

                    case '"':

                        if (tagIndex > 0)
                            isDoubleQuoteOpen = !isDoubleQuoteOpen;
                        break;

                    case '\'':
                        if (tagIndex > 0)
                            isSingleQuoteOpen = !isSingleQuoteOpen;
                        break;

                    case '>' when !isSomeQuoteOpen:
                        tagIndex--;

                        if (tagIndex == 0 && tagStack == 0)
                        {
                            currentTag.TextTagsLength += i - startedAt;
                            return;
                        }

                        break;

                    case '/' when !isSomeQuoteOpen:
                        tagStack--;
                        break;
                }
            }
        }

        private static void GetTag(ref Int32 i,
                                   String str,
                                   StringBuilder sbOuter,
                                   StringBuilder sbInner,
                                   ref MarkupNode? currentTag,
                                   Boolean isSkipScripts)
        {
            if (!isSkipScripts)
            {
            }

            var next = str[i];

            sbOuter.Append(next);
            sbInner.Append(next);

            for (i++; i < str.Length; i++)
            {
                next = str[i];

                if (next == ' ')
                {
                    //finished reading the tag name
                    currentTag = new MarkupNode(sbOuter.ToString(), currentTag);
                    sbOuter.Clear();
                    sbInner.Clear();

                    BuildAttributes(ref i, str, sbOuter, sbInner, ref currentTag);

                    if (currentTag?.CanHaveChildren == false)
                        currentTag = currentTag.GetParentOrSelf();

                    break;
                }

                if (next == '/')
                {
                    // self closing tag with no attributes
                    next = str[++i];
                }

                if (next == '>')
                {
                    // tag with no attributes
                    currentTag = new MarkupNode(sbOuter.ToString(), currentTag);
                    sbOuter.Clear();
                    sbInner.Clear();

                    if (currentTag?.CanHaveChildren == false)
                        currentTag = currentTag?.GetParentOrSelf();

                    break;
                }

                sbOuter.Append(next);
                sbInner.Append(next);
            }
        }


        private static void BuildAttributes(ref Int32 i,
                                            String str,
                                            StringBuilder sbOuter,
                                            StringBuilder sbInner,
                                            ref MarkupNode currentTag)
        {
            // tag identified.  Extract attributes

            for (i++; i < str.Length; i++)
            {
                var next = str[i];

                if (next == '=')
                {
                    BuildAttributeValue(ref i, str, sbOuter, sbInner, currentTag);
                }
                else if (next == '>')
                {
                    if (sbOuter.Length > 0)
                    {
                        // a key with no value...
                        //currentTag.Attributes[sbOuter.ToString()] = String.Empty;
                        currentTag.AddAttribute(sbOuter.ToString(), String.Empty);
                        sbOuter.Clear();
                        sbInner.Clear();
                    }

                    if (!currentTag.CanHaveChildren)
                        currentTag = currentTag.GetParentOrSelf();

                    break;
                }
                else if (next == '/')
                {
                    // self closing

                    if (sbOuter.Length > 0)
                    {
                        // a key with no value...
                        currentTag.AddAttribute(sbOuter.ToString(), String.Empty);
                        //currentTag.Attributes[sbOuter.ToString()] = String.Empty;
                        sbOuter.Clear();
                        sbInner.Clear();
                    }

                    if (str[++i] != '>')
                    {
                        //awkward..
                    }

                    currentTag = currentTag.Parent!;
                    break;
                }
                else
                {
                    if (Char.IsWhiteSpace(next))
                    {
                        continue;
                    }

                    sbOuter.Append(next);
                    sbInner.Append(next);
                }
            }
        }

        private static void BuildAttributeValue(ref Int32 i,
                                                String str,
                                                StringBuilder sbOuter,
                                                StringBuilder sbInner,
                                                MarkupNode currentTag)
        {
            var key = sbOuter.ToString().Trim();

            sbOuter.Clear();
            sbInner.Clear();

            var next = str[++i];

            var singleQuoteCounter = 0;
            var doubleQuoteCounter = 0;

            Char outerQuote;

            if (next == '"')
            {
                doubleQuoteCounter++;
                outerQuote = '"';
            }
            else if (next == '\'')
            {
                outerQuote = '\'';
                singleQuoteCounter++;
            }

            else
            {
                // something weird going on with this "attribute"

                for (i++; i < str.Length; i++)
                    if (str[i] == '>')
                        break;

                return;
            }

            // got key, get value
            var curlyIndex = 0;

            for (i++; i < str.Length; i++)
            {
                next = str[i];

                var amInQuote = outerQuote == '\''
                    ? singleQuoteCounter % 2 != 0
                    : doubleQuoteCounter % 2 != 0;

                switch (next)
                {
                    case '"':
                        doubleQuoteCounter++;
                        sbOuter.Append(next);
                        sbInner.Append(next);

                        if (amInQuote && outerQuote == '"')
                        {

                        }


                        if (!amInQuote)
                            outerQuote = '"';

                        break;

                    case '\'':
                        singleQuoteCounter++;
                        sbOuter.Append(next);
                        sbInner.Append(next);

                        if (amInQuote && outerQuote == '\'')
                        {

                        }

                        if (!amInQuote)
                            outerQuote = '\'';

                        break;

                    case ' ':
                    case '\r':
                    case '\n':
                        if (amInQuote || curlyIndex > 0)
                            goto default;

                        else goto setAttributeValue;

                    case '/':
                        if (amInQuote || curlyIndex > 0)
                            goto default;
                        else goto setAttributeValue;

                    case '>':

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
                        sbOuter.Append(next);
                        sbInner.Append(next);

                        break;
                }
            }

            setAttributeValue:
            if (sbOuter.Length == 0)
                return;

            sbOuter.Remove(sbOuter.Length - 1, 1); //remove quote

            var val = sbOuter.ToString();

            currentTag.AddAttribute(key, val);
            sbOuter.Clear();
            sbInner.Clear();

            if (!currentTag.CanHaveChildren)
            {
            }

            if (next == '/' || next == '>')
                i--;
            //i++;
        }
    }

    public class MarkupNode
    {
        public MarkupNode(String name,
                          MarkupNode? parent)
        {
            Name = name;
            Children = new List<MarkupNode>();
            _attributes = new Dictionary<String, String>();
            Parent = parent;

            CanHaveChildren = true;
        }

        public String Name { get; }

        public List<MarkupNode> Children { get; }

        public MarkupNode? Parent { get; }

        public String? OuterText { get; set; }

        public String? InnerText { get; set; }

        public Int32 TextTagsCount { get; set; }

        /// <summary>
        ///     The total characters in the IntterText property that are hyperlinks/images etc
        /// </summary>
        public Int32 TextTagsLength { get; set; }

        public MarkupNode GetParentOrSelf()
        {
            return Parent ?? this;
        }

        public void AddAttribute(String key,
                                 String value)
        {
            _attributes[key] = value;
        }

        public Boolean CanHaveChildren { get; }

        private Dictionary<String, String> _attributes;
    }
}
