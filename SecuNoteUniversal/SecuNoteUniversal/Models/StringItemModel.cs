using System;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SecuNoteUniversal.Models
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

        public String Content { get; set; }
        public String AuthenticationTag { get; set; }

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