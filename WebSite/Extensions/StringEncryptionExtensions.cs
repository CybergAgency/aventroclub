using System.Security.Cryptography;
using System.Text;

namespace WebSite.Extensions
{
    public static class StringEncryptionExtensions
    {
        private const int KeySizeBits = 256;      // AES-256 key
        private const int BlockSizeBits = 128;    // AES block size (fixed)
        private const int SaltSize = 32;          // 256-bit salt
        private const int IvSize = BlockSizeBits / 8; // 16 bytes
        private const int DerivationIterations = 600_000; // daha güvenli

        public static string Encrypt(this string plainText, string passPhrase)
        {
            if (plainText is null) throw new ArgumentNullException(nameof(plainText));
            if (passPhrase is null) throw new ArgumentNullException(nameof(passPhrase));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(IvSize);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                passPhrase, salt, DerivationIterations, HashAlgorithmName.SHA256, KeySizeBits / 8);

            try
            {
                using var aes = Aes.Create();
                aes.KeySize = KeySizeBits;
                aes.BlockSize = BlockSizeBits;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor(key, iv);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }

                byte[] cipher = ms.ToArray();

                // [salt|iv|cipher]
                byte[] output = new byte[salt.Length + iv.Length + cipher.Length];
                Buffer.BlockCopy(salt, 0, output, 0, salt.Length);
                Buffer.BlockCopy(iv, 0, output, salt.Length, iv.Length);
                Buffer.BlockCopy(cipher, 0, output, salt.Length + iv.Length, cipher.Length);

                return Convert.ToBase64String(output);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plainBytes);
            }
        }

        public static string Decrypt(this string cipherText, string passPhrase)
        {
            if (cipherText is null) throw new ArgumentNullException(nameof(cipherText));
            if (passPhrase is null) throw new ArgumentNullException(nameof(passPhrase));

            byte[] all = Convert.FromBase64String(NormalizeBase64Input(cipherText));
            if (all.Length < SaltSize + IvSize)
                throw new FormatException("Invalid encrypted payload.");

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(all, 0, salt, 0, SaltSize);

            byte[] iv = new byte[IvSize];
            Buffer.BlockCopy(all, SaltSize, iv, 0, IvSize);

            int cipherLen = all.Length - SaltSize - IvSize;
            byte[] cipher = new byte[cipherLen];
            Buffer.BlockCopy(all, SaltSize + IvSize, cipher, 0, cipherLen);

            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                passPhrase, salt, DerivationIterations, HashAlgorithmName.SHA256, KeySizeBits / 8);

            try
            {
                using var aes = Aes.Create();
                aes.KeySize = KeySizeBits;
                aes.BlockSize = BlockSizeBits;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor(key, iv);
                using var ms = new MemoryStream(cipher);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (CryptographicException)
            {
                throw new CryptographicException("Decryption failed: wrong passphrase or corrupted data.");
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(cipher);
            }
        }

        public static bool TryDecrypt(this string cipherText, string passPhrase, out string? plainText)
        {
            plainText = null;
            try
            {
                plainText = Decrypt(cipherText, passPhrase);
                return true;
            }
            catch (Exception ex) when (ex is FormatException || ex is CryptographicException || ex is ArgumentException)
            {
                return false;
            }
        }

        private static string NormalizeBase64Input(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new FormatException("Cipher is empty.");

            s = s.Trim();

            s = Uri.UnescapeDataString(s);

            s = s.Replace(' ', '+');

            if (s.Contains('-') || s.Contains('_'))
                s = s.Replace('-', '+').Replace('_', '/');

            s = s.Replace("\r", "").Replace("\n", "");

            int mod = s.Length % 4;
            if (mod != 0) s = s.PadRight(s.Length + (4 - mod), '=');

            return s;
        }
    }
}