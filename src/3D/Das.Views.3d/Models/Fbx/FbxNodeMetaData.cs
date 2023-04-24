using System;
using System.IO;
using System.Text;

namespace Das.Views.Extended;

public readonly struct FbxNodeMetaData
{
   public FbxNodeMetaData(Int64 endOffset, 
                          Int64 numProperties, 
                          Int64 propertyListLen, 
                          Byte nameLen)
   {
      EndOffset = endOffset;
      NumProperties = numProperties;
      PropertyListLen = propertyListLen;
      NameLen = nameLen;
      IsEmpty = endOffset == 0;
   }

   public readonly Int64 EndOffset;
   public readonly Int64 NumProperties;
   public readonly Int64 PropertyListLen;
   public readonly Byte NameLen;
   public readonly Boolean IsEmpty;


   public Boolean IsValid() => !IsEmpty ||
                               NumProperties == 0 && PropertyListLen == 0 && NameLen == 0;

   public String GetName(BinaryReader reader)
   {
      return NameLen == 0 
         ? String.Empty 
         : Encoding.ASCII.GetString(reader.ReadBytes(NameLen));
   }

   public void SkipData(BinaryReader reader) => SkipData(reader.BaseStream);
        

   public void SkipData(Stream stream)
   {
      stream.Seek(EndOffset, SeekOrigin.Begin);
   }
}