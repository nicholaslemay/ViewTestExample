using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;

namespace ViewTestExample.Support
{
    public static class TestServiceScopeFactory
    {
        public static IServiceScopeFactory Build(string applicationName)
        {
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, applicationName);

            // Add a custom service that is used in the view. ex: services.AddSingleton<EmailReportGenerator>();
            return services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
        }

        private static void ConfigureDefaultServices(IServiceCollection services, string applicationName)
        {
            IFileProvider fileProvider = new PhysicalFileProvider($"{Directory.GetCurrentDirectory()}/../../../../{applicationName}/");

            services.AddSingleton<IWebHostEnvironment>(new WebHostEnvironment
            {
                ApplicationName = applicationName,
                WebRootFileProvider = fileProvider
            });

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddSingleton(diagnosticSource);
            services.AddSingleton<IHostEnvironmentNavigationManager>(new FakeIHostEnvironmentNavigationManager());
            services.AddSingleton<NavigationManager>(new TestNavigationManager());
            services.AddLogging();

            var builder = services.AddMvcCore();
            builder.AddViews();
            builder.AddRazorViewEngine();
            services.AddTransient<RazorViewToStringRenderer>();

            // services.Configure<MvcRazorRuntimeCompilationOptions>(options => { options.FileProviders.Clear(); options.FileProviders.Add(fileProvider);});
            // services.AddMvcCore().AddRazorPages(options =>
            // { options.RootDirectory = "/"; options.Conventions.AddPageRoute("/Accounts/Registration/Login", "/account/login");
            // });
        }
    }

    internal class FakeIHostEnvironmentNavigationManager : IHostEnvironmentNavigationManager
    {
        public void Initialize(string baseUri, string uri) {}
    }

    internal class WebHostEnvironment : IWebHostEnvironment
    {
        public string? ApplicationName { get; set; }
        public IFileProvider? ContentRootFileProvider { get; set; }
        public string? ContentRootPath { get; set; }
        public string? EnvironmentName { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; } = "/";
    }

    internal class TestNavigationManager : NavigationManager
    {
        private string LastRedirect { get; set; } = "/";

        public TestNavigationManager()
        {
            BaseUri = "http://localhost/";
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad) => LastRedirect = uri;
    }
}