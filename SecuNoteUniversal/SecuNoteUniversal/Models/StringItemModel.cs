using System;
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
        public IBuffer AuthenticationTag { get; set; }
    }
}