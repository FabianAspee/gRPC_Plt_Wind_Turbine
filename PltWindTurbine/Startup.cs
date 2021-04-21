using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Services.LoadFilesService;
using PltWindTurbine.Services.MetricCalculusService;
using PltWindTurbine.Services.ObtaininfoTurbinesService;
using PltWindTurbine.Services.ViewFailuresService;
using PltWindTurbine.Subscriber.SubscriberFactory;

namespace PltWindTurbine
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddGrpc(options => { options.MaxReceiveMessageSize = 512 * 1024 * 1024; options.MaxSendMessageSize = 512 * 1024 * 1024; });
            services.AddSingleton<ISubscriberFactory, SubscriberFactory>();

          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<LoadFileService>();
                endpoints.MapGrpcService<MetricCalculuService>();
                endpoints.MapGrpcService<ObtainInfoTurbineService>();
                endpoints.MapGrpcService<ViewFailureService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
