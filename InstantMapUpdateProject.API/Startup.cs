using InstantMapUpdateProject.API.Hubs;
using InstantMapUpdateProject.API.Models;
using InstantMapUpdateProject.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstantMapUpdateProject.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:44340").AllowAnyHeader().AllowAnyMethod().AllowCredentials();//UI url
                    builder.WithOrigins("https://localhost:5001").AllowAnyHeader().AllowAnyMethod().AllowCredentials();//UI url
                });
            });
            services.AddDbContext<UpdateMapContext>(optionsBuldier => optionsBuldier.UseSqlServer(Configuration.GetConnectionString("ConStr")));
           // services.AddDbContextPool<UpdateMapContext>(options => options.UseSqlServer(Configuration.GetSection("ConnectionString")["ConStr"]));

            services.AddSignalR();
            services.AddScoped<PharmacyService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InstantMapUpdateProject.API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InstantMapUpdateProject.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PharmacyHub>("/PharmacyHub");
            });
        }
    }
}
