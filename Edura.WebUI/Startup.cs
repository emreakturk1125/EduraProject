using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Edura.WebUI.Repository.Concrete.EntityFramework;
using Edura.WebUI.Repository.Abstract;
using Edura.WebUI.Repository.Concrete.Adonet;
using Edura.WebUI.IdentityCore;
using Microsoft.AspNetCore.Identity;

namespace Edura.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
      
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EduraContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();

            services.AddTransient<IProductRepository, EfProductRepository>();
            services.AddTransient<ICategoryRepository, EfCategoryRepository>();
            services.AddTransient<IUnitOfWork, EfUnitOfWork>();
            services.AddMvc();

            services.AddMemoryCache();  //Session kullanımı
            services.AddSession();
        }
 
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseSession(); //session kullanımı
            app.UseAuthentication();
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                       name: "products",
                       template: "products/{category?}",
                       defaults: new { controller = "Product", action = "List" });

                routes.MapRoute(
                       name: "default",
                       template: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.EnsurePopulated(app);
            SeedIdentity.CreateIdentityUsers(app.ApplicationServices, Configuration).Wait();
        }

    }
}
