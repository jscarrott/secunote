using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SecuNoteUniversal.Models
{
    public class SerialisableAuthData
    {
        [XmlAttribute]
        public byte[] TagBuffer { get; set; }
        [XmlAttribute]
        public byte[] DataBuffer { get; set; }

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
