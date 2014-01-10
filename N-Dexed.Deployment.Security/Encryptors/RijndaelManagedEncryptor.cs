using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Security.Encryptors
{
    public class RijndaelManagedEncryptor: IEncryptor
    {
        private const string CRYPTO_KEY = "N-Dexed.Deployment";
        private const string INITIAL_VECTOR = "ARandomStringThatContainsLotsOfCharactersUsedAsAnInitalVectorForEncryption";

        public string EncryptValue(string value)
        {
            string returnValue = null;

            if (!string.IsNullOrEmpty(value))
            {
                RijndaelManaged cryptoProvider = new RijndaelManaged
                {
                    Key = GenerateHash(CRYPTO_KEY),
                    IV = GenerateHash(INITIAL_VECTOR)
                };

                ICryptoTransform transform = cryptoProvider.CreateEncryptor(cryptoProvider.Key, cryptoProvider.IV);

                MemoryStream stream = new MemoryStream();
                using (stream)
                {
                    CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                    using (cryptoStream)
                    {
                        byte[] data = Encoding.ASCII.GetBytes(value);

                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();

                        byte[] encryptedData = stream.ToArray();

                        returnValue = Convert.ToBase64String(encryptedData);
                    }
                }
            }

            return returnValue;
        }

        public string DecryptValue(string encryptedValue)
        {
            string returnValue = null;

            if (!string.IsNullOrEmpty(encryptedValue))
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedValue);
                RijndaelManaged cryptoProvider = new RijndaelManaged
                {
                    Key = GenerateHash(CRYPTO_KEY),
                    IV = GenerateHash(INITIAL_VECTOR)
                };

                ICryptoTransform transform = cryptoProvider.CreateDecryptor(cryptoProvider.Key, cryptoProvider.IV);

                MemoryStream stream = new MemoryStream(encryptedBytes);
                using (stream)
                {
                    CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
                    using (cryptoStream)
                    {
                        StreamReader reader = new StreamReader(cryptoStream);
                        using (reader)
                        {
                            returnValue = reader.ReadToEnd();

                        }
                    }
                }
            }
         
            return returnValue;
        }

        #region Private Methods

        private static byte[] GenerateHash(string value)
        {
            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.ASCII.GetBytes(value);
            byte[] hash = md5.ComputeHash(bytes);

            return hash;
        }

        #endregion
    }
}
