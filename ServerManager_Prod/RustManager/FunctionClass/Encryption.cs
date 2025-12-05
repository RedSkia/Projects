using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace RustManager.FunctionClass
{
    class Encryption
    {

        public class AES
        {

            static byte[] SecretKey = { 0x92, 0x02, 0x74, 0x39, 0x84, 0x62, 0x07, 0x46, 0x58, 0x11, 0x26, 0x30, 0x17, 0x05, 0x41, 0x37 };
            static byte[] PublicKey = { 0x19, 0x71, 0x34, 0x90, 0x62, 0x15, 0x54, 0x43, 0x10, 0x51, 0x86, 0x40, 0x32, 0x61, 0x83, 0x25 };

            public static byte[] Encrypt(string TextToEncrypt)
            {
                byte[] EncryptedBytes;

                using (Aes Algorithm = Aes.Create())
                {
                    Algorithm.Key = SecretKey;
                    Algorithm.IV = PublicKey;

                    ICryptoTransform Encryptor = Algorithm.CreateEncryptor(Algorithm.Key, Algorithm.IV);


                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(TextToEncrypt);
                            }
                            EncryptedBytes = msEncrypt.ToArray();
                        }
                    }

                }
                return EncryptedBytes;
            }

            public static string Decrypt(byte[] BytesToDecrypt)
            {
                string DecryptedText;

                using (Aes Algorithm = Aes.Create())
                {
                    Algorithm.Key = SecretKey;
                    Algorithm.IV = PublicKey;

                    ICryptoTransform Decryptor = Algorithm.CreateDecryptor(Algorithm.Key, Algorithm.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(BytesToDecrypt))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                DecryptedText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return DecryptedText;
            }
        }


        public static class ForwardBackward
        {
            const int ForwardBackwardKey = 94 * 27 * 76 * 15 * 41;
            public static string TextForwardBackwards(string PlainText)
            {
                StringBuilder szInputStringBuild = new StringBuilder(PlainText);
                StringBuilder szOutStringBuild = new StringBuilder(PlainText.Length);
                char Textch;
                for (int iCount = 0; iCount < PlainText.Length; iCount++)
                {
                    Textch = szInputStringBuild[iCount];
                    Textch = (char)(Textch ^ ForwardBackwardKey);
                    szOutStringBuild.Append(Textch);
                }
                return szOutStringBuild.ToString();
            }
        }

        public static class Binary
        {

            public static string StringToBinary(string data)
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in data.ToCharArray())
                {
                    sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
                }
                return sb.ToString();
            }
            public static string BinaryToString(string data)
            {
                List<Byte> byteList = new List<Byte>();

                for (int i = 0; i < data.Length; i += 8)
                {
                    byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
                }
                return Encoding.ASCII.GetString(byteList.ToArray());
            }



        }
        public static class ShuffleEncryption
        {
            const int ShuffleKey = 736142958;
            public static int[] GetShuffleExchanges(int size)
            {
                int[] exchanges = new int[size - 1];
                var rand = new Random(ShuffleKey);
                for (int i = size - 1; i > 0; i--)
                {
                    int n = rand.Next(i + 1);
                    exchanges[size - 1 - i] = n;
                }
                return exchanges;
            }

            public static string Shuffle(string toShuffle)
            {
                int size = toShuffle.Length;
                char[] chars = toShuffle.ToArray();
                var exchanges = GetShuffleExchanges(size);
                for (int i = size - 1; i > 0; i--)
                {
                    int n = exchanges[size - 1 - i];
                    char tmp = chars[i];
                    chars[i] = chars[n];
                    chars[n] = tmp;
                }
                return new string(chars);
            }

            public static string DeShuffle(string shuffled)
            {
                int size = shuffled.Length;
                char[] chars = shuffled.ToArray();
                var exchanges = GetShuffleExchanges(size);
                for (int i = 1; i < size; i++)
                {
                    int n = exchanges[size - i - 1];
                    char tmp = chars[i];
                    chars[i] = chars[n];
                    chars[n] = tmp;
                }
                return new string(chars);
            }
        }
    }
}
