using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SecuNoteUniversal.Models;
using SecuNoteUniversal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SecuNoteUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainItemPage : Page
    {
        public MainItemPage()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<AbstractItemViewModel> ItemModels { get; set; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await DatabaseViewModel.Initialise();
            ItemModels = DatabaseViewModel.ItemViewModels;
            ItemsList.ItemsSource = ItemModels;
        }

        private void AddNewItemButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof (AddNewItemPage));
        }

        private async void EncryptFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var encryptor = new Encryptor();
            await encryptor.Initialise("12345");
            foreach (var abstractItemModel in ItemModels.Where(abstractItemModel => abstractItemModel.GetType() == typeof(FileItemViewModel)))
            {
                await encryptor.EncryptFileAes(abstractItemModel as FileItemViewModel);
            }
        }

        //private void Page_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    DatabaseViewModel.Initialise();
        //    ItemModels = DatabaseViewModel.ItemViewModels;
        //    ItemsList.ItemsSource = ItemModels;
        //}
    }
}
