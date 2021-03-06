using Autofac;
using AutoMapper;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DAL.Repositories;
using DAL.Repositories.Implementations;
using Manager4HospitalBackEnd.AzureStorage;

namespace Manager4HospitalBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<IDoctorRepository>(_ => new DoctorRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IAppointmentRepository>(_ => new AppointmentRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IPersonRepository>(_ => new PersonRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IProcedureRepository>(_ => new ProcedureRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IAzureBlobService>(_ => new AzureBlobService(Configuration["AzureBlobStorageConnection"], Configuration["AzureContainerName"]));
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200",
                                            "http://localhost:5001/api/journal");
                    });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            DAL.IoC.Bootstraper.Bootstrap(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
