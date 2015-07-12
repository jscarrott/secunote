using System;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using SQLite;

namespace SecuNoteUniversal.Models
{
    public abstract class AbstractItemModel
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

        //[PrimaryKey]
        public int Id { get; set; }

        public Boolean IsEncrypted { get; set; }
        public String Name { get; set; }
        public String Nonce { get; set; }


        public void SetNonce(IBuffer nonce)
        {
            Nonce = CryptographicBuffer.EncodeToBase64String(nonce);
        }

        public IBuffer GetNonce()
        {
            return CryptographicBuffer.DecodeFromBase64String(Nonce);
        }
    }
}