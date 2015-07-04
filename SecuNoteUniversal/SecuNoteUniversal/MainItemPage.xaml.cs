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

        public ObservableCollection<AbstractItemModel> ItemModels { get; set; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ItemModels = new ObservableCollection<AbstractItemModel>();
            var files = await  SynchronisationHandler.WorkingDirectory.GetFilesAsync();
            foreach (var storageFile in files)
            {
                ItemModels.Add(new FileItemModel(storageFile));
            }
            ItemsList.ItemsSource = files;
        }
    }
}
