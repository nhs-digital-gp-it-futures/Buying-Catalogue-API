using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Authentications;
using NHSD.GPITF.BuyingCatalog.OperationFilters;
using NLog;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace NHSD.GPITF.BuyingCatalog
{
  internal sealed class Startup
  {
    private IServiceProvider ServiceProvider { get; set; }
    private IConfigurationRoot Configuration { get; }
    private IHostingEnvironment CurrentEnvironment { get; }
    private IContainer ApplicationContainer { get; set; }

    public Startup(IHostingEnvironment env)
    {
      // Environment variable:
      //    ASPNETCORE_ENVIRONMENT == Development
      CurrentEnvironment = env;

      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddJsonFile($"autofac.json", optional: true)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>();

      Configuration = builder.Build();

      // database connection string for nLog
      GlobalDiagnosticsContext.Set("LOG_CONNECTIONSTRING", Settings.LOG_CONNECTIONSTRING(Configuration));

      DumpSettings();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      // Add controllers as services so they'll be resolved.
      services
        .AddMvc()
        .AddControllersAsServices();

      services.Configure<FormOptions>(x =>
      {
        x.ValueLengthLimit = int.MaxValue;
        x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
      });

      if (CurrentEnvironment.IsDevelopment())
      {
        // Register the Swagger generator, defining one or more Swagger documents
        services.AddSwaggerGen(options =>
        {
          options.SwaggerDoc("v1",
            new Info
            {
              Title = "catalogue-api",
              Version = "1.0.0-private-beta",
              Description = "NHS Digital GP IT Futures Buying Catalog API"
            });
          options.SwaggerDoc("porcelain",
            new Info
            {
              Title = "catalogue-api",
              Version = "porcelain",
              Description = "NHS Digital GP IT Futures Buying Catalog API"
            });

          options.DocInclusionPredicate((docName, apiDesc) =>
          {
            var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
              return false;
            }

            var versions = controllerActionDescriptor.MethodInfo.DeclaringType
              .GetCustomAttributes(true)
              .OfType<ApiVersionAttribute>()
              .SelectMany(attr => attr.Versions);
            var tags = controllerActionDescriptor.MethodInfo.DeclaringType
              .GetCustomAttributes(true)
              .OfType<ApiTagAttribute>();

            return versions.Any(v =>
              $"v{v.ToString()}" == docName) ||
              tags.Any(tag => tag.Tag == docName);
          });

          options.AddSecurityDefinition("oauth2", new OAuth2Scheme
          {
            Type = "oauth2",
            Flow = "accessCode"
          });
          options.AddSecurityDefinition("basic", new BasicAuthScheme());

          options.OperationFilter<AssignSecurityRequirements>();

          // Set the comments path for the Swagger JSON and UI.
          var xmlPath = Path.Combine(AppContext.BaseDirectory, "NHSD.GPITF.BuyingCatalog.xml");
          options.IncludeXmlComments(xmlPath);
          options.DescribeAllEnumsAsStrings();
          options.OperationFilter<ExamplesOperationFilter>();
        });
      }

      services
        .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
        .AddBasicAuthentication(
          options =>
          {
            options.Realm = "NHSD.GPITF.BuyingCatalog";
            options.Events = new BasicAuthenticationEvents
            {
              OnValidatePrincipal = context =>
              {
                var auth = ServiceProvider.GetService<IBasicAuthentication>();
                return auth.Authenticate(context);
              }
            };
          });

      services
        .AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          options.Authority = Settings.OIDC_ISSUER_URL(Configuration);
          options.Audience = Settings.OIDC_AUDIENCE(Configuration);
          options.RequireHttpsMetadata = !CurrentEnvironment.IsDevelopment();
          options.Events = new JwtBearerEvents
          {
            OnTokenValidated = async context =>
            {
              var auth = ServiceProvider.GetService<IBearerAuthentication>();
              await auth.Authenticate(context);
            }
          };
        });

      // Create the container builder.
      var builder = new ContainerBuilder();

      // Register dependencies, populate the services from
      // the collection, and build the container.
      //
      // Note that Populate is basically a foreach to add things
      // into Autofac that are in the collection. If you register
      // things in Autofac BEFORE Populate then the stuff in the
      // ServiceCollection can override those things; if you register
      // AFTER Populate those registrations can override things
      // in the ServiceCollection. Mix and match as needed.
      builder.Populate(services);

      // load all assemblies in same directory and register classes with interfaces
      // Note that we have to explicitly add this (executing) assembly
      var exeAssy = Assembly.GetExecutingAssembly();
      var exeAssyPath = exeAssy.Location;
      var exeAssyDir = Path.GetDirectoryName(exeAssyPath);
      var assyPaths = Directory.EnumerateFiles(exeAssyDir, "NHSD.*.dll");

      var useCRM = Settings.USE_CRM(Configuration);
      if (useCRM)
      {
        assyPaths = assyPaths.Where(x => !x.Contains("Database"));
      }
      else
      {
        assyPaths = assyPaths.Where(x => !x.Contains("CRM"));
      }

      // exclude test assys which are placed here by Docker build
      assyPaths = assyPaths.Where(x => !x.Contains("Test"));

      var assys = assyPaths.Select(filePath => Assembly.LoadFile(filePath)).ToList();
      assys.Add(exeAssy);

      builder
        .RegisterAssemblyTypes(assys.ToArray())
        .PublicOnly()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder.Register(cc => Configuration).As<IConfiguration>();

      // load configuration from autofac.json
      var module = new ConfigurationModule(Configuration);
      builder.RegisterModule(module);

      ApplicationContainer = builder.Build();

      // Create the IServiceProvider based on the container.
      return new AutofacServiceProvider(ApplicationContainer);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory logging)
    {
      ServiceProvider = app.ApplicationServices;

      if (CurrentEnvironment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseExceptionHandler(options =>
        {
          options.Run(async context =>
          {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/html";
            var ex = context.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
              var logger = ServiceProvider.GetService<ILogger<Startup>>();
              var err = $"Error: {ex.Error.Message}{Environment.NewLine}{ex.Error.StackTrace }";
              logger.LogError(err);
              if (CurrentEnvironment.IsDevelopment())
              {
                await context.Response.WriteAsync(err).ConfigureAwait(false);
              }
            }
          });
        }
      );

      app.UseAuthentication();

      if (CurrentEnvironment.IsDevelopment())
      {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(opts =>
        {
#pragma warning disable S1075 // URIs should not be hardcoded
          opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Buying Catalog API V1");
          opts.SwaggerEndpoint("/swagger/porcelain/swagger.json", "Buying Catalog API V1 Porcelain");
#pragma warning restore S1075 // URIs should not be hardcoded

          opts.DocExpansion(DocExpansion.None);
        });
      }

      app.UseStaticFiles();
      app.UseMvc();
    }

    private void DumpSettings()
    {
      Console.WriteLine("Settings:");
      Console.WriteLine($"  CRM:");
      Console.WriteLine($"    CRM_CLIENTID                      : {Settings.CRM_CLIENTID(Configuration)}");
      Console.WriteLine($"    CRM_CLIENTSECRET                  : {Settings.CRM_CLIENTSECRET(Configuration)}");
      Console.WriteLine($"    CRM_CACHE_EXPIRY_MINS             : {Settings.CRM_CACHE_EXPIRY_MINS(Configuration)}");
      Console.WriteLine($"    CRM_SHORT_TERM_CACHE_EXPIRY_SECS  : {Settings.CRM_SHORT_TERM_CACHE_EXPIRY_SECS(Configuration)}");

      Console.WriteLine($"  GIF:");
      Console.WriteLine($"    GIF_CRM_URL                 : {Settings.GIF_CRM_URL(Configuration)}");
      Console.WriteLine($"    GIF_AUTHORITY_URI           : {Settings.GIF_AUTHORITY_URI(Configuration)}");
      Console.WriteLine($"    GIF_AZURE_CLIENT_ID         : {Settings.GIF_AZURE_CLIENT_ID(Configuration)}");
      Console.WriteLine($"    GIF_ENCRYPTED_CLIENT_SECRET : {Settings.GIF_ENCRYPTED_CLIENT_SECRET(Configuration)}");

      Console.WriteLine($"  USE_CRM:");
      Console.WriteLine($"    USE_CRM : {Settings.USE_CRM(Configuration)}");

      Console.WriteLine($"  DATASTORE:");
      Console.WriteLine($"    DATASTORE_CONNECTION        : {Settings.DATASTORE_CONNECTION(Configuration)}");
      Console.WriteLine($"    DATASTORE_CONNECTIONTYPE    : {Settings.DATASTORE_CONNECTIONTYPE(Configuration, Settings.DATASTORE_CONNECTION(Configuration))}");
      Console.WriteLine($"    DATASTORE_CONNECTIONSTRING  : {Settings.DATASTORE_CONNECTIONSTRING(Configuration, Settings.DATASTORE_CONNECTION(Configuration))}");

      Console.WriteLine($"  LOG:");
      Console.WriteLine($"    LOG_CONNECTIONSTRING : {Settings.LOG_CONNECTIONSTRING(Configuration)}");
      Console.WriteLine($"    LOG_CRM              : {Settings.LOG_CRM(Configuration)}");
      Console.WriteLine($"    LOG_SHAREPOINT       : {Settings.LOG_SHAREPOINT(Configuration)}");
      Console.WriteLine($"    LOG_BEARERAUTH       : {Settings.LOG_BEARERAUTH(Configuration)}");

      Console.WriteLine($"  OIDC:");
      Console.WriteLine($"    OIDC_USERINFO_URL : {Settings.OIDC_USERINFO_URL(Configuration)}");
      Console.WriteLine($"    OIDC_ISSUER_URL   : {Settings.OIDC_ISSUER_URL(Configuration)}");
      Console.WriteLine($"    OIDC_AUDIENCE     : {Settings.OIDC_AUDIENCE(Configuration)}");

      Console.WriteLine($"  CACHE:");
      Console.WriteLine($"    CACHE_HOST : {Settings.CACHE_HOST(Configuration)}");
    }
  }
}
