using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.ViewModels;

namespace Das.Mvvm
{
    public class Selectable<T> : BaseViewModel
    {
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

        public Boolean IsSelected
        {
            get => _isSelected;
            set => SetValue(ref _isSelected, value);
        }

        public T Item { get; }

        public override String ToString()
        {
            return _description;
        }

        private readonly String _description;


        private Boolean _isSelected;
    }
}