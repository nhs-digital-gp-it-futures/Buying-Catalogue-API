using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

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

      services.AddApiVersioning(options =>
      {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = DefaultValues.ApiVersion;
      });

      if (CurrentEnvironment.IsDevelopment())
      {
        // Register the Swagger generator, defining one or more Swagger documents
        services.AddSwaggerGen(options =>
        {
          // add a custom operation filter which sets default values
          options.OperationFilter<SwaggerDefaultValues>();

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

          // Set the comments path for the Swagger JSON and UI.
          var xmlPath = Path.Combine(AppContext.BaseDirectory, "NHSD.GPITF.BuyingCatalog.xml");
          options.IncludeXmlComments(xmlPath);
          options.DescribeAllEnumsAsStrings();
        });
      }

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
      Console.WriteLine($"  DATASTORE:");
      Console.WriteLine($"    DATASTORE_CONNECTION        : {Settings.DATASTORE_CONNECTION(Configuration)}");
      Console.WriteLine($"    DATASTORE_CONNECTIONTYPE    : {Settings.DATASTORE_CONNECTIONTYPE(Configuration, Settings.DATASTORE_CONNECTION(Configuration))}");
      Console.WriteLine($"    DATASTORE_CONNECTIONSTRING  : {Settings.DATASTORE_CONNECTIONSTRING(Configuration, Settings.DATASTORE_CONNECTION(Configuration))}");

      Console.WriteLine($"  LOG:");
      Console.WriteLine($"    LOG_CONNECTIONSTRING : {Settings.LOG_CONNECTIONSTRING(Configuration)}");
    }
  }
}
