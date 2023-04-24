using System;
using System.Threading.Tasks;

namespace Das.ViewModels.ChangeTracking;

[AttributeUsage(AttributeTargets.Class)]
public class ChangeTrackingTypeAttribute : Attribute
{
   public ChangeTrackingTypeAttribute(ChangeTrackingTypes trackingType)
   {
      TrackingType = trackingType;
   }

   public ChangeTrackingTypes TrackingType { get; set; }
}