using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Das.Extensions;

public static class DtoExtensions
{
    [MethodImpl(256)]
    public static Boolean AreDifferent(this Double d1,
                                       Double d2)
    {
       if (Double.IsNaN(d1))
          return !Double.IsNaN(d2);

       if (Double.IsNaN(d2))
          return true;

        return Math.Abs(d1 - d2) > MyEpsilon;
    }

    [MethodImpl(256)]
    public static Boolean AreEqualEnough(this Double d1,
                                         Double d2)
    {
       if (Double.IsNaN(d1))
          return Double.IsNaN(d2);

       if (Double.IsNaN(d2))
          return false;

       return Math.Abs(d1 - d2) < MyEpsilon;
    }

    [MethodImpl(256)]
    public static Boolean AreEqualEnough(this Double? d1,
                                         Double? d2)
    {
       if (!d1.HasValue)
          return !d2.HasValue;

       if (d2.HasValue)
          return AreEqualEnough(d1.Value, d2.Value);
       return false;
    }

    [MethodImpl(256)]
    public static Boolean IsNotZero(this Decimal d)
    {
        return d != 0;
    }

    [MethodImpl(256)]
    public static Boolean IsNotZero(this Double d)
    {
        return Math.Abs(d) >= MyEpsilon;
    }

    [MethodImpl(256)]
    public static Boolean IsNotZero(this Int32 d)
    {
        return d != 0;
    }

    [MethodImpl(256)]
    public static Boolean IsZero(this Double d)
    {
        return Math.Abs(d) < MyEpsilon;
    }

    [MethodImpl(256)]
    public static Boolean IsZero(this Single d)
    {
        return Math.Abs(d) < MyEpsilon;
    }

    [MethodImpl(256)]
    public static Boolean IsNotZero(this Single d)
    {
       return Math.Abs(d) >= MyEpsilon;
    }

    [MethodImpl(256)]
    public static Boolean IsZero(this Int32 d)
    {
        return d == 0;
    }

    [MethodImpl(256)]
    public static Boolean AreDifferent(this Single d1,
                                       Single d2)
    {
       if (Single.IsNaN(d1))
          return !Single.IsNaN(d2);

       if (Single.IsNaN(d2))
          return true;

       return Math.Abs(d1 - d2) > MyEpsilon;
    }
   

    private const Double MyEpsilon = 0.00001;
}