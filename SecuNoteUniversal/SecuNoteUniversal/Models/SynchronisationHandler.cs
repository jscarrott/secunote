using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using OneDrive;

namespace SecuNoteUniversal.Models
{
    public static class SynchronisationHandler
    {
        public static string WorkingDirectoryString { get; set; }
        public static StorageFolder WorkingDirectory;
        private static Dictionary<string, StorageFile> _workingDirFiles = new Dictionary<string, StorageFile>();
        private static Dictionary<string, ODItem> _odAppFolderItems = new Dictionary<string, ODItem>(); 

        public async static void Initialise(string directoryPath)
        {
            WorkingDirectoryString = directoryPath;
            await CheckWorkingDirExistsElseCreateIt();
            //GetFilesFromCloud();
            await GetFileFromWorkingDirectory();
            //GetNewFilesFromCloud();
            //GetUpdatedFilesFromCloud();
        }

        public static async Task CheckWorkingDirExistsElseCreateIt()
        {
            var dir =
                await ApplicationData.Current.LocalFolder.TryGetItemAsync(WorkingDirectoryString);
            if(dir == null)
            {
                await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync(WorkingDirectoryString);
            }
        }

        public static async Task GetFilesFromCloud()
        {
            await OneDriveConnectionHelper.GetAppFolder();
            var list = OneDriveConnectionHelper.GetODItemsFromAppFolder();
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
            foreach (var odAppFolderItem in _odAppFolderItems.Where(odAppFolderItem => !_workingDirFiles.ContainsKey(odAppFolderItem.Key)))
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
                if (workingDirFile.Value.GetBasicPropertiesAsync().GetResults().DateModified >
                    _odAppFolderItems[workingDirFile.Key].LastModifiedDateTime)
                {
                    await OneDriveConnectionHelper.CreateFile(OneDriveConnectionHelper.AppFolder, workingDirFile.Value.Name,
                        await workingDirFile.Value.OpenStreamForReadAsync());
                }
            }
        }
    }
}
