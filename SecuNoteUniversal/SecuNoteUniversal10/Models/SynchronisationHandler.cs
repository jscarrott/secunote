using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using OneDrive;
using SecuNoteUniversal10.ViewModels;

namespace SecuNoteUniversal10.Models
{
    public static class SynchronisationHandler
    {
        public static StorageFolder WorkingDirectory;
        private static readonly Dictionary<string, StorageFile> _workingDirFiles = new Dictionary<string, StorageFile>();
        private static readonly Dictionary<string, ODItem> _odAppFolderItems = new Dictionary<string, ODItem>();
        public static string WorkingDirectoryString { get; set; }

        public static async void Initialise(string directoryPath)
        {
            await OneDriveConnectionHelper.Login();
            WorkingDirectoryString = directoryPath;
            await CheckWorkingDirExistsElseCreateIt();
            await GetFilesFromCloud();
            await GetFileFromWorkingDirectory();
            await GetNewFilesFromCloud();
            await GetUpdatedFilesFromCloud();
        }

        public static async Task CheckWorkingDirExistsElseCreateIt()
        {
            var dir =
                await ApplicationData.Current.LocalFolder.TryGetItemAsync(WorkingDirectoryString);
            if (dir == null)
            {
                await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync(WorkingDirectoryString);
            }
        }

        public static async Task GetFilesFromCloud()
        {
            await OneDriveConnectionHelper.GetAppFolder();
            var list = await OneDriveConnectionHelper.GetODItemsFromAppFolder();
            foreach (var odItem in list)
            {
                _odAppFolderItems.Add(odItem.Name, odItem);
            }
        }

        public static async Task GetFileFromWorkingDirectory()
        {
            WorkingDirectory = await
                ApplicationData.Current.LocalFolder.GetFolderAsync(WorkingDirectoryString);
            var files = await WorkingDirectory.GetFilesAsync();
            foreach (var file in files)
            {
                _workingDirFiles.Add(file.Name, file);
            }
        }

        public static async Task GetNewFilesFromCloud()
        {
            foreach (
                var odAppFolderItem in
                    _odAppFolderItems.Where(odAppFolderItem => !_workingDirFiles.ContainsKey(odAppFolderItem.Key)))
            {
                var fileOutStream = await
                    WorkingDirectory.OpenStreamForWriteAsync(odAppFolderItem.Value.Name,
                        CreationCollisionOption.FailIfExists);
                var fileInStream =
                    await
                        odAppFolderItem.Value.GetContentStreamAsync(OneDriveConnectionHelper.Connection,
                            StreamDownloadOptions.Default);

                await fileInStream.CopyToAsync(fileOutStream);
                await fileOutStream.FlushAsync();
                await fileInStream.FlushAsync();
                fileInStream.Dispose();
                fileOutStream.Dispose();
            }
        }

        public static async Task GetUpdatedFilesFromCloud()
        {
            foreach (
                var odAppFolderItem in
                    _odAppFolderItems.Where(
                        odAppFolderItem =>
                            (_workingDirFiles.ContainsKey(odAppFolderItem.Key)) &&
                            (odAppFolderItem.Value.LastModifiedDateTime >
                             _workingDirFiles[odAppFolderItem.Key].GetBasicPropertiesAsync().GetResults().DateModified))
                )
            {
                var fileOutStream = await
                    WorkingDirectory.OpenStreamForWriteAsync(odAppFolderItem.Value.Name,
                        CreationCollisionOption.ReplaceExisting);
                var fileInStream =
                    await
                        odAppFolderItem.Value.GetContentStreamAsync(OneDriveConnectionHelper.Connection,
                            StreamDownloadOptions.Default);
                await fileInStream.CopyToAsync(fileOutStream);
                await fileOutStream.FlushAsync();
                await fileInStream.FlushAsync();
                fileInStream.Dispose();
                fileOutStream.Dispose();
            }
        }

        public static async Task PushModifiedItemsToTheCLoud()
        {
            foreach (var workingDirFile in _workingDirFiles)
            {
                if (_odAppFolderItems.ContainsKey(workingDirFile.Key))
                {
                    var props = await workingDirFile.Value.GetBasicPropertiesAsync();
                    if (props.DateModified <= _odAppFolderItems[workingDirFile.Key].LastModifiedDateTime) continue;
                    var stream = await workingDirFile.Value.OpenStreamForReadAsync();
                    await
                        OneDriveConnectionHelper.CreateFile(OneDriveConnectionHelper.AppFolder,
                            workingDirFile.Value.Name, stream
                            );
                }
                else
                {
                    var stream = await workingDirFile.Value.OpenStreamForReadAsync();
                    await
                        OneDriveConnectionHelper.CreateFile(OneDriveConnectionHelper.AppFolder,
                            workingDirFile.Value.Name, stream
                            );
                }
            }
        }

        public static async Task<FileItemViewModel> AddTotalyNewFile(StorageFile fileIn)
        {
            try
            {
                var newFile = await WorkingDirectory.CreateFileAsync(fileIn.Name, CreationCollisionOption.FailIfExists);
                var fileItem = new FileItemViewModel(newFile);
                await fileIn.CopyAndReplaceAsync(newFile);
                fileItem.SaveItem(fileItem);
                return fileItem;
            }
            catch (Exception)
            {
                var warning =
                    new MessageDialog(
                        "Warning the file already exists in the main app directory, do you wish to overwrite it?")
                    {
                        Title = "Warning!"
                    };
                warning.Commands.Add(new UICommand {Label = "Ok", Id = 0});
                warning.Commands.Add(new UICommand {Label = "No", Id = 1});
                var res = await warning.ShowAsync();
                if ((int) res.Id == 0)
                {
                    var newFile =
                        await WorkingDirectory.CreateFileAsync(fileIn.Name, CreationCollisionOption.ReplaceExisting);
                    await fileIn.CopyAndReplaceAsync(newFile);
                    var fileItem = new FileItemViewModel(newFile);
                    fileItem.SaveItem(fileItem);
                    return fileItem;
                }
            }

            return null;
        }
    }
}