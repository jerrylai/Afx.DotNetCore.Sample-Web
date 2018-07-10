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


namespace AfxDotNetCoreSample.Controllers
{
    public class LogPageParam: PageDataInputDto
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class FileDto
    {
        public string Name { get; set; }

        public string Size { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }

    public class LogController : BaseController
    {
        [HttpGet, HttpPost]
        public ActionResult Index()
        {
            string url = $"~/log.html{this.Request.QueryString}";

            return LocalRedirect(url);
        }

        private bool CheckKey()
        {
            bool result = false;
            //string key = this.Request.Headers["key"];

            //if (string.IsNullOrEmpty(key))
            //{
            //    key = this.Request.Query["key"];
            //}

            //if (!string.IsNullOrEmpty(key))
            //{
            //    var service = this.GetService<IConfigService>();
            //    var k = service.Get(SysConfigType.LogKey.GetValue());
            //    if (string.IsNullOrEmpty(k))
            //    {
            //        k = "admin";
            //        service.Set(SysConfigType.LogKey.GetValue(), k);
            //    }

            //    if (key == k)
            //    {
            //        result = true;
            //    }
            //}
            result = true;


            return result;
        }

        [HttpGet, HttpPost]
        public ActionResult Check()
        {
            if(this.CheckKey())
            {
                return Success(true);
            }

            return Failure("key 不正确！");
        }
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;
        private string GetSize(long v)
        {
            string s = "";
            if(v > GB)
            {
                var vv = v * 1.0m / GB;
                s = vv.ToString("n2") + "GB";
            }
            else if(v > MB)
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

        [HttpGet, HttpPost]
        public ActionResult GetPageList(LogPageParam vm)
        {
            if (this.CheckKey())
            {
                if (vm != null && vm.PageIndex > 0 && vm.PageSize > 0)
                {
                    PageDataOutputDto<FileDto> pageData = new PageDataOutputDto<FileDto>();
                    var path = LogUtils.GetLogDir();
                    if (System.IO.Directory.Exists(path))
                    {
                        var dir = new System.IO.DirectoryInfo(path);
                        var files = dir.EnumerateFiles();
                        if(vm.BeginTime.HasValue)
                        {
                            files = files.Where(q => q.CreationTime >= vm.BeginTime.Value);
                        }
                        if (vm.EndTime.HasValue)
                        {
                            files = files.Where(q => q.CreationTime <= vm.EndTime.Value);
                        }
                        pageData.TotalCount = files.Count();
                        files = files.OrderByDescending(q => q.CreationTime);
                        if(vm.PageIndex > 1)
                        {
                            files = files.Skip(vm.PageSize * (vm.PageIndex - 1));
                        }
                        pageData.Data = files.Take(vm.PageSize).Select(q => new FileDto
                        {
                            Name = q.Name,
                            Size = GetSize(q.Length),
                            CreateTime = q.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            UpdateTime = q.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                        }).ToList();
                    }

                    return Success(pageData);
                }
                return Failure("请求参数不正确！");
            }

            return Failure("key 不正确！");
        }

        [HttpGet, HttpPost]
        public ActionResult Open()
        {
            if (this.CheckKey())
            {
                var file = this.Request.Query["file"];
                if(!string.IsNullOrEmpty(file))
                {
                    var path = System.IO.Path.Combine(LogUtils.GetLogDir(), file);
                    if(System.IO.File.Exists(path))
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
                return Ok("文件未找到！");
            }

            return Ok("key 不正确！");
        }

        [HttpGet, HttpPost]
        public ActionResult Del(FileDto vm)
        {
            if (this.CheckKey())
            {
                if (vm != null && !string.IsNullOrEmpty(vm.Name))
                {
                    var path = System.IO.Path.Combine(LogUtils.GetLogDir(), vm.Name);
                    if (System.IO.File.Exists(path))
                    {
                        try { System.IO.File.Delete(path); }
                        catch(Exception ex)
                        {
                            return Failure(ex.Message);
                        }

                        return Success(true);
                    }
                }
                return Failure("文件未找到！");
            }

            return Failure("key 不正确！");
        }

        [HttpGet, HttpPost]
        public ActionResult DelAll(LogPageParam vm)
        {
            if (this.CheckKey())
            {
                var path = LogUtils.GetLogDir();
                if (System.IO.Directory.Exists(path))
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
                    foreach(var f in files)
                    {
                        try { f.Delete(); }
                        catch { }
                    }
                }
                
                return Success(true);
            }

            return Failure("key 不正确！");
        }

        [HttpGet, HttpPost]
        public ActionResult GetLevel()
        {
            if (this.CheckKey())
            {
                return Success(LogUtils.GetLevel());
            }

            return Failure("key 不正确！");
        }

        const string LEVEL = "ALL|DEBUG|INFO|WARN|ERROR|FATAL|OFF";
        [HttpGet, HttpPost]
        public ActionResult SetLevel()
        {
            if (this.CheckKey())
            {
                string level = this.Request.Query["Level"];
                if(string.IsNullOrEmpty(level))
                {
                    level = this.Request.Form["level"];
                }

                level = level?.ToUpper();
                if (!string.IsNullOrEmpty(level) && LEVEL.Split('|').Contains(level))
                {
                    if(LogUtils.SetLevel(level))
                        return Success(true);
                    else
                        return Failure("设置失败！");
                }
                else
                {
                    return Failure("level 不正确！");
                }
            }

            return Failure("key 不正确！");
        }
    }
}
