using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Library.Web.Entities;
using Library.Web.Models;
using Library.Web.Services;
using Library.Web.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace Library.Web
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
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //services.AddAuthentication(opt =>
            //    {
            //        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //        opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //    })
            //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //    {
            //        options.AccessDeniedPath = "/Authorization/AccessDenied";
            //    })
            //    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
            //    {
            //        opt.Authority = "http://localhost:54900/";
            //        opt.RequireHttpsMetadata = false;

            //        opt.ClientId = "library-API";
            //        opt.ClientSecret = "library";
            //        opt.SignedOutRedirectUri = new PathString("/Home/Index");
            //        opt.ResponseType = "code id_token";

            //        opt.Scope.Clear();
            //        opt.Scope.Add("openid");
            //        opt.Scope.Add("profile");
            //        opt.Scope.Add("offline_access");
            //        opt.Scope.Add("email");

            //        opt.GetClaimsFromUserInfoEndpoint = true;
            //        opt.SaveTokens = true;

            //        opt.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            NameClaimType = JwtClaimTypes.Name,
            //            RoleClaimType = JwtClaimTypes.Role
            //        };
            //    });

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["IdentityServerAddress"];
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "libraryAPI";
                    options.ApiSecret = "APIsecret";
                });


            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            })
            .AddFluentValidation();

            services.AddTransient<IValidator<BookForCreationDto>, BookForManipulationDtoValidator>();
            services.AddTransient<IValidator<BookForUpdateDto>, BookForManipulationDtoValidator>();

            services.AddAutoMapper();

            var connectionString = Configuration["connectionStrings:LibraryDBConnectionString"];
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<ILibraryRepository, LibraryRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "LibraryAPI",
                    Version = "1.0",
                    Description = "Just a normal library API.",
                    TermsOfService = "None",
                    Contact = new Contact { Email = "simeon.banev3@gmail.com", Name = "Simeon Banev", Url = "https://github.com/simonbane" },
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Library.Web.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddCors();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Information);
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            libraryContext.Database.Migrate();
            libraryContext.SeedData();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LvMiniAPI 1.0");
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
