using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using SecuNoteUniversal10.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SecuNoteUniversal10
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewingPage : Page
    {
        private AbstractItemViewModel _item;

        public ViewingPage()
        {
            InitializeComponent();
        }

        private void BackFileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainItemPage));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var items = e.Parameter as AbstractItemViewModel;
            if (items != null) _item = items;
            if (_item is FileItemViewModel)
            {
                var fileItem = _item as FileItemViewModel;
                var type = fileItem.Filetype;
                switch (type)
                {
                    case Constants.Filetype.Picture:
                        var image = new Image();
                        var bitImage = new BitmapImage();
                        await bitImage.SetSourceAsync(await fileItem.File.OpenReadAsync());
                        image.Source = bitImage;
                        ContentPanel.Children.Add(image);
                        break;
                    case Constants.Filetype.Text:
                        var richText = new TextBlock {Text = await FileIO.ReadTextAsync(fileItem.File)};
                        ContentPanel.Children.Add(richText);
                        SaveButton.IsEnabled = true;
                        break;
                }
            }
            else if (_item is StringItemViewModel)
            {
                var stringItem = _item as StringItemViewModel;
                var richText = new TextBlock {Text = stringItem.Content};
                SaveButton.IsEnabled = true;
                ContentPanel.Children.Add(richText);
            }
        }
    }
}