using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Business.MinIO
{
    public interface IFileStore
    {
        Task UploadFile(IFormFile file);

        Task<Stream> DownloadFile(string bucketName, string fileName);
    }
}
