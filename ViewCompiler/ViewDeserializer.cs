using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Das.Serializer;
using Das.Views.DevKit;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace ViewCompiler
{
    public class ViewDeserializer : DasSerializer, IViewDeserializer
    {
        public ViewDeserializer(ISerializerSettings settings) : base(settings) { }

        public ITextNode RootNode { get; private set; }

        private IJsonLoaner _state;

        //public override T FromJson<T>(Stream stream)
        //{
        //    return ((DasCoreSerializer) this).FromJson<T>(stream);
        //}

        protected override T FromJsonCharArray<T>(Char[] json)
        {
            _state?.Dispose();

            _state = StateProvider.BorrowJson(Settings);

            var res = _state.Scanner.Deserialize<T>(json);
            RootNode = _state.Scanner.RootNode;
            return res;
        }

        public override T FromJson<T>(Stream stream)
        {  
            var len = (Int32)stream.Length;
            Byte[] buffer;

            using(var memoryStream = new MemoryStream(len))
            {
                stream.CopyTo(memoryStream);
                buffer = memoryStream.ToArray();
            }

            var encoding = GetEncoding(buffer);
            var charArr = encoding.GetChars(buffer, 0, len);

            return FromJsonCharArray<T>(charArr);
            //    return ((DasCoreSerializer) this).FromJson<T>(stream);
        }

        public IEnumerable<Tuple<IStyle, IVisualElement>> GetStyles()
        {
            foreach (var style in GetStyles(RootNode))
                yield return style;
        }

        private static Encoding GetEncoding(Byte[] bom)
        {
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }


        public void PostDeserialize()
        {
            PostDeserialize(RootNode);
        }

        private static void PostDeserialize(ITextNode node)
        {
            switch (node.Value)
            {
                case IVisualElement visualElement:
                    var current = node.Parent;
                    if (current != NullNode.Instance &&  current?.Value is IVisualContainer container)
                    {
                        if (container.Children.Contains(visualElement))
                            container.OnChildDeserialized(visualElement, node);

                        break;
                    }
                    else if (current == null)
                        break;
                    else current = current.Parent;
                    break;
                default:
                    break;
            }

            foreach (var kvp in node.Children.Values)
                PostDeserialize(kvp);
        }

        public IEnumerable<Tuple<IStyle, IVisualElement>> GetStyles(ITextNode node)
        {
            if (node.Attributes.TryGetValue(nameof(Style), out var styleName))
            {
                switch (node.Value)
                {
                    case IVisualElement visualElement:
                        if (TryGetElementStyle(visualElement, styleName, out var found))
                            yield return new Tuple<IStyle, IVisualElement>(found, visualElement);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            foreach (var kvp in node.Children.Values)
            {
                foreach (var style in GetStyles(kvp))
                    yield return style;
            }
        }

        private Boolean TryGetElementStyle(IVisualElement element, String styleName,
            out IStyle style)
        {
            var styleType = TypeInferrer.GetTypeFromClearName(styleName);

            if (typeof(ElementStyle).IsAssignableFrom(styleType))
            {
                var ctorToUse = styleType.GetConstructor(
                    new [] { typeof(IVisualElement) });
                if (ctorToUse == null)
                    throw new InvalidOperationException($"Style '{styleName}' does not have a valid constructor that accepts a single parameter of type: {nameof(IVisualElement)}.");

                style = (ElementStyle)Activator.CreateInstance(styleType, element);
                return true;
            }
            if (typeof(TypeStyle).IsAssignableFrom(styleType))
            {
                style = (TypeStyle)Activator.CreateInstance(styleType);
                return true;
            }

            style = default;
            return false;
        }
    }
}
