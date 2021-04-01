using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.SqlServer;
using System;
using StartWithHangFire.Data.Context;
using Microsoft.EntityFrameworkCore;
using StartWithHangFire.Data.Repository.Interfaces;
using StartWithHangFire.Data.Repository;
using StartWithHangFire.Service;
using StartWithHangFire.Configuration;

namespace StartWithHangFire
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IHostEnvironment hostEnvironment)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(hostEnvironment.ContentRootPath)
               .AddJsonFile("appsettings.json", true, true)
               .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
               .AddEnvironmentVariables();

            if (hostEnvironment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //Dependency Injection
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IJobToProcess, JobToProcess>();

            //Config Banco De dados
            services.AddDbContext<ApplicationDbContext>(Options =>
             Options.UseSqlServer(Configuration.GetConnectionString(name: "DefaultConnection")));

            #region HangFire Configuration
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer(opt =>
            {
                opt.ServerName = "MainServer";
                opt.WorkerCount = 4;
                opt.Queues = new[] { "mainfila1", "mainfila2", "default" };
            });

            // Add framework services.
            services.AddMvc();
            #endregion

            services.AddSwaggerConfiguration();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwaggerConfiguration();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region HangFire 
            //app.UseHangfireServer(new BackgroundJobServerOptions
            //{
            //    ServerName = String.Format("Jack_Server"),
            //    WorkerCount = 4,
            //    Queues = new[] { "fila1", "fila2" }
            //});

            app.UseHangfireDashboard();
            //backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            #endregion


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //HangFire
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
