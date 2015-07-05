using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.AccessCache;
using SecuNoteUniversal.Models;
using SQLite;

namespace SecuNoteUniversal.ViewModels
{
    public class FileItemViewModel : AbstractItemViewModel, IItemData
    {
        private IStorageFile _file;
        private Constants.Filetype _fileType;
        private String _mruToken;

        public FileItemViewModel()
        {
        }

        public FileItemViewModel(int id, string name, IStorageFile file) : base(id, name)
        {
            File = file;
            MruToken = StorageApplicationPermissions.FutureAccessList.Add(File);
        }

        public  FileItemViewModel(IStorageFile fileIn)
        {
            _file = fileIn;
            Name = fileIn.Name;
            IsEncrypted = false;
        }

        public IStorageFile File
        {
            get { return _file; }
            set
            {
                if (File == value) return;
                _file = value;
                RaisePropertyChanged("File");
            }
        }

        public Constants.Filetype Filetype
        {
            get { return _fileType; }
            set
            {
                if (Filetype == value)
                {
                    return;
                }
                _fileType = value;
                RaisePropertyChanged("FileType");
            }
        }

        public String MruToken
        {
            get { return _mruToken; }
            set
            {
                if (MruToken == value) return;
                _mruToken = value;
                RaisePropertyChanged("MruToken");
            }
        }

        public string SaveItem(AbstractItemViewModel item)
        {
            if (item.GetType() == GetType())
            {
                var fileItem = (FileItemViewModel) item;
                using (var db = new SQLiteConnection(DatabaseHandler.DBPath))
                {
                    string result;
                    try
                    {
                        var existingItem = (db.Table<FileItemModel>().Where(c => c.Id == fileItem.ID)).SingleOrDefault();

                        if (existingItem != null)
                        {
                            existingItem.Id = fileItem.ID;
                            existingItem.Name = fileItem.Name;
                            existingItem.IsEncrypted = fileItem.IsEncrypted;
                            existingItem.MruToken = fileItem.MruToken;
                            existingItem.Nonce = CryptographicBuffer.EncodeToBase64String(fileItem.Nonce);
                            db.Update(existingItem);
                            result = "Existing Item detected - Item updated";
                        }
                        else
                        {
                            db.Insert(new FileItemModel
                            {
                                IsEncrypted = fileItem.IsEncrypted,
                                Id = fileItem.ID,
                                Name = fileItem.Name,
                                FileType = fileItem.Filetype.ToString(),
                                MruToken = fileItem.MruToken,
                                Nonce = CryptographicBuffer.EncodeToBase64String(fileItem.Nonce)
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
            return "Failure - Item not of correct class";
        }

        public string DeleteItem(int id)
        {
            var result = String.Empty;
            using (var db = new SQLiteConnection(DatabaseHandler.DBPath))
            {
                var modelGotten = (db.Table<FileItemModel>().Where(c => c.Id == id)).Single();
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

        public async Task<AbstractItemViewModel> GetItem(string name)
        {
            var fileItemViewModelGotten = new FileItemViewModel();
            using (var db = new SQLiteConnection(DatabaseHandler.DBPath))
            {
                var modelGotten = (db.Table<FileItemModel>().Where(c => c.Name == name)).Single();
                if (modelGotten != null)
                {
                    fileItemViewModelGotten.ID = modelGotten.Id;
                    fileItemViewModelGotten.Name = modelGotten.Name;
                    fileItemViewModelGotten.IsEncrypted = modelGotten.IsEncrypted;
                    fileItemViewModelGotten.MruToken = modelGotten.MruToken;
                    fileItemViewModelGotten.File = await SynchronisationHandler.WorkingDirectory.GetFileAsync(name);
                    fileItemViewModelGotten.Nonce =
                        CryptographicBuffer.DecodeFromBase64String(modelGotten.Nonce);
                }
            }
            return fileItemViewModelGotten;
        }

        private async Task<StorageFile> GetFileFromToken()
        {
            var fileToReturn = await
                StorageApplicationPermissions.FutureAccessList.GetFileAsync(MruToken);
            return fileToReturn;
        }
    }
}