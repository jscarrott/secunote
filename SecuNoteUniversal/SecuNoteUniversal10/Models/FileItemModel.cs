using System;
using Windows.Storage;

namespace SecuNoteUniversal10.Models
{
    internal class FileItemModel : AbstractItemModel
    {
        //TODO make sure string FileType is converted to the enum correctly
        public FileItemModel(int id, bool encrypted, string name, string token, Type theType) : base(id, name)
        {
            MruToken = token;
            FileType = theType.ToString();
        }

        public FileItemModel()
        {
        }

        public FileItemModel(StorageFile fileIn)
        {
            IsEncrypted = false;
            Name = fileIn.Name;
            FileType = fileIn.FileType;
        }

        public string MruToken { get; set; }
        public string FileType { get; set; }
    }
}