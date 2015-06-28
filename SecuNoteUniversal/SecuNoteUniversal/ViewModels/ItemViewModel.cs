//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SecuNote.Models;

//namespace SecuNote.ViewModels
//{
//    class ItemViewModel : AbstractItemViewModel, ITemInterface
//    {
//        private bool _encrypted;
//        public Boolean Encrypted
//        {
//            get { return _encrypted; }
//            set
//            {
//                if (Encrypted == value) { return;}
//                _encrypted = Encrypted;
//                RaisePropertyChanged("Encryption");
//            }
//        }

//        private String _name;
//        public String Name
//        {
//            get { return _name; }
//            set
//            {
//                if (Name == value) { return;}
//                _name = Name;
//                RaisePropertyChanged("Name");
//            }
//        }

//        private int _iD;
//        public int ID
//        {
//            get { return _iD; }
//            set
//            {
//                if (ID == value)
//                {
//                    return;
//                }
//                _iD = value;
//                RaisePropertyChanged("ID");
//            }
//        }


//        public ItemViewModel(int id, string name)
//        {
//            this.ID = id;
//            this.Name = name;
//            _encrypted = false;
//        }

//        public ItemViewModel()
//        {
//        }


//        /// <summary>
//        /// Should check whether the file is stored in the database and whether it is _encrypted
//        /// </summary>
//        /// <returns></returns>
//        protected bool checkEncryption()
//        {

//            throw new NotImplementedException();
//        }

//        //public String SaveItem(AbstractItemViewModel item)
//        //{
//        //    string result = string.Empty;
//        //    using (var db = new SQLite.SQLiteConnection(App.DBPath))
//        //    {
//        //        try
//        //        {
//        //            var existingItem = (db.Table<AbstractItemModel>().Where(c => c.Id == item.ID)).SingleOrDefault();

//        //            if (existingItem != null)
//        //            {
//        //                existingItem.Id = item.ID;
//        //                existingItem.Name = item.Name;
//        //                existingItem.Encrypted = item.Encrypted;
//        //                db.Update(existingItem);
//        //                result = "Existing Item detected - Item updated";
//        //            }
//        //            else
//        //            {
//        //                db.Insert(new AbstractItemModel()
//        //                {
//        //                    Encrypted = item.Encrypted,
//        //                    Id = item.ID,
//        //                    Name = item.Name
//        //                });
//        //                result = "Success - new item added to database.";
//        //            }
//        //        }
//        //        catch
//        //        {

//        //            result = "Failure - Item not saved";
//        //        }
//        //        return result;
//        //    }
//        //}

//        //public ItemViewModel GetItem(int itemID)
//        //{
//        //    var itemViewModelGotten = new ItemViewModel();
//        //    using (var db = new SQLite.SQLiteConnection(App.DBPath))
//        //    {
//        //        var itemModelGotten = (db.Table<AbstractItemModel>().Where(c => c.Id == itemID)).Single();
//        //        if (itemModelGotten != null)
//        //        {

//        //            itemViewModelGotten.ID = itemID;
//        //            itemViewModelGotten.Name = itemModelGotten.Name;
//        //            itemViewModelGotten.Encrypted = itemModelGotten.Encrypted;

//        //        }
//        //    }
//        //    return itemViewModelGotten;
//        //}

//        ///// <summary>
//        ///// Get Id from database or if none present generate new ID
//        ///// </summary>
//        ///// <returns></returns>
//        //protected int getID()
//        //{
//        //    throw new NotImplementedException();
//        //}
//    }
//}

