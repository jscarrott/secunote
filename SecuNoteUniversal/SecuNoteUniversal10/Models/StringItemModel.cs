using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SecuNoteUniversal10.Models
{
    internal class StringItemModel : AbstractItemModel
    {
        private StringItemModel(int id, string name, string content) : base(id, name)
        {
            Content = content;
        }

        public StringItemModel()
        {
        }

        public string Content { get; set; }
        public string AuthenticationTag { get; set; }

        public void SetAuthenticationTag(IBuffer tag)
        {
            AuthenticationTag = CryptographicBuffer.EncodeToBase64String(tag);
        }

        public IBuffer GetAuthenicationTag()
        {
            return CryptographicBuffer.DecodeFromBase64String(AuthenticationTag);
        }
    }
}