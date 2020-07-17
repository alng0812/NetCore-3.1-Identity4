using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NewarePassPort.Common
{
    public static class IO
    {
        private static readonly object syncLock = new object();
        public static string DirectoryWindows()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        }
        public static string DirectoryCurrent()
        {
            return Directory.GetCurrentDirectory();
        }
        public static void DirectoryCreate(string path)
        {
            path = Path.GetDirectoryName(path);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public static void DirectoryDelete(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string item in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(item))
                        File.Delete(item);
                    else
                        DirectoryDelete(item);
                }
                Directory.Delete(path, true);
            }
        }
        public static void DirectoryCopy(string sourcePath, string targetPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);
                DirectoryInfo sourceInfo = new DirectoryInfo(sourcePath);
                FileInfo[] files = sourceInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    File.Copy(Path.Combine(sourcePath, file.Name), Path.Combine(targetPath, file.Name), true);
                }
                DirectoryInfo[] dirs = sourceInfo.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    string currentSource = dir.FullName;
                    string currentTarget = dir.FullName.Replace(sourcePath, targetPath);
                    Directory.CreateDirectory(currentTarget);
                    DirectoryCopy(currentSource, currentTarget);
                }
            }
        }
        public static string FileNameExtension(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(".") + 1);
        }
        public static void FileDelete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        public static string TextRead(string path, Encoding encoding = null)
        {
            string content = string.Empty;
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, encoding == null ? Encoding.UTF8 : encoding))
                {
                    content = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return content;
        }
        public static void TextWrite(string path, string content, bool isAppend = false, Encoding encoding = null)
        {
            lock (syncLock)
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (StreamWriter sw = new StreamWriter(path, isAppend, encoding == null ? Encoding.UTF8 : encoding))
                {
                    if (isAppend)
                    {
                        sw.WriteLine("");
                        sw.WriteLine("-------------------------------------------------------------");
                        sw.WriteLine("");
                    }
                    sw.Write(content);
                    sw.Close();
                }
            }
        }


        /// <summary>
        /// 接口通用日志
        /// </summary>
        /// <param name="RouteName">日志存放的路径名</param>
        /// <param name="Parameter">日志请求参数</param>
        /// <param name="Source">日志来源</param>
        /// <param name="Message">日志信息</param>
        /// <param name="isAppend">是否追加，默认追加</param>
        /// <param name="encoding">Encoding编码默认UTF8</param>
        public static void ApiTextWrite(string RouteName, string Parameter, string Source, string Message = "", bool isAppend = true, Encoding encoding = null)
        {
            try
            {
                var _Path = $"{AppDomain.CurrentDomain.BaseDirectory}/{RouteName}/{DateTime.Now.ToString("yyyyMMdd")}.txt";
                lock (syncLock)
                {
                    string dir = Path.GetDirectoryName(_Path);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    using (StreamWriter sw = new StreamWriter(_Path, isAppend, encoding == null ? Encoding.UTF8 : encoding))
                    {
                        sw.WriteLine("DateTime:" + DateTime.Now);
                        sw.WriteLine("");
                        sw.WriteLine("Source:" + Source);
                        sw.WriteLine("");
                        sw.WriteLine("Parameter:" + Parameter);
                        sw.WriteLine("");
                        sw.WriteLine("Message:" + Message);
                        sw.WriteLine("");
                        sw.WriteLine("-----------------------------------------------------------------------------------");
                        sw.WriteLine("");
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 创建XML节点,并设置属性
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="parentNode"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string nodeName, string nodeValue, Dictionary<string, string> attributes)
        {
            var node = xmlDoc.CreateElement(nodeName);
            if (!string.IsNullOrEmpty(nodeValue))
            {
                node.InnerText = nodeValue;
            }
            if (attributes != null && attributes.Count > 0)
            {
                foreach (var attr in attributes)
                {
                    node.SetAttribute(attr.Key, attr.Value);
                }
            }
            parentNode.AppendChild(node);
            return node;
        }
    }
}
