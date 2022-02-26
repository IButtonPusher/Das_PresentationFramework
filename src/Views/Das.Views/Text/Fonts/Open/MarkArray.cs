using System;
using System.Security;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    /// <SecurityNote>
    ///     Critical - Everything in this struct is considered critical
    ///     because they either operate on raw font table bits or unsafe pointers.
    /// </SecurityNote>
    [SecurityCritical(SecurityCriticalScope.Everything)]
    public struct MarkArray
    {
        private const Int32 offsetClassArray = 2;
        private const Int32 sizeClassRecord = 4;
        private const Int32 offsetClassRecordClass = 0;
        private const Int32 offsetClassRecordAnchor = 2;

        public UInt16 Class(FontTable Table,
                            UInt16 Index)
        {
            return Table.GetUShort(offset + offsetClassArray +
                                   Index * sizeClassRecord +
                                   offsetClassRecordClass);
        }

        public AnchorTable MarkAnchor(FontTable Table,
                                      UInt16 Index)
        {
            Int32 anchorTableOffset = Table.GetUShort(offset + offsetClassArray +
                                                      Index * sizeClassRecord +
                                                      offsetClassRecordAnchor
            );
            if (anchorTableOffset == 0)
            {
                return new AnchorTable(Table, 0);
            }

            return new AnchorTable(Table, offset + anchorTableOffset);
        }

        public MarkArray(Int32 Offset)
        {
            offset = Offset;
        }

        private readonly Int32 offset;
    }
}
