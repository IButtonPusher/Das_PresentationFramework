using System;
using System.Threading.Tasks;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public class UshortList
    {
        public UshortList(Int32 capacity,
                          Int32 leap)
        {
            Invariant.Assert(capacity >= 0 && leap >= 0, "Invalid parameter");
            _storage = new UshortArray(capacity, leap);
        }

        public UshortList(UInt16[] array)
        {
            Invariant.Assert(array != null, "Invalid parameter");
            _storage = new UshortArray(array);
        }

        public UshortList(CheckedUShortPointer unsafeArray,
                          Int32 arrayLength)
        {
            _storage = new UnsafeUshortArray(unsafeArray, arrayLength);
            _length = arrayLength;
        }

        public void SetRange(Int32 index,
                             Int32 length)
        {
            Invariant.Assert(length >= 0 && index + length <= _storage.Length, "List out of storage");
            _index = index;
            _length = length;
        }

        public void Insert(Int32 index,
                           Int32 count)
        {
            Invariant.Assert(index <= _length && index >= 0, "Index out of range");
            Invariant.Assert(count > 0, "Invalid argument");
            _storage.Insert(_index + index, count, _index + _length);
            _length += count;
        }

        public void Remove(Int32 index,
                           Int32 count)
        {
            Invariant.Assert(index < _length && index >= 0, "Index out of range");
            Invariant.Assert(count > 0 && index + count <= _length, "Invalid argument");
            _storage.Remove(_index + index, count, _index + _length);
            _length -= count;
        }

        public UInt16[] ToArray()
        {
            return _storage.ToArray();
        }

        public UInt16[] GetCopy()
        {
            return _storage.GetSubsetCopy(_index, _length);
        }

        public UInt16 this[Int32 index]
        {
            get
            {
                Invariant.Assert(index >= 0 && index < _length, "Index out of range");
                return _storage[_index + index];
            }
            set
            {
                Invariant.Assert(index >= 0 && index < _length, "Index out of range");
                _storage[_index + index] = value;
            }
        }

        public Int32 Length
        {
            get => _length;
            set => _length = value;
        }

        public Int32 Offset => _index;

        private readonly UshortBuffer _storage;

        private Int32 _index;
        private Int32 _length;
    }
}
