using System;

namespace Das.ViewModels
{
   public abstract class Selectable : BaseViewModel
   {
      public Boolean IsSelected
      {
         get => _isSelected;
         set => SetValue(ref _isSelected, value);
      }

      private Boolean _isSelected;

      protected Selectable(String description,
                           Boolean isSelected)
      {
         Description = description;
         _isSelected = isSelected;
      }

      protected Selectable(String description)
      {
         Description = description;
      }

      public String Description { get; }

      public override String ToString()
      {
         return Description;
      }
   }
}
