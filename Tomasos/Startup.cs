using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tomasos.Models;

namespace Tomasos
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddSession();
            services.AddDistributedMemoryCache();

            var conn = @"Server=localhost;Database=Tomasos;Trusted_Connection=True; ConnectRetryCount=0";
            services.AddDbContext<TomasosContext>(options => options.UseSqlServer(conn));

            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseSession();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Default",
                    "{controller=Home}/{action=Index}"
                );
            });
        }
    }
}