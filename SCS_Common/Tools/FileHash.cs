using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;


namespace SCS.Common
{
    /// <summary>
    /// 파일 해시값을 구합니다.
    /// </summary>
    public static class FileHash
    {
        /// <summary>
        /// path의 파일에 대한 SHA256 해시값을 구해 Hex로 나타내는 16진수로 반환합니다.
        /// </summary>
        public static string GetFileSHA256Str(string path)
        {
            return ByteToHex(GetFileSHA256Byte(path));
        }

        /// <summary>
        /// path의 파일에 대한 SHA256 해시값을 byte 배열로 반환합니다.
        /// </summary>
        public static byte[] GetFileSHA256Byte(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                using (var sha256 = new SHA256Managed())
                {
                    return sha256.ComputeHash(fs);
                }
            }
        }

        /// <summary>
        /// byte 배열을 Hex 코드 문자열로 전환합니다.
        /// </summary>
        private static string ByteToHex(byte[] value)
        {
            Debug.Assert(value != null && value.Length > 0);

            int charArrLength = value.Length * 2;
            char[] charArray = new char[charArrLength];
            int num2 = 0;
            byte b;

            for (int i = 0; i < charArrLength; i += 2)
            {
                b = value[num2++];
                charArray[i] = byteToHex_GetHexValue(b / 16);
                charArray[i + 1] = byteToHex_GetHexValue(b % 16);
            }
            return new string(charArray);
        }

        /// <summary>
        /// <see cref="ByteToHex(byte[])"/> 함수에서 내부적으로 사용하는 함수입니다.
        /// </summary>
        private static char byteToHex_GetHexValue(int i)
        {
            Debug.Assert(i >= 0);

            if (i < 10)
                return (char)(i + 48);
            return (char)(i + 55);
        }
    }
}
