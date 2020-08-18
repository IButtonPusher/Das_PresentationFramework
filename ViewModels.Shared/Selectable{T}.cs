using System;
using System.ComponentModel;
using Das.ViewModels;

namespace Das.Mvvm
{
    public class Selectable<T> : BaseViewModel
    {
        public T Item { get; }


        private Boolean _isSelected;
        private readonly String _description;

        public Boolean IsSelected
        {
            get => _isSelected;
            set => SetValue(ref _isSelected, value);
        }

        public Selectable(T item)
        {
            switch (item)
            {
                case null:
                    throw new ArgumentNullException();

                case Enum _:
                    var fi = item.GetType().GetField(item.ToString());

                    var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(
                        typeof(DescriptionAttribute), false);

                    if (attributes.Length > 0)
                        _description = attributes[0].Description;
                    else
                        _description = item.ToString();
                    break;

                default:
                    _description = item.ToString();
                    break;
            }

            Item = item;
        }

        public override String ToString() => _description;

    }
}
