﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Controllers
{
    /// <summary>
    /// 上传文件
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration _configuration;

        public UploadController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        [HttpPut]
        public ApiResult UploadFile(List<IFormFile> files)
        {
            ApiResult result = new ApiResult();
            //注：参数files对象去也可以通过换成： var files = Request.Form.Files;来获取
            if (files.Count <= 0)
            {
                result.ErrorMessage = "上传文件不能为空";
                return result;
            }

            #region 上传          

            List<string> filenames = new List<string>();

            var webRootPath = _env.WebRootPath;
            var rootFolder = _configuration.Get<AppSettings>().UploadPath;

            var physicalPath = $"{webRootPath}/{rootFolder}/";

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName);//获取文件格式，拓展名               

                var saveName = $"{rootFolder}/{Path.GetRandomFileName()}{fileExtension}";
                filenames.Add(saveName);//相对路径

                var fileName = webRootPath + saveName;

                using FileStream fs = System.IO.File.Create(fileName);
                file.CopyTo(fs);
                fs.Flush();

            }
            #endregion


            result.Success = true;
            result.Data = filenames;

            return result;
        }
    }
}
