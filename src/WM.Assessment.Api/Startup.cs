using System;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using WM.Assessment.Application.Behaviors;
using WM.Assessment.Application.EventHandling;
using WM.Assessment.Application.ExpirableGuids.CreateExpirableGuid;
using WM.Assessment.Domain.ExpirableGuids;
using WM.Assessment.Infrastructure.SqlDataAccess;

namespace WM.Assessment.Api
{
    public class Startup
    {
        private string _contentRoot;
        private readonly string _wmaConnection;

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            _contentRoot = env.ContentRootPath;

            //_wmaConnection = Environment.GetEnvironmentVariable("WMA_CONNECTION");
            _wmaConnection = Configuration["ConnectionStrings:WMA_CONNECTION"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();

            #region Repository injection

            services.AddTransient<IEventDispatcher, EventDispatcher>();
            services.AddTransient<IEventRepository, EventRepository>(ctx => new EventRepository(_wmaConnection));

            services.AddTransient<IExpirableGuidRepository, ExpirableGuidRepository>(ctx =>
            {
                var dispatcher = ctx.GetService<IEventDispatcher>();
                return new ExpirableGuidRepository(_wmaConnection, dispatcher);
            });

            #endregion

            #region Mediatr

            services.AddMediatR(typeof(CreateExpirableGuidHandler).GetTypeInfo().Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

            #endregion

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());

                options.DefaultPolicyName = "CorsPolicy";
            });

            //RESPONSE JSON FORMATTING
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver();
                    }
                );

            #region swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WM Assessment API", Version = "v1"});
                var filePath = Path.Combine(AppContext.BaseDirectory, "WM.Assessment.Api.xml");
                c.IncludeXmlComments(filePath);
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "WM Assessment API V1"); }
            );
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();

            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}