﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Das.ViewModels;

// ReSharper disable VirtualMemberCallInConstructor

namespace Das.Mvvm;

public class SelectableCollection<T> : ObservableRangeCollection<Selectable<T>>,
                                       IDisposable


{
    public SelectableCollection()
    {
        CollectionChanged += OnThisHereCollectionChanged;
    }

    public SelectableCollection(IEnumerable<Selectable<T>> items)
        : base(items)
    {
        foreach (var item in this)
        {
            item.PropertyChanged += OnSelectableValueChanged;
        }

        UpdateIsAnyOrAllSelected();

        CollectionChanged += OnThisHereCollectionChanged;
    }


    public SelectableCollection(IEnumerable<T> available,
                                IEnumerable<T> initiallySelected,
                                Boolean isSelectAllIfNoneSelected)
    {
        var searchSelected = new HashSet<T>(initiallySelected);

        isSelectAllIfNoneSelected &= searchSelected.Count == 0;

        foreach (var item in available)
        {
            var vm = new Selectable<T>(item,
                isSelectAllIfNoneSelected || searchSelected.Contains(item));

            vm.PropertyChanged += OnSelectableValueChanged;

            Add(vm);
        }

        UpdateIsAnyOrAllSelected();

        CollectionChanged += OnThisHereCollectionChanged;
    }


    public void Dispose()
    {
        SelectionChanged = null;
        foreach (var item in this)
        {
            item.Dispose();
        }
    }

    public void Add(T value,
                    Boolean isSelected,
                    String description)
    {
        Add(new Selectable<T>(value, isSelected, description));
    }

    public IEnumerable<T> GetSelectedItems()
    {
        foreach (var item in this)
        {
            if (!item.IsSelected)
                continue;

            yield return item.Item;
        }
    }

    public Boolean Select(T item)
    {
        foreach (var vm in this)
        {
            if (Equals(vm.Item, item))
            {
                vm.IsSelected = true;
                return true;
            }
        }

        return false;
    }

    private void SetValue<TProp>(ref TProp storage,
                                 TProp value,
                                 [CallerMemberName] String? propertyName = null)
    {
        if (Equals(storage, value))
            return;

        storage = value;
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    private void OnThisHereCollectionChanged(Object sender,
                                             NotifyCollectionChangedEventArgs e)
    {
        e.HandleCollectionChanges<Selectable<T>>(oldies =>
        {
            foreach (var o in oldies)
                o.Dispose();
        }, newbies =>
        {
            foreach (var o in newbies)
                o.PropertyChanged += OnSelectableValueChanged;
        });
    }

    private void OnSelectableValueChanged(Object sender,
                                          PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Selectable<T>.IsSelected))
        {
            if (sender is Selectable<T> good)
                SelectionChanged?.Invoke(good);

            UpdateIsAnyOrAllSelected();
        }
    }

    private void UpdateIsAnyOrAllSelected()
    {
        var selCount = this.Count(i => i.IsSelected);
        IsAllSelected = selCount == Count;
        IsAnySelected = selCount > 0;
    }

    public Boolean IsAnySelected
    {
        get => _isAnySelected;
        set => SetValue(ref _isAnySelected, value);
    }

    public Boolean IsAllSelected
    {
        get => _isAllSelected;
        set => SetValue(ref _isAllSelected, value);
    }

    public event Action<Selectable<T>>? SelectionChanged;

    private Boolean _isAllSelected;
    private Boolean _isAnySelected;
}