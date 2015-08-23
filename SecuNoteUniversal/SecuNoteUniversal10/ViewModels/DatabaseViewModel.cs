using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SecuNoteUniversal10.Models;
using SQLite;

namespace SecuNoteUniversal10.ViewModels
{
    public static class DatabaseViewModel
    {
        public static ObservableCollection<AbstractItemViewModel> ItemViewModels { get; set; }

        public static async Task Initialise()
        {
            ItemViewModels = new ObservableCollection<AbstractItemViewModel>();
            using (var _db = new SQLiteConnection(DatabaseHandler.DBPath))
            {
                var listFile = _db.Table<FileItemModel>().ToList();
                var listString = _db.Table<StringItemModel>().ToList();
                var listFileViewModel = new List<FileItemViewModel>();
                foreach (var fileItemModel in listFile)
                {
                    var item = await new FileItemViewModel().GetItem(fileItemModel.Name);
                    listFileViewModel.Add(item as FileItemViewModel);
                    //var itemvm = (FileItemViewModel) item;
                    //itemvm.SaveItem(itemvm);
                }
                var allList = new List<AbstractItemViewModel>();
                allList.AddRange(listFileViewModel);
                var listStringViewModel = new List<StringItemViewModel>();
                foreach (var fileItemModel in listString)
                {
                    var item = await new StringItemViewModel().GetItem(fileItemModel.Name);
                    listStringViewModel.Add(item as StringItemViewModel);
                }
                allList.AddRange(listStringViewModel);
                ItemViewModels = new ObservableCollection<AbstractItemViewModel>(allList);
            }
        }

        public static async void UpdateAllDatabaseEntries()
        {
            await Initialise();
            foreach (
                var item in
                    ItemViewModels.OfType<FileItemViewModel>().Select(abstractItemViewModel => abstractItemViewModel))
            {
                item.SaveItem(item);
            }
            foreach (
                var item in
                    ItemViewModels.OfType<StringItemViewModel>().Select(abstractItemViewModel => abstractItemViewModel))
            {
                item.SaveItem(item);
            }
        }
    }
}