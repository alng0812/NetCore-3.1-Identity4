using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Minio;
using NetCoreWebAPI.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Business.MinIO
{
    public class MinioFileStore : IFileStore
    {
        static long MB = 1024 * 1024;
        private readonly MinioClient _minioClient;

        public MinioFileStore(MinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task UploadFile(IFormFile file)
        {
            var folderYear = DateTime.Now.ToString("yyyy");
            var folderMonth = DateTime.Now.ToString("MM");
            var folderDay = DateTime.Now.ToString("dd");
            var bucketName = folderYear; ;// Path.Combine(folderYear, folderMonth, folderDay);
            // Make a bucket on the server, if not already present.
            bool found = await _minioClient.BucketExistsAsync(bucketName);
            if (!found)
            {
                await _minioClient.MakeBucketAsync(bucketName);
            }
            if (file != null && file.Length > 0 && file.Length < 100 * MB)
            {
                var fileName = file.FileName;

                var ext = Path.GetExtension(fileName).ToLower();
                string newName = $"{folderMonth}/{folderDay}/{GenerateId.GenerateOrderNumber() + ext}";
                //string thumbnailName = "";
                //if (ext.Contains(".jpg") || ext.Contains(".jpeg") || ext.Contains(".png") || ext.Contains(".bmp") || ext.Contains(".gif"))
                //{
                //    thumbnailName = $"{folderMonth}/{folderDay}/{GenerateId.GenerateOrderNumber() + ext}";
                //}

                var f = new FileExtensionContentTypeProvider();
                f.TryGetContentType(fileName, out string contentType);
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    await _minioClient.PutObjectAsync(bucketName, newName, binaryReader.BaseStream, binaryReader.BaseStream.Length, contentType);
                }
                //return new UploadFile
                //{
                //    FilePath = newName,
                //    //Thumbnail = thumbnailName,
                //    BucketName = bucketName,
                //    FileName = fileName,
                //    FileSize = file.Length,
                //    FileType = contentType,
                //    Extension = Path.GetExtension(fileName),
                //    UploadMode = UploadMode.Minio
                //};
            }
            else
            {
                throw new Exception("文件过大");
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFile(string bucketName, string fileName)
        {
            var fileStream = new MemoryStream();

            await _minioClient.GetObjectAsync(bucketName, fileName, s => s.CopyTo(fileStream));
            fileStream.Position = 0;
            return fileStream;
        }
    }
}
