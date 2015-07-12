using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using SecuNoteUniversal.ViewModels;

namespace SecuNoteUniversal.Models
{
    internal class Encryptor
    {
        private CryptographicKey aesKey;

        public async Task Initialise(string password)
        {
            await CreateAesKey(password);
        }

        //String strMsg = "1234567812345678";     // Data to encrypt.
        //String strAlgName = SymmetricAlgorithmNames.AesCbc;
        //UInt32 keyLength = 32;                  // Length of the key, in bytes
        //BinaryStringEncoding encoding;          // Binary encoding value
        //IBuffer iv;                             // Initialization vector
        //CryptographicKey key;

        public async Task EncryptFileAes( FileItemViewModel fileToEncrypt)
        {
            if (!fileToEncrypt.IsEncrypted)
            {
                var fileBuffer = await FileIO.ReadBufferAsync(fileToEncrypt.File);
                CreateNonce(fileToEncrypt);
                EncryptedAndAuthenticatedData encryptedData = CryptographicEngine.EncryptAndAuthenticate(aesKey, fileBuffer, fileToEncrypt.Nonce,
                    null);
                
                var serialData = new SerialisableAuthData(encryptedData.AuthenticationTag, encryptedData.EncryptedData);
                
                XmlSerializer serializer = new XmlSerializer(typeof(SerialisableAuthData));
                using (Stream stream = await fileToEncrypt.File.OpenStreamForWriteAsync())
                {
                    TextWriter output = new StreamWriter(stream);
                    serializer.Serialize(output, serialData);
                    await stream.FlushAsync();
                    output.Dispose();
                    stream.Dispose();
                }
                fileToEncrypt.IsEncrypted = true;

            }
            else
            {
                throw new Exception("Tried to encrypt file with encrypted flag already set to true");
            }
        }

        public async Task DecryptFileAes(FileItemViewModel fileToDecrypt)
        {
            if (fileToDecrypt.IsEncrypted)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerialisableAuthData));
                SerialisableAuthData tagData = null;
                using (Stream stream = await fileToDecrypt.File.OpenStreamForReadAsync())
                {
                    tagData = serializer.Deserialize(stream) as SerialisableAuthData;
                    stream.Dispose();
                }
                if (tagData != null)
                {
                    IBuffer data = tagData.GetData();
                    IBuffer tag = tagData.GetTag();
                    var decryptedData = CryptographicEngine.DecryptAndAuthenticate(aesKey, data,
                        fileToDecrypt.Nonce, tag, null);
                    await FileIO.WriteBufferAsync(fileToDecrypt.File, decryptedData);
                }
                fileToDecrypt.IsEncrypted = false;

            }
            else
            {
                throw new Exception("Tried to dencrypt file with encrypted flag already set to false");
            }
        }

        public void EncryptString(StringItemViewModel stringToEncrypt)
        {
            if (stringToEncrypt.IsEncrypted)
            {
                IBuffer stringBuffer = CryptographicBuffer.ConvertStringToBinary(stringToEncrypt.Content, BinaryStringEncoding.Utf8);
                var encryptedString = CryptographicEngine.EncryptAndAuthenticate(aesKey, stringBuffer, stringToEncrypt.Nonce,
                    null);
                stringToEncrypt.AuthenticationTag = encryptedString.AuthenticationTag;
                stringToEncrypt.Content = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, encryptedString.EncryptedData);
                stringToEncrypt.IsEncrypted = true;
            }
            else
            {
                throw new Exception("Tried to encrypt string with encrypted flag set to true");
            }
        }

        public void DecryptString(StringItemViewModel stringToDecrypt)
        {
            if (stringToDecrypt.IsEncrypted)
            {
                IBuffer stringBuffer = CryptographicBuffer.ConvertStringToBinary(stringToDecrypt.Content, BinaryStringEncoding.Utf8);
                var decryptedString = CryptographicEngine.DecryptAndAuthenticate(aesKey, stringBuffer, stringToDecrypt.Nonce,
                    stringToDecrypt.AuthenticationTag, null);
                stringToDecrypt.Content = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8,
                    decryptedString);
                stringToDecrypt.IsEncrypted = false;
            }
            else
            {
                throw new Exception("Tried to encrypt string with encrypted flag set to false");
            }
        }

        public void CreateNonce(FileItemViewModel fileToEncrypt)
        {
            fileToEncrypt.Nonce = CryptographicBuffer.GenerateRandom(12);
        }

        /// <summary>
        ///     Turns string password into a hash for use in AES
        ///     TODO remove magic numbers
        /// </summary>
        /// <param name="password"></param>
        public async Task CreateAesKey(string password)
        {
            var hashProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha512);
            var passwordBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            var salt = await GetSalt();
            //var salt = await GenerateAndSaveSalt();

            var keyCreationParameters = KeyDerivationParameters.BuildForPbkdf2(salt, 1000);

            var originalKey = hashProvider.CreateKey(passwordBuffer);

            var hashedKeyMaterial = CryptographicEngine.DeriveKeyMaterial(originalKey, keyCreationParameters, 32);
            var aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesGcm);
            var key = aesProvider.CreateSymmetricKey(hashedKeyMaterial);

            aesKey = key;
        }

        private async Task<IBuffer> GenerateAndSaveSalt()
        {
            var salt = CryptographicBuffer.GenerateRandom(32);
            var saltFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync("salt");
            try
            {
                await
                    FileIO.WriteBufferAsync(saltFile, salt);
            }
            catch (Exception)
            {
                
                
            }
            
            return salt;
        }

        private async Task<IBuffer> GetSalt()
        {
            StorageFile saltFile;
            
               saltFile = await ApplicationData.Current.RoamingFolder.GetFileAsync("salt");
            if (saltFile == null)
            {
                GenerateAndSaveSalt();
            }
                var stringSalt = await FileIO.ReadBufferAsync(saltFile);


            return stringSalt;
        }
    }
}