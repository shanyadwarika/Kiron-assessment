using System.Diagnostics;

public class AppSettingsService : IAppSettingsService
{
    private IConfigurationRoot _configuration;
    private AppSettings _appSettings;

    public AppSettingsService(string basePath = null)
    {
        string appSettingsFile = "appsettings.json";

        this._configuration = new ConfigurationBuilder()
             .SetBasePath(basePath == null ? AppContext.BaseDirectory : basePath)
             .AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();


    }

    public IAppSettings GetAppSettings()
    {
        if (_appSettings == null)
        {
            _appSettings = new AppSettings(this._configuration);
        }

        return _appSettings;
    }
}
