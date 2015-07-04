using System;
using Windows.Storage;

namespace SecuNoteUniversal.Models
{
    internal class FileItemModel : AbstractItemModel
    {
        //TODO make sure string FileType is converted to the enum correctly
        public FileItemModel(int id, Boolean encrypted, String name, String token, Type theType) : base(id, name)
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

        public String MruToken { get; set; }
        public String FileType { get; set; }
    }
}