using System;

namespace Das.Views.Core;

public interface IQuantifiedValue<TValue, out TUnits>
   where TUnits : Enum
{
   TValue GetQuantity(TValue available);

   TUnits Units {get;}

}