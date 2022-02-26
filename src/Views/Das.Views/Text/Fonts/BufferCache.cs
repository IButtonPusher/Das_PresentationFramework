using System;
using System.Threading;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public static class BufferCache
    {
        internal static void Reset()
        {
            if (Interlocked.Increment(ref _mutex) == 1L)
                _buffers = null;
            Interlocked.Decrement(ref _mutex);
        }

        internal static GlyphMetrics[] GetGlyphMetrics(Int32 length)
        {
            return (GlyphMetrics[])GetBuffer(length, 0) ?? new GlyphMetrics[length];
        }

        internal static void ReleaseGlyphMetrics(GlyphMetrics[] glyphMetrics)
        {
            ReleaseBuffer(glyphMetrics, 0);
        }

        internal static UInt16[] GetUShorts(Int32 length)
        {
            return (UInt16[])GetBuffer(length, 2) ?? new UInt16[length];
        }

        internal static void ReleaseUShorts(UInt16[] ushorts)
        {
            ReleaseBuffer(ushorts, 2);
        }

        internal static UInt32[] GetUInts(Int32 length)
        {
            return (UInt32[])GetBuffer(length, 1) ?? new UInt32[length];
        }

        internal static void ReleaseUInts(UInt32[] uints)
        {
            ReleaseBuffer(uints, 1);
        }

        private static Array GetBuffer(Int32 length,
                                       Int32 index)
        {
            Array buffer = null;
            if (Interlocked.Increment(ref _mutex) == 1L && _buffers != null && _buffers[index] != null &&
                length <= _buffers[index].Length)
            {
                buffer = _buffers[index];
                _buffers[index] = null;
            }

            Interlocked.Decrement(ref _mutex);
            return buffer;
        }

        private static void ReleaseBuffer(Array buffer,
                                          Int32 index)
        {
            if (buffer == null)
                return;
            if (Interlocked.Increment(ref _mutex) == 1L)
            {
                if (_buffers == null)
                    _buffers = new Array[3];
                if (_buffers[index] == null || _buffers[index].Length < buffer.Length && buffer.Length <= 1024)
                    _buffers[index] = buffer;
            }

            Interlocked.Decrement(ref _mutex);
        }

        private const Int32 MaxBufferLength = 1024;
        private const Int32 GlyphMetricsIndex = 0;
        private const Int32 UIntsIndex = 1;
        private const Int32 UShortsIndex = 2;
        private const Int32 BuffersLength = 3;
        private static Int64 _mutex;
        private static Array[] _buffers;
    }
}
