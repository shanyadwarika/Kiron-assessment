using Microsoft.Extensions.Configuration;
using System;

public class AppSettings : IAppSettings
{
    private readonly IConfigurationRoot _configuration;
    public AppSettings(IConfigurationRoot configuration)
    {
        _configuration = configuration;
    }

    public string DatabaseConnection { get { return _configuration["DatabaseConnection"]; } }
    public string AuthenticationSecurityKey { get { return _configuration["AuthenticationSecurityKey"]; } }
    public string BankHolidaysUrl { get { return _configuration["BankHolidaysUrl"]; } }
    public string CoinUrl { get { return _configuration["CoinUrl"]; } }
}
