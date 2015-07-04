using System;
using Windows.Storage.Streams;

namespace SecuNoteUniversal.ViewModels
{
    public abstract class AbstractItemViewModel : ViewModelBase
    {
        #region Properties

        private bool _isEncrypted;
        private int _iD;
        private String _name;

        public IBuffer Nonce { get; set; }


        protected AbstractItemViewModel(int id, string name)
        {
            ID = id;
            Name = name;
        }

        protected AbstractItemViewModel()
        {
        }

        public Boolean IsEncrypted
        {
            get { return _isEncrypted; }
            set
            {
                if (_isEncrypted == value)
                {
                    return;
                }
                _isEncrypted = value;
                RaisePropertyChanged("Encryption");
            }
        }

        public String Name
        {
            get { return _name; }
            set
            {
                if (Name == value)
                {
                    return;
                }
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public int ID
        {
            get { return _iD; }
            set
            {
                if (ID == value)
                {
                    return;
                }
                _iD = value;
                RaisePropertyChanged("ID");
            }
        }
        #endregion "Properties"

        public override string ToString()
        {
            return Name;
        }
    }
}