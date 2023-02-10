using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Nailhang.Blazor.Data;
using Nailhang.Display.MD5Cache;
using Nailhang.Display.NetPublicSearch.Base;
using Nailhang.IndexBase.PublicApi;
using Nailhang.IndexBase.Storage;
using Ninject;

namespace Nailhang.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<Data.NailhangModulesService>();
            services.AddScoped<States.NetSearchState>();

            //dirty place

            var rok = new StandardKernel();
            rok.Load<Nailhang.Mongodb.Module>();
            rok.Load<Nailhang.Mongodb.ModuleDefault>();
            rok.Load<Nailhang.Mongodb.ModulesStorage.Module>();
            rok.Load<Nailhang.Mongodb.PublicStorage.Module>();
            rok.Load<Nailhang.Display.Tools.TextSearch.Module>();
            rok.Load<Nailhang.Display.NetPublicSearch.Module>();
            rok.Load<Nailhang.Display.InterfacesSearch.Module>();
            rok.Bind<Nailhang.Display.Controllers.InterfaceController>().ToSelf().InSingletonScope();
            rok.Bind<Nailhang.Display.Controllers.IndexController>().ToSelf().InSingletonScope();

            rok.Bind<IConfiguration>()
                .ToMethod(q => Configuration).InSingletonScope();
            rok.Bind<IMD5Cache>().To<MD5NonBlockingCache>().InSingletonScope();

            services.AddTransient(sp => rok.Get<Nailhang.Display.Controllers.IndexController>());
            services.AddTransient(sp => rok.Get<Nailhang.Display.Controllers.InterfaceController>());

            services.AddTransient<IModulesStorage>(sp => rok.Get<IModulesStorage>());
            services.AddTransient<INetSearch>(sp => rok.Get<INetSearch>());
            services.AddTransient<IPublicApiStorage>(sp => rok.Get<IPublicApiStorage>());
            services.AddTransient<IMD5Cache>(sp => rok.Get<IMD5Cache>());           
            services.AddTransient<Display.InterfacesSearch.Base.IInterfacesSearch>(sp => rok.Get<Display.InterfacesSearch.Base.IInterfacesSearch>());           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            var root = Configuration.GetSection("general")["wwwRoot"];
            app.UsePathBase(new PathString(root));
            app.UseStaticFiles();
            //app.UseHttpsRedirection();

            

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
            //    RequestPath = new PathString(root)
            //});

            

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
