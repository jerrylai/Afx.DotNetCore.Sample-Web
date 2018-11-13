using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.Enums;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AfxDotNetCoreSample.Web.Controllers
{
    [UserAuth]
    public class TempFileController : BaseController
    {
        [HttpGet]
        public IActionResult Get(string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                var temppath = System.IO.Path.Combine(ConfigUtils.TempDirectory, file);
                if (System.IO.File.Exists(temppath))
                {
                    byte[] buffer = null;
                    using (var fs = System.IO.File.Open(temppath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                    }

                    return File(buffer, "application/octet-stream");
                }
            }

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public IActionResult Upload()
        {
            var files = this.Request.Form.Files;
            if (files.Count > 0)
            {
                var list = new List<string>(files.Count);
                var dir = ConfigUtils.TempDirectory;
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                foreach (var f in files)
                {
                    var ext = (System.IO.Path.GetExtension(f.FileName) ?? "").ToLower();
                    string name = Guid.NewGuid().ToString("n") + ext;
                    string savepath = System.IO.Path.Combine(dir, name);
                    using (var rs = f.OpenReadStream())
                    {
                        using (var ws = System.IO.File.Create(savepath))
                        {
                            rs.CopyTo(ws);
                            ws.Flush();
                        }
                    }
                    list.Add(name);
                }

                return Success(list);
            }

            return Error();
        }

    }
}