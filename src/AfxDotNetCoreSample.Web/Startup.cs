using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AfxDotNetCoreSample.Common;
using System.Text.Unicode;
using System.Text.Encodings.Web;

namespace AfxDotNetCoreSample.Web
{
    public class Startup
    {
        private readonly TimeSpan? sidExpire;
        private readonly double minRefExpire;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IocConfig.Register(configuration);
            sidExpire = CacheKeyUtils.GetExpire("SessionDb", "UserSession");
            minRefExpire = sidExpire.HasValue ? sidExpire.Value.TotalMinutes / 2 : 0d;
            if (minRefExpire > 10d) minRefExpire = 10d;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MvcJsonOptions>(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });

            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = AfxDotNetCoreSample.Common.ConfigUtils.MultipartBodyLengthLimit;
            });

            services.AddMvc(option =>
            {
                option.Filters.Add<ApiExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSid(option =>
            {
                option.IsCookie = true;
                option.Name = SessionUtils.SidName;
                option.EncryptCallback = (val) => EncryptUtils.Encrypt(val);
                option.DecryptCallback = (val) => EncryptUtils.Decrypt(val);
                option.BeginRequestCallback = this.OnRequest;
                option.EndRequestCallback = this.OnResponse;
            });

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "areas",
                //    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            //生成数据库
            IocUtils.Get<IService.ISystemService>().Init();

            LogDelete.Start();
        }

        private void OnRequest(HttpContext context, string sid)
        {
            context.Items["BeginRequestTime"] = DateTime.Now;
        }

        private string OnResponse(HttpContext context, string sid)
        {
            string newsid = sid;
            if (sidExpire.HasValue)
            {
                var arr = sid.Split('-');
                long ticks = 0;
                if (arr.Length >= 2)
                {
                    long.TryParse(arr[1], out ticks);
                }
                var now = DateTime.Now;
                var expire = now;
                if (ticks > now.Ticks) expire = new DateTime(ticks);
                var ets = expire - now;
                if (ets.TotalMinutes < minRefExpire)
                {
                    var sessionService = IocUtils.Get<IService.IUserSessionService>();
                    sessionService.Expire(sid);
                    ticks = now.Add(sidExpire.Value).Ticks;
                    newsid = $"{arr[0]}-{ticks}";
                }
            }

            //请求日志
            DateTime startTime = (DateTime)context.Items["BeginRequestTime"];
            DateTime endTime = DateTime.Now;
            var ts = endTime - startTime;
            string url = context.Request.Path;
            string method = context.Request.Method;
            Microsoft.Extensions.Primitives.StringValues stringValues;
            context.Request.Headers.TryGetValue("User-Agent", out stringValues);
            var msg = $"【Api】method: {method}, url: {url}, TotalMilliseconds: {ts.TotalMilliseconds}, scheme: {context.Request.Scheme}, host: {context.Request.Host }, sid: {sid}, newsid: {newsid} UserAgent:{stringValues.FirstOrDefault()}";
            LogUtils.Debug(msg, WebLogger.LOG_NAME);

            return newsid;
        }
    }

}
