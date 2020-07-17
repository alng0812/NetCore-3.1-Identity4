using System.Text;

namespace NewarePassPort.Common
{
    public static class Codec
    {
        public static string UrlEncode(string sourceString)
        {
            return System.Web.HttpUtility.UrlEncode(sourceString);
        }
        public static string UrlDecode(string sourceString)
        {
            return System.Web.HttpUtility.UrlDecode(sourceString);
        }
        public static string HtmlEncode(string input)
        {
            return System.Web.HttpUtility.HtmlEncode(input);
        }
        public static string HtmlDecode(string input)
        {
            return System.Web.HttpUtility.HtmlDecode(input);
        }
        public static string Base64Encode(string plainText, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return System.Convert.ToBase64String(encoding.GetBytes(plainText));
        }
        public static string Base64Decode(string cipherText, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(System.Convert.FromBase64String(cipherText));
        }
        public static string Base64Encode(byte[] plainBytes)
        {
            return System.Convert.ToBase64String(plainBytes);
        }
        public static byte[] Base64Decode(string cipherText)
        {
            return System.Convert.FromBase64String(cipherText);
        }
    }
}
