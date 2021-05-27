using Blazored.Toast;
using ClientPltTurbine.Data;
using ClientPltTurbine.Pages.Component.ChartComponent; 
using ClientPltTurbine.Pages.Component.LoadFileComponent;
using ClientPltTurbine.Pages.Component.ModelPredictionComponent; 
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO; 
using System.Threading.Tasks;

namespace ClientPltTurbine
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
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredToast();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<LoadFile>();
            services.AddSingleton<ModelPrediction>();  
            services.AddSingleton<ChartSingleton>();  
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

            app.UseHttpsRedirection();
            app.UseStaticFiles(); 
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images")),
                RequestPath = "/wwwroot/images"
            });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            { 
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            Task.Run(async () => await Electron.WindowManager.CreateBrowserViewAsync());
            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }
    }
}
