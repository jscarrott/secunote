using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SecuNoteUniversal10.Models
{
    public abstract class AbstractItemModel
    {
        protected AbstractItemModel(int id, string name)
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
        public bool IsEncrypted { get; set; }
        public string Name { get; set; }
        public string Nonce { get; set; }

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