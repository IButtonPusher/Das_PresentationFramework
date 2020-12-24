using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable All
#pragma warning disable 8618
#pragma warning disable 8603
#pragma warning disable 8625
#pragma warning disable 8600

namespace CSharp8NetFramework
{
    public abstract class ArrayPool<T>
  {
    private static ArrayPool<T> s_sharedInstance;

    public static ArrayPool<T> Shared
    {
      [MethodImpl((MethodImplOptions) 256)] get
      {
        return Read<ArrayPool<T>>(ref s_sharedInstance) ?? EnsureSharedCreated();
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ArrayPool<T> EnsureSharedCreated()
    {
      Interlocked.CompareExchange<ArrayPool<T>>(ref s_sharedInstance, Create(), null);
      return s_sharedInstance;
    }
   

    public static ArrayPool<T> Create()
    {
      return new DefaultArrayPool<T>();
    }

    public static ArrayPool<T> Create(int maxArrayLength, int maxArraysPerBucket)
    {
      return new DefaultArrayPool<T>(maxArrayLength, maxArraysPerBucket);
    }

    public abstract T[] Rent(int minimumLength);

    public abstract void Return(T[] array, bool clearArray = false);

   
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SecuritySafeCritical]
    public static TSomething Read<TSomething>(ref TSomething location) 
        where TSomething : class
    {
        TSomething result = location;
        Thread.MemoryBarrier();
        return result;
    }
  }


    internal sealed class DefaultArrayPool<T> : ArrayPool<T>
  {
    private const int DefaultMaxArrayLength = 1048576;
    private const int DefaultMaxNumberOfArraysPerBucket = 50;
    private static T[] s_emptyArray;
    private readonly Bucket[] _buckets;

    internal DefaultArrayPool()
      : this(1048576, 50)
    {
    }

    internal DefaultArrayPool(int maxArrayLength, int maxArraysPerBucket)
    {
      if (maxArrayLength <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxArrayLength));
      if (maxArraysPerBucket <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxArraysPerBucket));
      if (maxArrayLength > 1073741824)
        maxArrayLength = 1073741824;
      else if (maxArrayLength < 16)
        maxArrayLength = 16;
      int id = Id;
      Bucket[] bucketArray = new Bucket[Utilities.SelectBucketIndex(maxArrayLength) + 1];
      for (int binIndex = 0; binIndex < bucketArray.Length; ++binIndex)
        bucketArray[binIndex] = new Bucket(Utilities.GetMaxSizeForBucket(binIndex), maxArraysPerBucket, id);
      _buckets = bucketArray;
    }

    private int Id
    {
      get
      {
        return GetHashCode();
      }
    }

    public override T[] Rent(int minimumLength)
    {
      if (minimumLength < 0)
        throw new ArgumentOutOfRangeException(nameof (minimumLength));
      if (minimumLength == 0)
        return s_emptyArray ?? (s_emptyArray = new T[0]);
      //ArrayPoolEventSource log = ArrayPoolEventSource.Log;
      int index1 = Utilities.SelectBucketIndex(minimumLength);
      T[] objArray1;
      if (index1 < _buckets.Length)
      {
        int index2 = index1;
        do
        {
          T[] objArray2 = _buckets[index2].Rent();
          if (objArray2 != null)
          {
            //if (log.IsEnabled())
            //  log.BufferRented(objArray2.GetHashCode(), objArray2.Length, this.Id, this._buckets[index2].Id);
            return objArray2;
          }
        }
        while (++index2 < _buckets.Length && index2 != index1 + 2);
        objArray1 = new T[_buckets[index1]._bufferLength];
      }
      else
        objArray1 = new T[minimumLength];
      //if (log.IsEnabled())
      //{
      //  int hashCode = objArray1.GetHashCode();
      //  int bucketId = -1;
      //  log.BufferRented(hashCode, objArray1.Length, this.Id, bucketId);
      //  log.BufferAllocated(hashCode, objArray1.Length, this.Id, bucketId, index1 >= this._buckets.Length ? ArrayPoolEventSource.BufferAllocatedReason.OverMaximumSize : ArrayPoolEventSource.BufferAllocatedReason.PoolExhausted);
      //}
      return objArray1;
    }

    public override void Return(T[] array, bool clearArray = false)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (array.Length == 0)
        return;
      int index = Utilities.SelectBucketIndex(array.Length);
      if (index < _buckets.Length)
      {
        if (clearArray)
          Array.Clear(array, 0, array.Length);
        _buckets[index].Return(array);
      }
      //ArrayPoolEventSource log = ArrayPoolEventSource.Log;
      //if (!log.IsEnabled())
      //log.BufferReturned(array.GetHashCode(), array.Length, this.Id);
    }

    private sealed class Bucket
    {
      internal readonly int _bufferLength;
      private readonly T[][] _buffers;
      private readonly int _poolId;
      private SpinLock _lock;
      private int _index;

      internal Bucket(int bufferLength, int numberOfBuffers, int poolId)
      {
        _lock = new SpinLock(Debugger.IsAttached);
        _buffers = new T[numberOfBuffers][];
        _bufferLength = bufferLength;
        _poolId = poolId;
      }

      internal int Id
      {
        get
        {
          return GetHashCode();
        }
      }

      internal T[] Rent()
      {
        T[][] buffers = _buffers;
        T[] objArray = null;
        bool lockTaken = false;
        bool flag = false;
        try
        {
          _lock.Enter(ref lockTaken);
          if (_index < buffers.Length)
          {
            objArray = buffers[_index];
            buffers[_index++] = null;
            flag = objArray == null;
          }
        }
        finally
        {
          if (lockTaken)
            _lock.Exit(false);
        }
        if (flag)
        {
          objArray = new T[_bufferLength];
          //ArrayPoolEventSource log = ArrayPoolEventSource.Log;
          //if (log.IsEnabled())
          //  log.BufferAllocated(objArray.GetHashCode(), this._bufferLength, this._poolId, this.Id, ArrayPoolEventSource.BufferAllocatedReason.Pooled);
        }
        return objArray;
      }

      internal void Return(T[] array)
      {
        if (array.Length != _bufferLength)
          throw new ArgumentException(nameof (array));
        bool lockTaken = false;
        try
        {
          _lock.Enter(ref lockTaken);
          if (_index == 0)
            return;
          _buffers[--_index] = array;
        }
        finally
        {
          if (lockTaken)
            _lock.Exit(false);
        }
      }
    }
  }

    internal static class Utilities
    {
        [MethodImpl((MethodImplOptions) 256)]
        internal static int SelectBucketIndex(int bufferSize)
        {
            uint num1 = (uint) (bufferSize - 1) >> 4;
            int num2 = 0;
            if (num1 > ushort.MaxValue)
            {
                num1 >>= 16;
                num2 = 16;
            }
            if (num1 > byte.MaxValue)
            {
                num1 >>= 8;
                num2 += 8;
            }
            if (num1 > 15U)
            {
                num1 >>= 4;
                num2 += 4;
            }
            if (num1 > 3U)
            {
                num1 >>= 2;
                num2 += 2;
            }
            if (num1 > 1U)
            {
                num1 >>= 1;
                ++num2;
            }
            return num2 + (int) num1;
        }

        [MethodImpl((MethodImplOptions) 256)]
        internal static int GetMaxSizeForBucket(int binIndex)
        {
            return 16 << binIndex;
        }
    }
}


