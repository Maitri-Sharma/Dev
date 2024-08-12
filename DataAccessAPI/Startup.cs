using AspNetCoreRateLimit;
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.Hangfire;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Puma.CustomException;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Repository.KspuDB;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;

namespace DataAccessAPI
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfiguration = configuration;
        }

      

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the static configuration.
        /// </summary>
        /// <value>
        /// The static configuration.
        /// </value>
        public static IConfiguration StaticConfiguration { get; private set; }




        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = new List<string>() { "application/json" };
                options.EnableForHttps = true;
                //ResponseCompressionDefaults.MimeTypes.Concat(
                //    new[] { "image/svg+xml" });
            });




            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                    });
            });

            //services.AddResponseCompression();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IAppSettingRepository, AppSettingRepository>();

            var appSettingService = services.BuildServiceProvider().GetService<IAppSettingRepository>();

           
            bool isToAuthenticate = Convert.ToBoolean(Configuration.GetSection("IsAuthenticationApplicable").Value);
            if (isToAuthenticate)
            {
                var key = Encoding.ASCII.GetBytes(appSettingService.GetAppSettingValue(AppSetting.JWTSecret, true).Result);
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                   // options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                        ValidateLifetime = true
                    };
                });
            }
            
            


            string sqlConnectionString = appSettingService.GetConnectionString().Result;

            //Regsiter DB context
            // var sqlConnectionString = Helper.ConfigSettings.GetConnectionString;

            services.AddDbContext<KspuDBContext>(options => options.UseNpgsql(sqlConnectionString));
            //services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataAccessAPI", Version = "v1" });
                if (isToAuthenticate)
                {
                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme."
                    });
                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                 });
                }
            });

            //Call methods to register services
            services.AddAppMVC();

            //Register piple behavior for fluent validatio
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            //Call method to register all Dependencies
            services.RegisterDependencies();

            //Register mapping profile
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue; // or your desired value
            });

            services.AddSingleton(mapper);

            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(sqlConnectionString, new PostgreSqlStorageOptions()
                {
                    SchemaName = "kspu_db_hangfire",
                    DistributedLockTimeout = TimeSpan.FromMinutes(2),
                    PrepareSchemaIfNecessary = true
                });
                config.UseConsole();
            });
            services.AddHangfireServer();
            services.AddMemoryCache();
            //Set IP rate limit for API

            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = false;
                options.StackBlockedRequests = false;
                options.HttpStatusCode = 429;
                options.RealIpHeader = "X-Real-IP";
                options.ClientIdHeader = "X-ClientId";
                options.GeneralRules = new List<RateLimitRule>
                            {
                                new RateLimitRule
                                {
                                    Endpoint = "*",
                                    Period = "1s",
                                    Limit = 100,
                                }
                            };
            });
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string pathBase = string.Empty;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataAccessAPI v1"));
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new MyAuthorizationFilter() },
                    IgnoreAntiforgeryToken = true
                });
            }
            else
            {
                string hangFirePrefix = Configuration.GetValue<string>("HangFirePrefix");

                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new MyAuthorizationFilter() },
                    PrefixPath = !string.IsNullOrWhiteSpace(hangFirePrefix) ? hangFirePrefix : null,
                    IgnoreAntiforgeryToken = true
                });
            }

            app.UseHttpsRedirection();
            //Compression middleware before other middlewares which serves the files.

            app.UseResponseCompression();

            //for IP rate limit on API
            app.UseIpRateLimiting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.EnableFilter();
                c.EnableValidator();
                c.EnableDeepLinking();
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.List);
                c.DefaultModelRendering(ModelRendering.Example);
                c.SwaggerEndpoint("./v1/swagger.json", "DataAccessAPI v1");
            });
            app.UseStaticFiles();
            // Add external authentication middleware below.  


            //app.UseHangfireServer();
            HangfireJobScheduler.ScheduleJob();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseCors();

            bool isToAuthenticate = Convert.ToBoolean(Configuration.GetSection("IsAuthenticationApplicable").Value);
            if (isToAuthenticate)
            {
                app.UseMiddleware<JwtMiddleware>();
            }
            app.UseRouting();

          

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            /// <summary>
            /// Authorizes the specified context.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns></returns>
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                //When we implement authentication uncomment below line..refer https://docs.hangfire.io/en/latest/configuration/using-dashboard.html#configuring-authorization
                return true; //httpContext.User.Identity.IsAuthenticated;
            }
        }
    }
}
