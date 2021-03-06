﻿using System.Collections.ObjectModel;

namespace SecuNoteUniversal.ViewModels
{
    class FileItemsViewModel : ViewModelBase
    {
        private ObservableCollection<FileItemViewModel> _fileItems;

        public ObservableCollection<FileItemViewModel> FileItems
        {
            get { return _fileItems; }
            set
            {
                if (_fileItems == value) return;
                _fileItems = value;
                RaisePropertyChanged("FileItems");
            }
        }
    }
}
