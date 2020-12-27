using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Styles;

namespace Das.Views.DevKit
{
    public interface IViewDeserializer : ISerializationCore
    {
        IEnumerable<Tuple<IStyle, IVisualElement>> GetStyles();

        Task<T> FromJsonAsync<T>(Stream stream);
    }
}