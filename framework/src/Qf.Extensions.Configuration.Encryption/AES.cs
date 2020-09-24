using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Qf.Extensions.Configuration.Encryption
{
    public class AES
    {
        public static string EncryptFile(string inputFile, string outputFile, string password)
        {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
            try
            {
                locker.EnterReadLock();
                byte[] encryptedBytes = File.ReadAllBytes(inputFile);
                byte[] passwordToByteArray = System.Text.Encoding.ASCII.GetBytes(password);
                //hash the password with sha256
                passwordToByteArray = SHA256.Create().ComputeHash(passwordToByteArray);
                byte[] encryptedByteArray = GetEncryptedByteArray(encryptedBytes, passwordToByteArray);
                string writeAt = !string.IsNullOrEmpty(outputFile) ? outputFile : inputFile;
                File.WriteAllBytes(writeAt, encryptedByteArray);
                return "encryption succeeded";
            }
            catch (Exception)
            {
                return "encryption failed";
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static string DecryptFile(string inputFile, string outputFile, string password)
        {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
            try
            {
                locker.EnterReadLock();
                byte[] bytesToBeDecrypted = File.ReadAllBytes(inputFile);
                byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
                string writeAt = !string.IsNullOrEmpty(outputFile) ? outputFile : inputFile;
                byte[] bytesDecrypted = GetDecryptedByteArray(bytesToBeDecrypted, passwordBytes);
                File.WriteAllBytes(writeAt, bytesDecrypted);
                return "Decryption succeeded";
            }
            catch (Exception)
            {
                return "Decryption failed";
            }
            finally
            {
                locker.ExitReadLock();
            }
        }
        public static byte[] GetEncryptedByteArray(byte[] encryptedBytes, byte[] password)
        {
            //the salt bytes must be at least 8 bytes
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] encyptedbytes = null;
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(password, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.Zeros;
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                        cs.Close();
                    }
                    encyptedbytes = ms.ToArray();
                }
                return encyptedbytes;
            }
        }

        public static byte[] GetDecryptedByteArray(byte[] bytesDecrypted, byte[] password)
        {
            byte[] decrypted = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(password, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.Zeros;
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesDecrypted, 0, bytesDecrypted.Length);
                        cs.Close();
                    }
                    decrypted = ms.ToArray();
                }
            }
            return decrypted;
        }

    }
}
