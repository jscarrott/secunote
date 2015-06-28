using System;
using Windows.Storage.Streams;
using SQLite;

namespace SecuNoteUniversal.Models
{
    internal abstract class AbstractItemModel
    {
        protected AbstractItemModel(int id, String name)
        {
            Id = id;
            IsEncrypted = false;
            Name = name;
        }

        protected AbstractItemModel()
        {
        }

        [PrimaryKey]
        public int Id { get; set; }

        public Boolean IsEncrypted { get; set; }
        public String Name { get; set; }
        public IBuffer Nonce { get; set; }
    }
}