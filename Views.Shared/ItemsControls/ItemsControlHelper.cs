using System;
using System.Threading.Tasks;
using Das.Views.Mvvm;

namespace Das.Views.ItemsControls
{
    public class ItemsControlHelper : ItemsControlHelperBase
    {
        public ItemsControlHelper(
            Func<INotifyingCollection?, INotifyingCollection?, Boolean>? onItemsSourceChanging,
            Action<IDataTemplate?>? onItemTemplateChanged)
        : base(onItemTemplateChanged)
        {
            _onItemsSourceChanging = onItemsSourceChanging ?? DefaultOnItemsSourceChanging;
        }

        public INotifyingCollection? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value,
                _onItemsSourceChanging);
        }

        private readonly Func<INotifyingCollection?, INotifyingCollection?, Boolean> _onItemsSourceChanging;
        private INotifyingCollection? _itemsSource;
        private static Boolean DefaultOnItemsSourceChanging(INotifyingCollection? oldValue,
                                                            INotifyingCollection? newValue) => true;
        
        
    }

    public class ItemsControlHelperBase : NotifyPropertyChangedBase
    {
        public ItemsControlHelperBase(Action<IDataTemplate?>? onItemTemplateChanged)
        {
            _onItemTemplateChanged = onItemTemplateChanged ?? DefaultOnItemTemplateChanged;
        }
        
        public virtual IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate, value, _onItemTemplateChanged);
        }

        private readonly Action<IDataTemplate?> _onItemTemplateChanged;
        private IDataTemplate? _itemTemplate;
        private static void DefaultOnItemTemplateChanged(IDataTemplate? newValue) {}
    }
}