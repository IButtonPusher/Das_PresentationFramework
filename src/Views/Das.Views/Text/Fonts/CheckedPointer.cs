using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct CheckedPointer
    {
        private unsafe void* _pointer;
        private Int32 _size;

        public unsafe CheckedPointer(void* pointer,
                                     Int32 size)
        {
            _pointer = pointer;
            _size = size;
        }

        public unsafe CheckedPointer(UnmanagedMemoryStream stream)
        {
            _pointer = stream.PositionPointer;
            var length = stream.Length;
            _size = length >= 0L && length <= Int32.MaxValue ? (Int32)length : throw new ArgumentOutOfRangeException();
        }

        public unsafe Boolean IsNull => (IntPtr)_pointer == IntPtr.Zero;

        public Int32 Size => _size;

        public unsafe Byte[] ToArray()
        {
            var destination = new Byte[_size];
            if ((IntPtr)_pointer == IntPtr.Zero)
                throw new ArgumentOutOfRangeException();
            Marshal.Copy((IntPtr)_pointer, destination, 0, Size);
            return destination;
        }

        public unsafe void CopyTo(CheckedPointer dest)
        {
            if ((IntPtr)_pointer == IntPtr.Zero)
                throw new ArgumentOutOfRangeException();
            var pointer = (Byte*)_pointer;
            var numPtr = (Byte*)dest.Probe(0, _size);
            for (var index = 0; index < _size; ++index)
                numPtr[index] = pointer[index];
        }

        public unsafe Int32 OffsetOf(void* pointer)
        {
            var num = (SByte*)pointer - (SByte*)_pointer;
            if (num < 0L || num > _size || (IntPtr)_pointer == IntPtr.Zero || (IntPtr)pointer == IntPtr.Zero)
                throw new ArgumentOutOfRangeException();
            return (Int32)num;
        }

        public unsafe Int32 OffsetOf(CheckedPointer pointer)
        {
            return OffsetOf(pointer._pointer);
        }

        public static unsafe CheckedPointer operator +(CheckedPointer rhs,
                                                       Int32 offset)
        {
            if (offset < 0 || offset > rhs._size || (IntPtr)rhs._pointer == IntPtr.Zero)
                throw new ArgumentOutOfRangeException();
            rhs._pointer = (Byte*)rhs._pointer + offset;
            rhs._size -= offset;
            return rhs;
        }

        public unsafe void* Probe(Int32 offset,
                                  Int32 length)
        {
            if ((IntPtr)_pointer == IntPtr.Zero || offset < 0 || offset > _size || offset + length > _size ||
                offset + length < 0)
                throw new ArgumentOutOfRangeException();
            return (void*)((IntPtr)_pointer + offset);
        }

        public unsafe CheckedPointer CheckedProbe(Int32 offset,
                                                  Int32 length)
        {
            if ((IntPtr)_pointer == IntPtr.Zero || offset < 0 || offset > _size || offset + length > _size ||
                offset + length < 0)
                throw new ArgumentOutOfRangeException();
            return new CheckedPointer((void*)((IntPtr)_pointer + offset), length);
        }

        public void SetSize(Int32 newSize)
        {
            _size = newSize;
        }

        public unsafe Boolean PointerEquals(CheckedPointer pointer)
        {
            return _pointer == pointer._pointer;
        }

        public unsafe void WriteBool(Boolean value)
        {
            *(Boolean*)Probe(0, sizeof(Boolean)) = value;
        }

        public unsafe Boolean ReadBool()
        {
            return *(Boolean*)Probe(0, sizeof(Boolean));
        }
    }
}
