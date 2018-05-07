using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Email_Sender
{
    public class Converter
    {
        /// <summary>
        /// Convert String to Binary
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns></returns>
        public static byte[] StringToBinary(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        /// <summary>
        /// Convert Binary to String
        /// </summary>
        /// <param name="bin">Binary to convert</param>
        /// <param name="start">Starting byte</param>
        /// <param name="length">Count of bytes to convert</param>
        /// <returns></returns>
        public static string BinaryToString(byte[] bin, int start, int length)
        {
            return Encoding.UTF8.GetString(bin, start, length);
        }

        /// <summary>
        /// Convert String to Base64 string
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns></returns>
        public static string StringToBase64(string str)
        {
            return Convert.ToBase64String(StringToBinary(str));

        }
        /// <summary>
        /// Convert Base64 string to String
        /// </summary>
        /// <param name="base64">Base64 string to convert</param>
        /// <returns></returns>
        public static string Base64ToString(string base64)
        {
            var b64Bytes = Convert.FromBase64String(base64);
            return BinaryToString(b64Bytes, 0, b64Bytes.Length);
        }
    }
}
