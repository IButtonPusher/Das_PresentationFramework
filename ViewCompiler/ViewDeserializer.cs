using System;
using System.Collections.Generic;
using Das;
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

        protected override T FromJsonCharArray<T>(IEnumerable<Char> json)
        {
            _state?.Dispose();

            _state = StateProvider.BorrowJson(Settings);
            
            var res = _state.Scanner.Deserialize<T>(json);
            RootNode = _state.Scanner.RootNode;
            return res;
        }

        public IEnumerable<Tuple<IStyle, IVisualElement>> GetStyles()
        {
            foreach (var style in GetStyles(RootNode))
                yield return style;
        }


        public void PostDeserialize()
        {
            PostDeserialize(RootNode);
        }

        private void PostDeserialize(ITextNode node)
        {
            switch (node.Value)
            {
                case IVisualElement visualElement:
                    var current = node.Parent;
                    if (current?.Value is IVisualContainer container)
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
            var styleType = GetTypeFromClearName(styleName);

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
