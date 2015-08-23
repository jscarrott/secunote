using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OneDrive;

namespace SecuNoteUniversal10.Models
{
    internal static class OneDriveConnectionHelper
    {
        private const string AppFolderName = "SecuNote";
        private const string AppsFolderName = "Apps";
        public static ODConnection Connection { get; set; }
        public static ODItem RootFiles { get; set; }
        public static ODItem AppFolder { get; set; }

        /// <summary>
        ///     Login and create the connection, if necessary.
        /// </summary>
        public static async Task Login()
        {
            if (Connection == null || !LiveIdAuth.IsSignedIn)
            {
                // Get an OAuth2 access token through REST.           
                var token = await LiveIdAuth.GetAuthToken();

                // Initialize connection
                Connection = new ODConnection("https://api.onedrive.com/v1.0", new OneDriveSdkAuthenticationInfo(token));
            }
        }

        public static void EnsureConnection()
        {
            if (Connection == null || !LiveIdAuth.IsSignedIn)
            {
                if (Debugger.IsAttached) Debugger.Break();
                throw new Exception("You're not logged in.");
            }
        }

        public static async Task<ODItem> GetOneDriveRoot()
        {
            EnsureConnection();
            var rootFolder = await Connection.GetRootItemAsync(ItemRetrievalOptions.DefaultWithChildren);
            return rootFolder;
        }

        public static async Task SetAppFolder()
        {
            await GetRootFiles();
            await GetAppFolder();
        }

        private static async Task GetRootFiles()
        {
            RootFiles = await GetOneDriveRoot();
        }

        /// <summary>
        ///     Checks whether the main app folder and the working folder exists and if it does returns the working folder
        ///     else creates the folders and returns them.
        /// </summary>
        /// <returns>
        ///     App Folder
        /// </returns>
        public static async Task GetAppFolder()
        {
            EnsureConnection();
            await GetRootFiles();
            var appsFolder = FindItem(AppsFolderName, RootFiles);
            if (appsFolder == null)
            {
                await GetOrCreateFolder(appsFolder, AppsFolderName, RootFiles);
                await GetRootFiles();
            }
            appsFolder =
                await Connection.GetItemAsync(appsFolder.ItemReference(), ItemRetrievalOptions.DefaultWithChildren);
            var appFolder = FindItem(AppFolderName, appsFolder);
            await GetOrCreateFolder(appFolder, AppFolderName, appsFolder);

            AppFolder = appFolder;
        }

        /// <summary>
        ///     Creates folder if none exists else returns the folder
        /// </summary>
        public static async Task<ODItem> GetOrCreateFolder(ODItem itemToGet, string name, ODItem rootItem)
        {
            if (itemToGet != null)
            {
                if (itemToGet.Folder == null)
                {
                    throw new Exception("A file with the same name as the specified app folder already exists.");
                }
                return itemToGet;
            }
            var newFolder = await Connection.CreateFolderAsync(rootItem.ItemReference(), name);
            return newFolder;
        }

        private static ODItem FindItem(string itemName, ODItem root)
        {
            if (root.Children != null)
            {
                var existingItem = (from f in root.Children
                    where f.Name == itemName
                    select f).FirstOrDefault();
                return existingItem;
            }
            return null;
        }

        public static async Task<ODItem> CreateFile(ODItem parentItem, string itemName, Stream itemContentStream)
        {
            EnsureConnection();
            return
                await
                    Connection.PutNewFileToParentItemAsync(parentItem.ItemReference(), itemName, itemContentStream,
                        ItemUploadOptions.Default);
        }

        public static async Task<ODItem> ModifyFile(ODItem parentItem, string itemName, Stream itemContentStream)
        {
            EnsureConnection();
            return
                await
                    Connection.PutNewFileToParentItemAsync(parentItem.ItemReference(), itemName, itemContentStream,
                        ItemUploadOptions.Default);
        }

        public static async Task<bool> DeleteFile(ODItem itemToDelete, string itemName)
        {
            EnsureConnection();
            return
                await
                    Connection.DeleteItemAsync(itemToDelete.ItemReference(), ItemDeleteOptions.Default);
        }

        public static async Task<List<ODItem>> GetODItemsFromAppFolder()
        {
            EnsureConnection();
            AppFolder =
                await Connection.GetItemAsync(AppFolder.ItemReference(), ItemRetrievalOptions.DefaultWithChildren);
            var itemsToReturn = AppFolder.Children.ToList();
            return itemsToReturn;
        }
    }
}