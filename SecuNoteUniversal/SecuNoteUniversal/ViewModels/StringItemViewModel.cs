using System;
using System.Linq;
using Windows.Storage.Streams;
using SecuNoteUniversal.Models;
using SQLite;

namespace SecuNoteUniversal.ViewModels
{
    public class StringItemViewModel : AbstractItemViewModel, IItemData
    {
        #region Properties
        private String _content;

        

        public String Content
        {
            get { return _content; }

            set
            {
                if (Content == value) return;
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        private IBuffer _authenticationTag;

        public IBuffer AuthenticationTag
        {
            get { return _authenticationTag; }
            set
            {
                if (AuthenticationTag == value) return;
                _authenticationTag = value;
                RaisePropertyChanged("AuthenticationTag");
            }
        }


        #endregion "Properties"
        public StringItemViewModel(int id, string name, string content) : base(id, name)
        {
            ID = id;
            IsEncrypted = false;
            Name = name;
            Content = content;
        }

        public StringItemViewModel()
        {
        }
        public string SaveItem(AbstractItemViewModel item)
        {
            if (item.GetType() == GetType())
            {
                var stringItem = (StringItemViewModel) item;
                var result = string.Empty;
                using (var db = new SQLiteConnection(DatabaseHandler.DBPath))
                {
                    try
                    {
                        var existingItem = (db.Table<StringItemModel>().Where(c => c.Id == item.ID)).SingleOrDefault();

                        if (existingItem != null)
                        {
                            existingItem.Id = stringItem.ID;
                            existingItem.Name = stringItem.Name;
                            existingItem.IsEncrypted = stringItem.IsEncrypted;
                            existingItem.Content = stringItem.Content;
                            existingItem.Nonce = stringItem.Nonce;
                            existingItem.AuthenticationTag = stringItem.AuthenticationTag;
                            db.Update(existingItem);
                            result = "Existing Item detected - Item updated";
                        }
                        else
                        {
                            db.Insert(new StringItemModel
                            {
                                IsEncrypted = stringItem.IsEncrypted,
                                Id = stringItem.ID,
                                Name = stringItem.Name,
                                Content = stringItem.Content,
                                Nonce = stringItem.Nonce,
                                AuthenticationTag = stringItem.AuthenticationTag
                            });
                            result = "Success - new item added to database.";
                        }
                    }
                    catch
                    {
                        result = "Failure - Item not saved";
                    }
                    return result;
                }
            }
            return "Failure Item not of correct class";
        }

        public AbstractItemViewModel GetItem(int itemID) 
        {
            var stringItemViewModelGotten = new StringItemViewModel();
            using (var db = new SQLiteConnection(DatabaseHandler.DBPath))
            {
                //StringItemModel modelGotten;

                var modelGotten = (db.Table<StringItemModel>().Where(c => c.Id == itemID)).SingleOrDefault();
                
                
                if (modelGotten != null)
                {
                    stringItemViewModelGotten.ID = itemID;
                    stringItemViewModelGotten.Name = modelGotten.Name;
                    stringItemViewModelGotten.IsEncrypted = modelGotten.IsEncrypted;
                    stringItemViewModelGotten.Content = modelGotten.Content;
                    stringItemViewModelGotten.Nonce = modelGotten.Nonce;
                    stringItemViewModelGotten.AuthenticationTag = modelGotten.AuthenticationTag;
                }
                else
                {
                    throw new ArgumentException("No item with that ID");
                }
            }
            return stringItemViewModelGotten;
        }

        public string DeleteItem(int itemID)
        {
            var result = String.Empty;
            using (var db = new SQLiteConnection(DatabaseHandler.DBPath))
            {
                var modelGotten = (db.Table<StringItemModel>().Where(c => c.Id == itemID)).Single();
                if (modelGotten != null)
                {
                    db.Delete(modelGotten);
                    result = "Success - String item deleted.";
                }
                else
                {
                    result = "Failure - String item not found";
                }
            }
            return result;
        }

        public override string ToString()
        {

            return Name;
        }
    }
}