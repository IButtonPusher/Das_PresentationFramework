using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Construction
{
    public abstract class MarkupNode : IMarkupNode
    {
        protected MarkupNode(String name,
                             MarkupNode? parent, 
                             Boolean isEncodingHeader,
                             MarkupLanguage language)
        {
            Name = name;
            Children = new List<MarkupNode>();
            _attributes = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            Parent = parent;
            IsEncodingHeader = isEncodingHeader;
            Language = language;

            CanHaveChildren = true;
            parent?.Children.Add(this);
        }

        public override String ToString()
        {
            return _aboutMe ??= BuildToString();
        }

        private String BuildToString()
        {
            var str = Name + " children: " + ChildrenCount;

            if (_attributes.Count == 0)
                return str;

            var sb = new StringBuilder(str);

            sb.Append(' ');
            sb.Append('(');
            foreach (var kvp in _attributes)
            {
                sb.Append(kvp.Key);
                sb.Append('=');
                sb.Append(kvp.Value);
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(')');
            return sb.ToString();

        }

        public Boolean IsEncodingHeader { get; }

        public IMarkupNode this[Int32 index] => Children[index];

        public Int32 ChildrenCount => Children.Count;

        public String Name { get; }

        public MarkupLanguage Language { get; }

        IEnumerable<IMarkupNode> IMarkupNode.Children => Children;

        public Boolean TryGetAttributeValue(String key, 
                                            out String value)
        {
            return _attributes.TryGetValue(key, out value);
        }

        public IEnumerable<KeyValuePair<String, String>> GetAllAttributes()
        {
            return _attributes;
        }

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
            _aboutMe = null;
        }

        public Boolean CanHaveChildren { get; }

        private readonly Dictionary<String, String> _attributes;
        private String? _aboutMe;
    }
}
