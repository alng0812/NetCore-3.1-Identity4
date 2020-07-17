using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewarePassPort.Common
{
    public static class Convert
    {
        public static bool TryParse(string s, bool defaultResult = false)
        {
            bool result = false;
            if (bool.TryParse(s, out result))
                return result;
            else
                return defaultResult;
        }
        public static int TryParse(string s, int defaultResult = 0)
        {
            int result = 0;
            if (int.TryParse(s, out result))
                return result;
            else
                return defaultResult;
        }
        public static long TryParse(string s, long defaultResult = 0)
        {
            long result = 0;
            if (long.TryParse(s, out result))
                return result;
            else
                return defaultResult;
        }
        public static float TryParse(string s, float defaultResult = 0)
        {
            float result = 0;
            if (float.TryParse(s, out result))
                return result;
            else
                return defaultResult;
        }
        public static decimal TryParse(string s, decimal defaultResult = 0)
        {
            decimal result = 0;
            if (decimal.TryParse(s, out result))
                return result;
            else
                return defaultResult;
        }
        public static double TryParse(string s, double defaultResult = 0)
        {
            double result = 0;
            if (double.TryParse(s, out result))
                return result;
            else
                return defaultResult;
        }
        public static byte[] StreamToBytes(Stream s)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static Stream BytesToStream(byte[] b)
        {
            return new MemoryStream(b);
        }
        public static string StreamToString(Stream s, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            string result = string.Empty;
            using (StreamReader sr = new StreamReader(s, encoding))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }
        public static Stream StringToStream(string s, Encoding encoding = null)
        {
            return new MemoryStream(Convert.StringToBytes(s));
        }
        public static bool StreamToFile(Stream s, string filePath)
        {
            try
            {
                IO.DirectoryCreate(filePath);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    s.CopyTo(fs);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static byte[] StringToBytes(string s, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetBytes(s);
        }
        public static string BytesToString(byte[] b, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(b);
        }
        public static bool BytesToFile(byte[] b, string filePath)
        {
            try
            {
                IO.DirectoryCreate(filePath);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(b, 0, b.Length);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static string DictionaryToString<TKey, TValue>(IDictionary<TKey, TValue> dict, string arraySplit, string keyValueSplit)
            where TKey : IConvertible
            where TValue : IConvertible
        {
            string s = string.Empty;
            if (dict != null && dict.Count > 0)
                s = string.Join(arraySplit, dict.Select(item => item.Key + keyValueSplit + item.Value));
            return s;
        }
        public static IDictionary<TKey, TValue> StringToDictionary<TKey, TValue>(string sourceString, char arraySplit, char keyValueSplit, bool duplicateCover)
            where TKey : IConvertible
            where TValue : IConvertible
        {
            if (sourceString == null || sourceString.Length == 0)
                return null;
            string[] arr1 = sourceString.TrimEnd(arraySplit).Split(arraySplit);
            if (arr1 == null || arr1.Length == 0)
                return null;
            Dictionary<TKey, TValue> d = new Dictionary<TKey, TValue>();
            foreach (string s in arr1)
            {
                string[] arr2 = s.Split(keyValueSplit);
                if (arr2 == null || arr2.Length != 2)
                    continue;
                TKey _k = default(TKey);
                TValue _v = default(TValue);
                try
                {
                    _k = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(arr2[0]);
                    _v = (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFromString(arr2[1]);
                }
                catch
                {
                    continue;
                }
                if (!d.ContainsKey(_k))
                {
                    d.Add(_k, _v);
                }
                else if (duplicateCover)
                {
                    d[_k] = _v;
                }
            }
            return d;
        }
        public static IDictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }
        public static double ByteToKb(long bytes, int digits = 0)
        {
            return System.Math.Round((double)bytes / 1024, digits);
        }
        public static double ByteToMb(long bytes, int digits = 0)
        {
            return System.Math.Round((double)bytes / (1024 * 1024), digits);
        }
        public static double ByteToGb(long bytes, int digits = 2)
        {
            return System.Math.Round((double)bytes / (1024 * 1024 * 1024), digits);
        }
    }
}
