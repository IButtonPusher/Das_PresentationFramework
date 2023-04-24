using Das.Views.Extended.Core;
using Das.Views.Extended.Models.Fbx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Triangulation;

namespace Das.Views.Extended;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CoreFbxLoader
{
   // Header string, found at the top of all compliant files
   private static readonly Byte[] _standardHeader;
   private static readonly Int32 _standardHeaderLength;

   private const Byte _Single = 70;
   private const Byte _Int16 = 89;
   private const Byte _Int32 = 73;
   private const Byte _Int64 = 76;
   private const Byte _String = 83;
   private const Byte _ByteArray = 82;
   private const Byte _Double = 68;
        
   private const Byte _BooleanArray = 98;
   private const Byte _SingleArray = 102;
   private const Byte _DoubleArray = 100;
   private const Byte _IntegerArray = 105;
        

   private const Byte _Char = 67;

   static CoreFbxLoader()
   {
      _standardHeader = Encoding.ASCII.GetBytes("Kaydara FBX Binary  \0\x1a\0");
      _standardHeaderLength = _standardHeader.Length;
   }

   public async Task<Core3dModel> LoadModelAsync(FileInfo file)
   {
      using (var stream = file.OpenRead())
      {
         return await LoadModelAsync(stream).ConfigureAwait(false);
      }
   }

   private static FbxNodeMetaData GetNodeMeta(BinaryReader reader, Double fbxVersion)
   {
      return fbxVersion >= 7.5
         ? GetNodeMeta64(reader)
         : GetNodeMeta32(reader);
   }

   private static FbxNodeMetaData GetNodeMeta32(BinaryReader reader)
   {
      var data = new FbxNodeMetaData(
         reader.ReadInt32(),
         reader.ReadInt32(),
         reader.ReadInt32(),
         reader.ReadByte());

      if (!data.IsValid())
         throw new Exception();

      return data;
   }

   private static FbxNodeMetaData GetNodeMeta64(BinaryReader reader)
   {
      var data = new FbxNodeMetaData(
         reader.ReadInt64(),
         reader.ReadInt64(),
         reader.ReadInt64(),
         reader.ReadByte());

      if (!data.IsValid())
         throw new Exception();

      return data;
   }

   public async Task<Core3dModel> LoadModelAsync(Stream stream)
   {
      await CheckHeaderAsync(stream).ConfigureAwait(false);

      using (var reader = new BinaryReader(stream))
      {
         var fbxVersion = reader.ReadInt32() / 1000.0;

         while (stream.Position < stream.Length)
         {
            var meta = GetNodeMeta(reader, fbxVersion);

            if (meta.IsEmpty)
               continue;

            var name = meta.GetName(reader);

            switch (name)
            {
               case "Objects":
                  var meshes = GetMeshes(reader, fbxVersion);
                  var mofo = new Core3dModel(Vector3.Zero, Vector3.Zero, meshes);
                  return mofo;

               default:
                  meta.SkipData(reader);
                  break;
            }
         }
      }

      throw new NotImplementedException();
   }

   private static IEnumerable<NamedMesh> GetMeshes(BinaryReader reader, 
                                                   Double fbxVersion)
   {
      var namedMeshes = new Dictionary<String, NamedMesh>();
            

      while (true)
      {
         var meta = GetNodeMeta(reader, fbxVersion);

         if (meta.IsEmpty)
            break;

         var nomnom = meta.GetName(reader);

         switch (nomnom)
         {
            case "Geometry":
               if (TryGetNamedMesh(reader, fbxVersion, meta, out var namedMesh))
               {
                  namedMeshes.Add(namedMesh.Name, namedMesh);
               }

               break;

            case "Model":
               var name = GetNameFromProperties(reader, meta);
               var xform = GetModelTransformation(reader, fbxVersion);

                       

               if (namedMeshes.TryGetValue(name, out var m))
               {
                  //if (m.Name == "Cylinder.003")
                  {
                     m.Transformation = xform;
                     //m.Transform(xform);
                     yield return m;
                  }

                  //  m.Transform(xform);
               }

               goto default;

            default:
               meta.SkipData(reader);
               break;
         }
      }

      //return namedMeshes.Values;
   }

   private static Transformation3D GetModelTransformation(BinaryReader reader,
                                                          Double fbxVersion)
   {
      var positionOffset = new ValueVector3(0, 0, 0);
      var rotation = new ValueVector3(0, 0, 0);
      var scale = new ValueVector3(1, 1, 1);

      while (true)
      {
         var meta = GetNodeMeta(reader, fbxVersion);

         if (meta.IsEmpty)
            break;

         var nomnom = meta.GetName(reader);

         switch (nomnom)
         {
            case "Properties70":
               while (true)
               {
                  var childMeta = GetNodeMeta(reader, fbxVersion);
                  if (childMeta.IsEmpty)
                     break;

                  //skip 'P'
                  reader.BaseStream.Seek(childMeta.NameLen, SeekOrigin.Current);

                  var modelProps = GetPropertyValues(reader, childMeta);
                  if (modelProps.Length < 7)
                     continue;

                  switch (modelProps[0])
                  {
                     case "Lcl Translation":
                        positionOffset = new ValueVector3(
                           (Double)modelProps[4] / 100,
                           (Double)modelProps[5] / 100,
                           (Double)modelProps[6] / 100);
                        break;

                     case "Lcl Rotation":
                        rotation = new ValueVector3((Double)modelProps[4],
                           (Double)modelProps[5],
                           (Double)modelProps[6]);
                        break; 

                     case "Lcl Scaling":
                        scale = new ValueVector3(
                           (Double)modelProps[4] / 100,
                           (Double)modelProps[5] / 100,
                           (Double)modelProps[6] / 100);
                        break; 
                  }

               }

               break;


            default:
               meta.SkipData(reader);
               break;
         }
      }

      return new Transformation3D(positionOffset, rotation, scale);
   }

   private static Object[] GetPropertyValues(BinaryReader reader,
                                             FbxNodeMetaData meta)
   {
      var res = new Object[meta.NumProperties];

      for (var c = 0; c < meta.NumProperties; c++)
      {
         var b = reader.ReadByte();

         switch (b)
         {
            case _Double:
               res[c] = reader.ReadDouble();
               break;

            case _Int64:
               res[c] = reader.ReadInt64();
               break;

            case _Int32:
               res[c] = reader.ReadInt32();
               break;

            case _Single:
               res[c] = reader.ReadSingle();
               break;

            case _Int16:
               res[c] = reader.ReadInt16();
               break;

            case _Char:
               res[c] = reader.ReadChar();
               break;

            case _String:
               var len = reader.ReadInt32();

               res[c] = len == 0
                  ? String.Empty
                  : Encoding.ASCII.GetString(reader.ReadBytes(len));
               break;

            case _ByteArray:
               res[c] = reader.ReadBytes(reader.ReadInt32());
               break;

            case _BooleanArray:
               res[c] = ReadArray(reader, br => br.ReadBoolean());
               break;

            case _SingleArray:
               res[c] = ReadArray(reader, br => br.ReadSingle());
               break;

            case _DoubleArray:
               res[c] = ReadArray(reader, br => br.ReadDouble());
               break;

            case _IntegerArray:
               res[c] = ReadArray(reader, br => br.ReadInt32());
               break;

            default:
               throw new NotImplementedException();
         }
      }

      return res;

   }

   private static String GetNameFromProperties(BinaryReader reader,
                                               FbxNodeMetaData meta)
   {
      String? meshName = null;

      for (var c = 0; c < meta.NumProperties; c++)
      {
         switch (reader.ReadByte())
         {
            case _Double:
            case _Int64:
               reader.BaseStream.Seek(8, SeekOrigin.Current);
               break;

            case _Int32:
            case _Single:
               reader.BaseStream.Seek(4, SeekOrigin.Current);
               break;

            case _Int16:
               reader.BaseStream.Seek(2, SeekOrigin.Current);
               break;

            case _Char:
               reader.BaseStream.Seek(1, SeekOrigin.Current);
               break;

            case _String:
               var len = reader.ReadInt32();
               if (len == 0)
                  break;

               if (meshName != null)
                  reader.BaseStream.Seek(len, SeekOrigin.Current);
               else
               {
                  var sb = new StringBuilder();
                  for (var i = 0; i < len; i++)
                  {
                     var current = reader.ReadChar();

                     if (current == '\0')
                     {
                        meshName ??= sb.ToString();
                        //reader.BaseStream.Seek(len - i, SeekOrigin.Current);
                        //break;
                     }
                     else if (meshName == null) 
                        sb.Append(current);
                  }

                  //meshName = Encoding.ASCII.GetString(reader.ReadBytes(len));
               }

               break;

            case _ByteArray:
               reader.BaseStream.Seek(reader.ReadInt32(), SeekOrigin.Current);
               break;

            case _BooleanArray:
               ReadArray(reader, br => br.ReadBoolean());
               break;

            case _SingleArray:
               ReadArray(reader, br => br.ReadSingle());
               break;

            case _DoubleArray:
               ReadArray(reader, br => br.ReadDouble());
               break;

            case _IntegerArray:
               ReadArray(reader, br => br.ReadInt32());
               break;
         }
      }

      return meshName ?? throw new MissingFieldException("mesh name");
   }

   private static Boolean TryGetNamedMesh(BinaryReader reader,
                                          Double fbxVersion,
                                          FbxNodeMetaData meta,
                                          out NamedMesh mesh)
   {
      var meshName = GetNameFromProperties(reader, meta);
      return TryGetNamedMesh(reader, meshName, fbxVersion, out mesh);

   }

   private static Boolean TryGetNamedMesh(BinaryReader reader, 
                                          String meshName,
                                          Double fbxVersion,
                                          out NamedMesh mesh)
   {
      Double[]? allMyPoints = null;
      Int32[]? allMyIndeces = null;
      Vector3[]? allMyVectors=null;

      while (true)
      {
         var meta = GetNodeMeta(reader, fbxVersion);

         if (meta.IsEmpty)
         {
            if (allMyPoints == null || allMyIndeces == null || allMyVectors == null)
               goto fail;

            Debug.WriteLine("mesh " + meshName + " has " + allMyVectors.Length + " vectors");

            var allMyFaces = EarClipping.GetFaces(allMyVectors, allMyIndeces);

            mesh = new NamedMesh(allMyVectors, allMyFaces, meshName);
            return true;
                    
         }

         var nomnom = meta.GetName(reader);

         switch (nomnom)
         {
            case "Vertices":

               switch ((Char) reader.ReadByte())
               {
                  case 'd':
                     allMyPoints = ReadArray(reader, br => br.ReadDouble());
                     allMyVectors = new Vector3[allMyPoints.Length / 3];
                     var i = 0;
                     for (var c = 0; c + 2 < allMyPoints.Length;)
                     {
                        allMyVectors[i++] = new Vector3(allMyPoints[c++],
                           allMyPoints[c++], allMyPoints[c++]);
                     }
                     break;

                  default:
                     throw new NotImplementedException();
               }
               break;

            case "PolygonVertexIndex":
               var doWot = (Char) reader.ReadByte();
               switch (doWot)
               {
                  case 'i':
                     allMyIndeces = ReadArray(reader, br => br.ReadInt32());
                     break;

                  default:
                     throw new NotImplementedException();
               }
               break;

            default:
               meta.SkipData(reader);
               break;
         }
      }

      fail:
      mesh = default!;
      return false;
   }

   //private static CoreMesh GetMeshFromQuads(Int32[] allMyIndeces,
   //                                         ref Int32 indecesCount,
   //                                         Vector3[] allMyVectors)
   //{

   //    var allMyFaces = new Face[indecesCount/4*2];
   //    var faceIndex = 0;

   //    var c = 0;

   //    for (; c + 3 < indecesCount; c += 4)
   //    {
   //        var lefty = Vector3.GetDistance(
   //            allMyVectors[allMyIndeces[c]], // A
   //            allMyVectors[allMyIndeces[c + 2]]); // C

   //        var righty = Vector3.GetDistance(
   //            allMyVectors[allMyIndeces[c + 1]], // B
   //            allMyVectors[0 - allMyIndeces[c + 3] - 1]); // D

   //        if (lefty < righty)
   //        {
   //            // A - C < B - D

   //            allMyFaces[faceIndex++] = new Face(
   //                allMyIndeces[c], // A
   //                allMyIndeces[c + 1], // B
   //                allMyIndeces[c+2]); // C

   //            allMyFaces[faceIndex++] = new Face(
   //                allMyIndeces[c], // A
   //                allMyIndeces[c+2], // C
   //                0 - allMyIndeces[c + 3] - 1); // D
   //        }
   //        else
   //        {
   //            allMyFaces[faceIndex++] = new Face(
   //                allMyIndeces[c], // A
   //                allMyIndeces[c + 1], // B
   //                0 - allMyIndeces[c + 3] - 1); // D


   //            allMyFaces[faceIndex++] = new Face(
   //                0 - allMyIndeces[c + 3] - 1, // D
   //            allMyIndeces[c + 1], // B    
   //            allMyIndeces[c + 2]); // C
   //        }
   //    }

   //    return new CoreMesh(allMyVectors, allMyFaces);
   //}

   //private static CoreMesh GetMeshFromTriangles(Int32[] allMyIndeces,
   //                                             ref Int32 indecesCount,
   //                                             Vector3[] allMyVectors)
   //{
   //    var faceIndex = 0;
   //    var allMyFaces = new Face[indecesCount/3];

   //    for (var c = 0; c + 2 < indecesCount; c += 2)
   //    {
   //        allMyFaces[faceIndex++] = new Face(
   //            allMyIndeces[c], // A
   //            allMyIndeces[c + 1], // B
   //            0 - allMyIndeces[c+2] - 1); // C
   //    }

   //    return new CoreMesh(allMyVectors, allMyFaces);
   //}

   //private IEnumerable<Vector3> GetVertices(BinaryReader reader, 
   //                                        Double fbxVersion)
   //{
   //    while (true)
   //    {
   //        var meta = GetNodeMeta(reader, fbxVersion);

   //        if (meta.IsEmpty)
   //            yield break;

   //        var nomnom = meta.GetName(reader);

   //        switch (nomnom)
   //        {
   //            case "Vertices":

   //                switch ((Char) reader.ReadByte())
   //                {
   //                    case 'd':
   //                        var allMyPoints = ReadArray(reader, br => br.ReadDouble());

   //                        for (var c = 0; c + 2 < allMyPoints.Length;)
   //                        {
   //                            yield return new Vector3(allMyPoints[c],
   //                                allMyPoints[c++], allMyPoints[c++]);
   //                        }

   //                        break;

   //                    default:
   //                        throw new NotImplementedException();
   //                }
   //                break;

   //            default:
   //                meta.SkipData(reader);
   //                break;
   //        }
   //    }
   //}

   private async Task CheckHeaderAsync(Stream stream)
   {
      var buf = new Byte[_standardHeaderLength];
      var amountRead = await stream.ReadAsync(buf, 0, _standardHeaderLength).ConfigureAwait(false);
      if (amountRead != _standardHeaderLength || !AreEqual(_standardHeader, buf))
         throw new InvalidOperationException("Invalid header");
   }

   /// <summary>
   /// Let system specific implementations do faster array comparisons (memcmp etc)
   /// </summary>
   protected virtual Boolean AreEqual(Byte[] arr1, Byte[] arr2)
   {
      if (arr1.Length != arr2.Length)
         return false;

      for (var i = 0; i < arr1.Length; i++)
         if (arr2[i] != arr1[i])
            return false;
      return true;
   }

   private static T[] ReadArray<T>(BinaryReader stream,
                                   Func<BinaryReader, T> readPrimitive)
   {
      var len = stream.ReadInt32();
      var encoding = stream.ReadInt32();
      var compressedLen = stream.ReadInt32();
      var ret = new T[len];
      //var ret = Array.CreateInstance(arrayType, len);
      var s = stream;
      var endPos = stream.BaseStream.Position + compressedLen;
      if (encoding != 0)
      {
         //if(errorLevel >= ErrorLevel.Checked)
         {
            if(encoding != 1)
               throw new Exception(
                  "Invalid compression encoding (must be 0 or 1)");
            var cmf = stream.ReadByte();
            if((cmf & 0xF) != 8 || (cmf >> 4) > 7)
               throw new Exception(
                  "Invalid compression format " + cmf);
            var flg = stream.ReadByte();
            if(((cmf << 8) + flg) % 31 != 0)
               throw new Exception(
                  "Invalid compression FCHECK");
            if((flg & (1 << 5)) != 0)
               throw new Exception(
                  "Invalid compression flags; dictionary not supported");
         } 
         //            else
         //{
         //	stream.BaseStream.Position += 2;
         //}
         var codec = new DeflateWithChecksum(stream.BaseStream, CompressionMode.Decompress);
         s = new BinaryReader(codec);
      }
      try
      {
         for (var i = 0; i < len; i++)
         {
            ret[i] = readPrimitive(s);
            //ret.SetValue(readPrimitive(s), i);
         }
      }
      catch (InvalidDataException)
      {
         throw new Exception("Compressed data was malformed");
      }
      if (encoding != 0)
      {
         //if (errorLevel >= ErrorLevel.Checked)
         {
            stream.BaseStream.Position = endPos - sizeof(Int32);
            var checksumBytes = new Byte[sizeof(Int32)];
            var returned = stream.BaseStream.Read(checksumBytes, 0, checksumBytes.Length);
            var checksum = 0;
            //for (var i = 0; i < checksumBytes.Length; i++)
            for (var i = 0; i < returned; i++)
            {
               checksum = (checksum << 8) + checksumBytes[i];
            }

            if(checksum != ((DeflateWithChecksum)s.BaseStream).Checksum)
               throw new Exception("Compressed data has invalid checksum");
         }
				
      }
      return ret;
   }
}