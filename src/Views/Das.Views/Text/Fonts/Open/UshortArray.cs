using System;
using System.Threading.Tasks;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public class UshortArray : UshortBuffer
    {
        internal UshortArray(UInt16[] array)
        {
            _array = array;
        }

        internal UshortArray(Int32 capacity,
                             Int32 leap)
        {
            _array = new UInt16[capacity];
            _leap = leap;
        }

        public override UInt16[] ToArray()
        {
            return _array;
        }

        public override UInt16[] GetSubsetCopy(Int32 index,
                                               Int32 count)
        {
            var dst = new UInt16[count];
            Buffer.BlockCopy(_array, index * 2, dst, 0, (index + count <= _array.Length ? count : _array.Length) * 2);
            return dst;
        }

        public override void Insert(Int32 index,
                                    Int32 count,
                                    Int32 length)
        {
            var num = length + count;
            if (num > _array.Length)
            {
                Invariant.Assert(_leap > 0, "Growing an ungrowable list!");
                var dst = new UInt16[_array.Length + ((num - _array.Length - 1) / _leap + 1) * _leap];
                Buffer.BlockCopy(_array, 0, dst, 0, index * 2);
                if (index < length)
                    Buffer.BlockCopy(_array, index * 2, dst, (index + count) * 2, (length - index) * 2);
                _array = dst;
            }
            else
            {
                if (index >= length)
                    return;
                Buffer.BlockCopy(_array, index * 2, _array, (index + count) * 2, (length - index) * 2);
            }
        }

        public override void Remove(Int32 index,
                                    Int32 count,
                                    Int32 length)
        {
            Buffer.BlockCopy(_array, (index + count) * 2, _array, index * 2, (length - index - count) * 2);
        }

        public override UInt16 this[Int32 index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public override Int32 Length => _array.Length;

        private UInt16[] _array;
    }
}
