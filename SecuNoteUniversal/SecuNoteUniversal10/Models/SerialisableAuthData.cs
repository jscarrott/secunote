using System.Xml.Serialization;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SecuNoteUniversal10.Models
{
    public class SerialisableAuthData
    {
        public SerialisableAuthData(IBuffer tag, IBuffer data)
        {
            byte[] tagI;
            CryptographicBuffer.CopyToByteArray(tag, out tagI);
            TagBuffer = tagI;
            byte[] tagD;
            CryptographicBuffer.CopyToByteArray(data, out tagD);
            DataBuffer = tagD;
        }

        public SerialisableAuthData()
        {
        }

        [XmlAttribute]
        public byte[] TagBuffer { get; set; }

        [XmlAttribute]
        public byte[] DataBuffer { get; set; }

        public IBuffer GetTag()
        {
            return CryptographicBuffer.CreateFromByteArray(TagBuffer);
        }

        public IBuffer GetData()
        {
            return CryptographicBuffer.CreateFromByteArray(DataBuffer);
        }
    }
}