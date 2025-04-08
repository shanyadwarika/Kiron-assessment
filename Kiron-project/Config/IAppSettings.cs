public interface IAppSettings
{
    string DatabaseConnection { get; }
    string AuthenticationSecurityKey { get; }
    string BankHolidaysUrl { get; }
    string CoinUrl { get; }
}