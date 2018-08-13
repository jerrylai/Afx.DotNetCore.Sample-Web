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
using Afx.Ioc;

namespace AfxDotNetCoreSample.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IocUtils.RegisterSingle<IConfiguration>(configuration);
            IocRegister.Register();
            ConfigUtils.SetThreads();
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


            services.AddMvc(option=> 
            {
                option.Filters.Add<ApiExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSid(option =>
            {
                option.RequestSidCallback = SessionUtils.OnRequestSid;
                option.ResponseSidCallback = SessionUtils.OnResponseSid;
                SessionUtils.ResponseSidCall = () => IocUtils.Get<IService.IUserSessionService>().Expire();
            });

            app.Use((fun) => new LogMiddleware(fun).Invoke);

            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //生成数据库
            IocUtils.Get<IService.ISystemService>().Init();
        }
    }

    class LogMiddleware
    {
        private RequestDelegate next;
        public LogMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            DateTime startTime = DateTime.Now;
            await this.next(context);
            DateTime endTime = DateTime.Now;
            string url = context.Request.Path;
            string method = context.Request.Method;
            var ts = endTime - startTime;
            LogUtils.Debug($"【Api】method: {method}, url: {url}, TotalMilliseconds: {ts.TotalMilliseconds}");
        }
    }
}
