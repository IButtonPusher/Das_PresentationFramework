using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.ViewModels;

namespace Das.Mvvm
{
   public class Selectable<T> : Selectable
   {
      public Selectable(T item,
                        Boolean isSelected)
         : base(GetDescription(item), isSelected)
      {
         Item = item;
      }

      public Selectable(T item) : base(GetDescription(item))
      {
         Item = item;
      }

      private static String GetDescription(T item)
      {
         switch (item)
         {
            case null:
               throw new ArgumentNullException();

            case Enum _:
               var fi = item.GetType().GetField(item.ToString());

               var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                  typeof(DescriptionAttribute), true);

               if (attributes.Length > 0)
                  return attributes[0].Description;
               else
                  return item.ToString();

            default:
               return item.ToString();
         }
      }


      public T Item { get; }
   }
}
