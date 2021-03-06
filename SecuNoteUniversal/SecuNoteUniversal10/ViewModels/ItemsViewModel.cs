﻿using System.Collections.ObjectModel;

namespace SecuNoteUniversal10.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private ObservableCollection<AbstractItemViewModel> _items;

        public ObservableCollection<AbstractItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (Items == value) return;
                _items = value;
                RaisePropertyChanged("Items");
            }
        }
    }
}