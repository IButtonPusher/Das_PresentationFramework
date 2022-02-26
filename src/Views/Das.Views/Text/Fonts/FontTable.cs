using System;
using System.Threading.Tasks;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts
{
    public class FontTable
    {
        public FontTable(Byte[] data)
        {
            m_data = data;
            if (data != null)
                m_length = (UInt32)data.Length;
            else
                m_length = 0U;
        }

        public UInt16 GetUShort(Int32 offset)
        {
            Invariant.Assert(m_data != null);
            if (offset + 1 >= m_length)
                throw new FormatException();
            return (UInt16)(((UInt32)m_data[offset] << 8) + m_data[offset + 1]);
        }

        public Int16 GetShort(Int32 offset)
        {
            Invariant.Assert(m_data != null);
            if (offset + 1 >= m_length)
                throw new FormatException();
            return (Int16)((m_data[offset] << 8) + m_data[offset + 1]);
        }

        public UInt32 GetUInt(Int32 offset)
        {
            Invariant.Assert(m_data != null);
            if (offset + 3 >= m_length)
                throw new FormatException();
            return (UInt32)((m_data[offset] << 24) + (m_data[offset + 1] << 16) + (m_data[offset + 2] << 8)) +
                   m_data[offset + 3];
        }

        public UInt16 GetOffset(Int32 offset)
        {
            Invariant.Assert(m_data != null);
            if (offset + 1 >= m_length)
                throw new FormatException();
            return (UInt16)(((UInt32)m_data[offset] << 8) + m_data[offset + 1]);
        }

        public Boolean IsPresent => m_data != null;

        public const Int32 InvalidOffset = 2147483647;
        public const Int32 NullOffset = 0;
        private readonly Byte[] m_data;
        private readonly UInt32 m_length;
    }
}
