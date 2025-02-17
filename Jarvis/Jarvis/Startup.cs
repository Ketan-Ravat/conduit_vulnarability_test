using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Repo.Concrete;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Jarvis.Repo;
using AutoMapper;
using Jarvis.Service.MappingProfile;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Microsoft.Extensions.Hosting;
using Jarvis.Service.SchedulerServices;
using Jarvis.ViewModels.Filter;
using Jarvis.ViewModels.Utility;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Jarvis.db.MongoDB;

namespace Jarvis
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
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ServiceLayerProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHttpClient();

            services.AddResponseCompression();

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true; // false by default
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Conduit API",
                    Version = "v1"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                string SharedBaseDirectory = Directory.GetCurrentDirectory();
                var xmlFile2 = "Jarvis.ViewModels.xml";
                var xmlPath2 = Path.Combine(SharedBaseDirectory, xmlFile2);

                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(xmlPath2);
            });

            services.AddDbContext<DBContextFactory>(opt =>
            //opt.UseNpgsql("User ID = postgres;Password=icreate;Server=15.206.187.80;Port=5432;Database=jarvis;Integrated Security=true;Pooling=true;"));
            opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            
          //  ConventionRegistry.Register("Camel Case", new ConventionPack { new CamelCaseElementNameConvention() }, _ => true);
            services.AddSingleton<IMongoClient>(s => new MongoClient("mongodb://form-portal:2kxykNM6O4JWa5dXBfIs8esGpRWKbRdxTU2R22xuJHF1vImQIlk2VvhzYJV5ndmO9r7DOQrCajmjGkNm2oYnHQ==@form-portal.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@form-portal@"));
            services.AddScoped(s => new MongoDBContext(s.GetRequiredService<IMongoClient>(), Configuration["DbName"]));

            services.AddScoped(typeof(IBaseGenericRepository<>), typeof(BaseGenericRepository<>));
            services.AddScoped(typeof(IFormAttributesRepository<>), typeof(FormAttributesRepository<>));
            services.AddScoped(typeof(IInspectionFormRepository), typeof(InspectionFormRepository));
            services.AddScoped(typeof(IIssueRepository), typeof(IssueRepository));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(IDeviceRepository), typeof(DeviceRepository));
            services.AddScoped(typeof(IPMCategoryRepository), typeof(PMCategoryRepository));
            services.AddScoped(typeof(IPMPlansRepository), typeof(PMPlansRepository));
            services.AddScoped(typeof(ITaskRepository), typeof(TaskRepository));
            services.AddScoped(typeof(IPMRepository), typeof(PMRepository));
            services.AddScoped(typeof(IAssetPMPlansRepository), typeof(AssetPMPlansRepository));
            services.AddScoped(typeof(IAssetPMsRepository), typeof(AssetPMsRepository));
            services.AddScoped(typeof(IPMTriggersRemarksRepository), typeof(PMTriggersRemarksRepository));
            services.AddScoped(typeof(ICompletedPMTriggerRepository), typeof(CompletedPMTriggerRepository));
            services.AddScoped(typeof(IPMNotificationRepository), typeof(PMNotificationRepository));
            services.AddScoped(typeof(IMaintenanceRequestRepository), typeof(MaintenanceRequestRepository));
            services.AddScoped(typeof(IWorkOrderRepository), typeof(WorkOrderRepository));
            services.AddScoped(typeof(IFormIORepository), typeof(FormIORepository));
            services.AddScoped(typeof(IAssetFormIORepository), typeof(AssetFormIORepository));
            services.AddScoped(typeof(IDashboardRepository), typeof(DashboardRepository));
            
            services.AddScoped<ValidationFilterAttribute>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IFormAttributesService, FormAttributesService>();
            services.AddTransient<IInspectionFormService, InspectionFormService>();
            services.AddTransient<IIssueService, IssueService>();
            services.AddTransient<IAssetService, AssetService>();
            services.AddTransient<IInspectionService, InspectionService>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IPMCategoryService, PMCategoryService>();
            services.AddTransient<IPMPlansService, PMPlansService>();
            services.AddTransient<IAssetPMService, AssetPMService>();
            services.AddTransient<ITaskService, TaskService>();
            services.AddTransient<IPMService, PMService>();
            services.AddTransient<IPMNotificationService, PMNotificationService>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IJarvisUOW, JarvisUOW>();
            services.AddTransient<IJwtHandler, JwtHandler>();
            services.AddTransient<ServiceLayerProfile>();
            services.AddTransient<IMaintenanceRequestService, MaintenanceRequestService>();
            services.AddTransient<IWorkOrderService, WorkOrderService>();
            services.AddTransient<IMobileWorkOrderService, MobileWorkorderService>();
            services.AddTransient<IFormIOService, FormIOService>();
            services.AddTransient<IAssetFormIOService, AssetFormIOService>();
            services.AddTransient<IMongoDBFormIOServices, MongoDBFormIOServices>();
            services.AddTransient<IFormIOAssetClassService, FormIOAssetClassService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddHttpContextAccessor();
            //services.AddTransient<IMongoDBFormIORepository>();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
                options.Level = System.IO.Compression.CompressionLevel.Fastest);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    // General
                    "text/plain",
                    // Static files
                    "text/css",
                    "application/javascript",
                    // MVC
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",
                });
                options.Providers.Add<GzipCompressionProvider>();
            });
           
            // get app setting configuration


            services.AddOptions();

            var section = Configuration.GetSection("AWS_Config");
            services.Configure<AWSConfigurations>(section);
            var JWTsection = Configuration.GetSection("ExternalClientServer");
            services.Configure<JWTExternalClientsKeyDetails>(JWTsection);
            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            //services.TryAddAWSService<IAmazonS3>();

            //services.AddHostedService<PendingInspectionEmailService>();
            //services.AddHostedService<RemoveOlderSyncRecordService>();
            //services.AddHostedService<AddUserRolesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //StringValues platform;
            //app.Use(async (context, next) =>
            //{
            //    context.Request.Headers.TryGetValue("platform", out platform);

            //    //context.Request.Headers.Add("platform", platform);

            //    //context.Request.Headers(() =>
            //    //{
            //    //    context.Response.Headers.Add("MyHeader", "GotItWorking!!!");
            //    //    return Task.FromResult(0);
            //    //});
            //    //await next();
            //});
            app.Map("/checkStatus", config => config.Run(async context =>
            {
                // Log request parameters
                string queryStr = context.Request.QueryString.ToString();
                try
                {
                    // Log Hello World"
                    await context.Response.WriteAsync("Hello World");
                }
                catch (Exception exception)
                {
                    context.Response.StatusCode = 500;
                    // Log exception error
                    await context.Response.WriteAsync("Internal Server Error");
                }
            }));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conduit API V1");
            });


            app.UseCors("AllowOrigin");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
            //    RequestPath = new PathString("/Uploads")
            //});

            app.UseResponseCompression();

            app.UseMvc();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute("default", "{controller=Values}/{action=Index}/{id?}");
            //});

            // Get Prefer Language while loading and save in singleton
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                DBContextFactory DbContext = serviceScope.ServiceProvider.GetService<DBContextFactory>();
                List<PreferLanguageMaster> PreferLanguageMaster = DbContext.PreferLanguageMaster.ToList();
                PreferLanguageSingleton.Instance.PreferLanguageMaster = PreferLanguageMaster;

                List<InspectionAttributeCategory> CategoryMaster = DbContext.InspectionAttributeCategory.ToList();
                InspectionFormCategorySingleton.Instance.InspectionFormCategoryMaster = CategoryMaster;


            }
        }
    }
}
