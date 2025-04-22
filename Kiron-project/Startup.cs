using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddControllers().AddNewtonsoftJson();
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        var module = services.FirstOrDefault(t => t.ImplementationFactory?.GetType() == typeof(Func<IServiceProvider, DependencyTrackingTelemetryModule>));
        if (module != null)
        {
            services.Remove(module);
        }

        services.AddDbContext<KironDbContext>((serviceProvider, options) =>
        {
            var appSettingsService = serviceProvider.GetRequiredService<IAppSettingsService>();
            var appSettings = appSettingsService.GetAppSettings();

            options.UseSqlServer(appSettings.DatabaseConnection, sqlOptions =>
            {
                sqlOptions.MaxBatchSize(10);
            });
        });

        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        services.AddSingleton(new AppSettingsService() as IAppSettingsService);

        services.Configure<FormOptions>(x => x.ValueCountLimit = 12048);

        services.Add(new ServiceDescriptor(typeof(BankHolidayRepository), typeof(BankHolidayRepository), ServiceLifetime.Transient));
        services.Add(new ServiceDescriptor(typeof(CoinRepository), typeof(CoinRepository), ServiceLifetime.Transient));
        services.Add(new ServiceDescriptor(typeof(NavigationRepository), typeof(NavigationRepository), ServiceLifetime.Transient));
        services.Add(new ServiceDescriptor(typeof(UserRepository), typeof(UserRepository), ServiceLifetime.Transient));
        services.Add(new ServiceDescriptor(typeof(AuthenticationService), typeof(AuthenticationService), ServiceLifetime.Transient));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseValidateJwtToken();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());

    }
}