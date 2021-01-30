using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Views.Images.Svg
{
    /// <summary>
    ///     https://github.com/svg-net
    /// </summary>
    [TypeConverter(typeof(SvgPathBuilder))]
    public sealed class SvgPathSegmentList : IList<SvgPathSegment>, ICloneable
    {
        public Object Clone()
        {
            var segments = new SvgPathSegmentList();
            foreach (var segment in this)
            {
                segments.Add(segment.Clone());
            }

            return segments;
        }

        public Int32 IndexOf(SvgPathSegment item)
        {
            return _segments.IndexOf(item);
        }

        public void Insert(Int32 index,
                           SvgPathSegment item)
        {
            _segments.Insert(index, item);
            if (Owner != null)
                Owner.OnPathUpdated();
        }

        public void RemoveAt(Int32 index)
        {
            _segments.RemoveAt(index);
            if (Owner != null)
                Owner.OnPathUpdated();
        }

        public SvgPathSegment this[Int32 index]
        {
            get => _segments[index];
            set
            {
                _segments[index] = value;
                if (Owner != null)
                    Owner.OnPathUpdated();
            }
        }

        public void Add(SvgPathSegment item)
        {
            _segments.Add(item);
            if (Owner != null)
                Owner.OnPathUpdated();
        }

        public void Clear()
        {
            _segments.Clear();
        }

        public Boolean Contains(SvgPathSegment item)
        {
            return _segments.Contains(item);
        }

        public void CopyTo(SvgPathSegment[] array,
                           Int32 arrayIndex)
        {
            _segments.CopyTo(array, arrayIndex);
        }

        public Int32 Count => _segments.Count;

        public Boolean IsReadOnly => false;

        public Boolean Remove(SvgPathSegment item)
        {
            var removed = _segments.Remove(item);

            if (removed && Owner != null)
                Owner.OnPathUpdated();

            return removed;
        }

        public IEnumerator<SvgPathSegment> GetEnumerator()
        {
            return _segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _segments.GetEnumerator();
        }

        public SvgPathSegment First => _segments[0];

        public SvgPathSegment Last => _segments[_segments.Count - 1];

        public ISvgPathElement? Owner { get; set; }

        public override String ToString()
        {
            return String.Join(" ", this.Select(p => p.ToString()).ToArray());
        }

        private readonly List<SvgPathSegment> _segments = new List<SvgPathSegment>();
    }

    public interface ISvgPathElement
    {
        void OnPathUpdated();
    }
}
