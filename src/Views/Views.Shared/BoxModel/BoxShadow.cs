using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Views.BoxModel
{
    public class BoxShadow : IBoxShadow
    {
        public BoxShadow(IEnumerable<IBoxShadowLayer> layers)
        {
            _layers = new List<IBoxShadowLayer>(layers);
            IsEmpty = _layers.Count == 0;
        }

        public IEnumerator<IBoxShadowLayer> GetEnumerator()
        {
            return _layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Boolean IsEmpty { get; }

        public static readonly BoxShadow Empty = new BoxShadow(Enumerable.Empty<IBoxShadowLayer>());
        private readonly List<IBoxShadowLayer> _layers;
    }
}