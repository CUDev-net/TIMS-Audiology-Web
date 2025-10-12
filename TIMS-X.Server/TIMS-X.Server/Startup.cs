using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AspNetCore.RouteAnalyzer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Filters;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Integrations;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TIMS_X.Server.Examples;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using TIMS_X.BLL;
using TIMS_X.Core;
using TIMS_X.DAL.DAL;

namespace TIMS_X.Server
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
            services.AddRazorPages();
            services.AddControllers();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;

                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<CookieTempDataProviderOptions>(options => {
                options.Cookie.IsEssential = true;
            });

            services.Configure<AppSettings>(Configuration);
            var appSettings = Configuration.Get<AppSettings>();
            var useSwagger = false;
#if DEBUG
            useSwagger = true;
#endif
            if (useSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", _GetApiInfo());

                    c.ExampleFilters();

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = @"Authorization Header (e.g. ""Bearer xd232024"")",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                            Reference = new OpenApiReference
                                {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                    });
                    
                    var filePath = Path.Combine(AppContext.BaseDirectory, "TIMS-X.Server.xml");
                    c.IncludeXmlComments(filePath);
                });
                services.ConfigureSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerOperationFilter>();
                });

                services.AddSwaggerExamplesFromAssemblyOf<FindAppointmentOpeningsExample>();

            }

            services.AddHttpContextAccessor();

            services.AddSignalR();

            services.AddMvc(options =>
            {
                options.Filters.Add(new ChangePasswordFilter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            })
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AllowAnonymousToPage("/Account/SignIn");
                options.Conventions.AllowAnonymousToPage("/Account/SignedOut");
                options.Conventions.AllowAnonymousToPage("/Error");
			})
            .AddSessionStateTempDataProvider()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            services.AddScoped(_ => {
                return new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage"));
            });

            services.AddSingleton<IUserIdProvider, UserIdProvider>();

            services.AddSession();
            services.AddTransient<UserService>();
            services.AddTransient<UserSecurityService>();
            services.AddTransient<UserQuery>();
            services.AddTransient<ProviderQuery>();
            services.AddTransient<ClaimsQuery>();
            services.AddTransient<NoahQuery>();
            services.AddTransient<VendorQuery>();
            services.AddTransient<VendorService>();
            services.AddTransient<ImagingService>();
            services.AddTransient<PatientQuery>();
            services.AddTransient<HaHistoryQuery>();
            services.AddTransient<PracticeQuery>();
            services.AddTransient<CustomerQuery>();
            services.AddTransient<SchedulerQuery>();
            services.AddTransient<HistoryQuery>();
            services.AddTransient<TimsUpdateQuery>();
            services.AddTransient<NoahDataMiningQuery>();
            services.AddTransient<NoahService>();
            services.AddTransient<PatientService>();
            services.AddTransient<OpportunityTrackingService>();
            services.AddTransient<SchedulerService>();
            services.AddTransient<TimsUpdateService>();
            services.AddTransient<TwilioMessenger>();
            services.AddTransient<AwsMessenger>();
            services.AddTransient<MailgunEmailer>();

            services.AddTransient<INoahDataMiningService, NoahDataMiningService>();
            services.AddTransient<IPatientMessagingService, PatientMessagingService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<DbContextResolver>();
            services.AddScoped<ContextHelper>();

            services.AddDbContext<TimsUpdateDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<HaHistoryDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<HistoryDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<ClaimsDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<UserDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<ProviderDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<NoahDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<PracticeDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<PatientDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<SchedulerDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<ImagingDbContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });
            services.AddDbContext<TimsContext>((serviceProvider, options) =>
            {
                serviceProvider.GetService<DbContextResolver>().ResolveConnection(options);
            });

            services.AddTimsXBusinessLogic(Configuration);

            services.AddDbContext<TimsInternalDbContext>(options =>
            {

#if DEBUG
                options.UseSqlServer(appSettings.ConnectionStrings.TIMSInternal);
#elif TEST
                ConnectionStringBuilder.SetConnectionString(options, "", "timsaudiology-test", "", "", true);
#else
                ConnectionStringBuilder.SetConnectionString(options, "", "timsaudiology-prod", "", "");

#endif
            });

            var key = Encoding.ASCII.GetBytes(appSettings.Keys.JwtSecret);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/SignIn";
                    options.LogoutPath = "/Account/SignOut";
                    options.Cookie.Expiration = null;
                    options.Cookie.IsEssential = true;
                });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            if (!string.IsNullOrWhiteSpace(appSettings.LogFile))
            {
                Core.Application.Initialize(logEvent =>
                {
                    if (string.IsNullOrWhiteSpace(appSettings.LogFile))
                        return;

                    using (var streamWriter = File.AppendText(appSettings.LogFile))
                    {
                        streamWriter.WriteLine($"[{DateTime.Now:MM/dd/yyyy HH:mm tt}] {logEvent.RenderMessage()}");
                    }
                });
            }

            services.AddRouteAnalyzer();
            services.ConfigureCors();

            services.AddSpaStaticFiles(
                configuration => configuration.RootPath = "wwwroot");
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
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                        {
                            await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
                        }
                        else
                        {
                            var err = exceptionHandlerPathFeature?.Error;
                            if (err != null)
                            {
                                await context.Response.WriteAsync($"Error Message: {err.Message}<br>{err.InnerException?.ToString()}<br>\r\n");
                            }
                        }

                        await context.Response.WriteAsync(
                                                      "<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512));
                    });
                });
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 401)
                {
                    context.HttpContext.Response.Redirect("/Account/SignIn");
                }
            });
#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TIMS-X Server"); });
#endif
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();
            app.UseCors(ServiceExtensions.CORS_POLICY);

            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseReDoc(c => {
                c.DocumentTitle = "TIMS API Documentation";
                c.SpecUrl = "/swagger/v1/swagger.json";

            });

            app.UseEndpoints(endpoints =>
            {
                var desiredTransports =
                    HttpTransportType.WebSockets |
                    HttpTransportType.LongPolling;


                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<SmsHub>("/smshub", (options) =>
                {
                    options.Transports = desiredTransports;
                });
                endpoints.MapHub<PatientPhotoHub>("/photohub", (options) =>
                {
                    options.Transports = desiredTransports;
                });
                endpoints.MapHub<UserManagementHub>("/usermgmt", (options) =>
                {
                    options.Transports = desiredTransports;
                });
                endpoints.MapHub<SchedulerHub>("/web/schedulerhub", (options) =>
                {
                    options.Transports = desiredTransports;
                });
            });

            app.UseSpa(spa =>
            {
#if DEBUG
                spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
#endif
            });
        }
        private OpenApiInfo _GetApiInfo()
        {
            return new OpenApiInfo
            {
                Title = "TIMS for Audiology Vendor Api",
                Version = "v1.0",
                Description =
    @"
## Getting Started
Welcome to the TIMS for Audiology vendor api documentation.<br />
Version 1.1 of the api provides the following capabilities:<br />
<br />

## Authentication / Authorization

Authentication is handled by requiring an api key in the header of each request.<br/>
If you don't already have an api key, please contact support to request one.<br/>
In addition to the api key, the customer office code (unique customer id) must also be passed in the header of each request.<br/>
Database access is granted on a per-customer database. Please contact support to request access to a customer database.<br/>
The server will return a 403 Forbidden error if you attempt to access a customer database without permission.",

                Contact = new OpenApiContact()
                {
                    Name = "Support",
                    Email = "audiologysupport@cu.net"
                }
            };
        }
    }


    public static class ServiceExtensions
    {
        public static string CORS_POLICY = "CorsPolicy";

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CORS_POLICY, builder =>
                    builder.WithOrigins("https://test.timsaudiology.com/", "https://timsaudiology-test.azurewebsites.net/")
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    );
            });
        }
    }
}
