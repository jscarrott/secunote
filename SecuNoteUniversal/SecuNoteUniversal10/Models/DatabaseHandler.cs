using System.IO;
using Windows.Storage;
using SQLite;

namespace SecuNoteUniversal10.Models
{
    internal static class DatabaseHandler
    {
        private static readonly string _databaseName = "Database.sqlite";
        public static string DBPath = string.Empty;

        public static void SetupDatabase()
        {
            DBPath = Path.Combine(
                ApplicationData.Current.RoamingFolder.Path, _databaseName);
            using (var _db = new SQLiteConnection(DBPath))
            {
                // TODO: Remove this!!!!! For testing only!!!
                //  _db.DropTable<FileItemModel>();
                // Create the tables if they don't exist
                _db.CreateTable<FileItemModel>();
                _db.CreateTable<StringItemModel>();
            }
        }
    }
}