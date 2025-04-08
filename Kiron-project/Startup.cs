using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddControllers();//.AddNewtonsoftJson();
        services.AddHttpContextAccessor();
        services.AddHttpClient();


        var module = services.FirstOrDefault(t => t.ImplementationFactory?.GetType() == typeof(Func<IServiceProvider, DependencyTrackingTelemetryModule>));
        if (module != null)
        {
            services.Remove(module);
        }



        IAppSettingsService appSettingsService = new AppSettingsService();
        services.AddSingleton(new AppSettingsService() as IAppSettingsService);

        services.Configure<FormOptions>(x => x.ValueCountLimit = 12048);

        services.Add(new ServiceDescriptor(typeof(BankHolidayRepository), typeof(BankHolidayRepository), ServiceLifetime.Transient));
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