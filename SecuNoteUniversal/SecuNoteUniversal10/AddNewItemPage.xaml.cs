using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SecuNoteUniversal10.Models;
using SecuNoteUniversal10.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SecuNoteUniversal10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewItemPage : Page
    {

        private StorageFile _filePicked = null;
        public AddNewItemPage()
        {
            this.InitializeComponent();
            TypeBox.ItemsSource = Enum.GetValues(typeof (Constants.Filetype));
        }



        private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            _filePicked =  await picker.PickSingleFileAsync();

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_filePicked == null) return;
            var item = await SynchronisationHandler.AddTotalyNewFile(_filePicked);
            //DatabaseViewModel.UpdateAllDatabaseEntries();
            item.Filetype = (Constants.Filetype)TypeBox.SelectionBoxItem;
            DatabaseViewModel.ItemViewModels.Add(item);
            this.Frame.Navigate(typeof (MainItemPage));
        }
    }
}
