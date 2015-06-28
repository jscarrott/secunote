using SecuNote;
using SecuNoteUniversal.Models;

namespace SecuNoteUniversal.ViewModels
{
    public class StringItemsViewModel : ViewModelBase
    {
        private StringItemObservableCollection _stringItems;

        public StringItemObservableCollection StringItems
        {
            get { return _stringItems; }
            set
            {
                if (_stringItems == value) return;
                _stringItems = value;
                RaisePropertyChanged("StringItems");
            }
        }

        public StringItemObservableCollection GetStringItems()
        {
            StringItems = new StringItemObservableCollection();
            using (var db = new SQLite.SQLiteConnection(DatabaseHandler.DBPath))
            {
                var query = db.Table<StringItemViewModel>().OrderBy(c => c.Name);
                foreach (var stringItem in query)
                {
                    var customer = new StringItemViewModel()
                    {
                        ID = stringItem.ID,
                        Name = stringItem.Name,
                        Content = stringItem.Content,
                        IsEncrypted = stringItem.IsEncrypted
                    };
                    StringItems.Add(customer);
                }
            }
            return StringItems;
        }
    }
    
}