using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

using AfxDotNetCoreSample.Dto;
using AfxDotNetCoreSample.IService;
using AfxDotNetCoreSample.Common;
using AfxDotNetCoreSample.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AfxDotNetCoreSample.Web.Controllers
{
    [UserAuth("1002002000000")]
    public class LogController : BaseController
    {
        private const string LEVEL = "ALL|DEBUG|INFO|WARN|ERROR|FATAL|OFF";
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;
        private string GetSize(long v)
        {
            string s = "";
            if (v > GB)
            {
                var vv = v * 1.0m / GB;
                s = vv.ToString("n2") + "GB";
            }
            else if (v > MB)
            {
                var vv = v * 1.0m / MB;
                s = vv.ToString("n2") + "MB";
            }
            else if (v > KB)
            {
                var vv = v * 1.0m / KB;
                s = vv.ToString("n2") + "KB";
            }
            else
            {
                s = v.ToString("n0") + "B";
            }

            return s;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetPageList(LogPageInputDto vm)
        {
            if (vm != null && this.ModelState.IsValid)
            {
                PageDataOutputDto<LogFileOutputDto> pageData = new PageDataOutputDto<LogFileOutputDto>();
                var path = LogUtils.GetLogDir(vm.Name);
                if (!string.IsNullOrEmpty(path) && System.IO.Directory.Exists(path))
                {
                    var dir = new System.IO.DirectoryInfo(path);
                    var files = dir.EnumerateFiles();
                    if (vm.BeginTime.HasValue)
                    {
                        files = files.Where(q => q.CreationTime >= vm.BeginTime.Value);
                    }
                    if (vm.EndTime.HasValue)
                    {
                        files = files.Where(q => q.CreationTime <= vm.EndTime.Value);
                    }
                    pageData.TotalCount = files.Count();
                    files = files.OrderByDescending(q => q.CreationTime);
                    if (vm.PageIndex > 1)
                    {
                        files = files.Skip(vm.PageSize * (vm.PageIndex - 1));
                    }
                    pageData.Data = files.Take(vm.PageSize).Select(q => new LogFileOutputDto
                    {
                        Name = q.Name,
                        Size = GetSize(q.Length),
                        CreateTime = q.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = q.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList();
                }

                return Success(pageData);
            }

            return Error("请求参数不正确！");
        }

        [HttpGet]
        public ActionResult Open()
        {
            var name = this.Request.Query["name"].FirstOrDefault();
            var file = this.Request.Query["file"].FirstOrDefault();
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(file))
            {
                var dir = LogUtils.GetLogDir(name);
                if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
                {
                    var path = System.IO.Path.Combine(dir, file);
                    if (System.IO.File.Exists(path))
                    {
                        string s = "";
                        using (var fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                        {
                            using (var rd = new System.IO.StreamReader(fs, Encoding.UTF8))
                            {
                                s = rd.ReadToEnd();
                            }
                        }
                        return Content(s, "text/plain", Encoding.UTF8);
                    }
                }
            }

            return Ok("文件未找到！");
        }

        [HttpPost]
        public ActionResult Del()
        {
            string name = this.Request.Form["Name"].FirstOrDefault();
            var file = this.Request.Form["Files"].FirstOrDefault();
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(file))
            {
                var dir = LogUtils.GetLogDir(name);
                if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
                {
                    var arr = file.Split(',');
                    foreach (var s in arr)
                    {
                        var path = System.IO.Path.Combine(dir, s);
                        if (System.IO.File.Exists(path))
                        {
                            try { System.IO.File.Delete(path); }
                            catch (Exception ex)
                            {
                                return Error(ex.Message);
                            }
                        }
                    }

                    return Success(true);
                }
            }

            return Error("文件未找到！");
        }
        
        [HttpGet]
        public ActionResult GetLevel(string name)
        {
            return Success(LogUtils.GetLevel(name));
        }

        [HttpPost]
        public ActionResult SetLevel()
        {
            string level = this.Request.Form["Level"].FirstOrDefault();
            string name = this.Request.Form["Name"].FirstOrDefault();

            level = level?.ToUpper();
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(level) && LEVEL.Split('|').Contains(level))
            {
                if (LogUtils.SetLevel(name, level))
                    return Success(true);
                else
                    return Error("设置失败！");
            }
            else
            {
                return Error("level 不正确！");
            }
        }
    }
}
