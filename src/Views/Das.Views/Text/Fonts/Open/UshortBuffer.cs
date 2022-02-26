using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public abstract class UshortBuffer
    {
        public virtual UInt16[] ToArray()
        {
            return null;
        }

        public virtual UInt16[] GetSubsetCopy(Int32 index,
                                              Int32 count)
        {
            return null;
        }

        public virtual void Insert(Int32 index,
                                   Int32 count,
                                   Int32 length)
        {
        }

        public virtual void Remove(Int32 index,
                                   Int32 count,
                                   Int32 length)
        {
        }

        public abstract UInt16 this[Int32 index] { get; set; }

        public abstract Int32 Length { get; }

        protected Int32 _leap;
    }
}
