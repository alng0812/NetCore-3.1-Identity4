using Microsoft.AspNetCore.Http;
using NetCoreWebAPI.Business.MinIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Business
{
    public class FileApp : BaseInstance<FileApp>
    {
        private IFileStore _fileStore;

        public FileApp(IFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        public async Task Add(IFormFileCollection files)
        {
            foreach (var file in files)
            {
                await Add(file);
            }
        }

        public async Task Add(IFormFile file)
        {
            await _fileStore.UploadFile(file);
            //uploadResult.CreateUserName = _auth.GetCurrentUser().User.Name;
            //uploadResult.CreateUserId = Guid.Parse(_auth.GetCurrentUser().User.Id);
            //var a = await Repository.AddAsync(uploadResult);
            //return uploadResult.MapTo<UploadFileResp>();
        }
    }
}
