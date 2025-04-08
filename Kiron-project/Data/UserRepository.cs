using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

public class UserRepository : Repository<User>
{
    private readonly HttpClient _httpClient;
    private readonly KironDbContext _context;
    private readonly IAppSettings _appSettings;
    private readonly AuthenticationService _authService;

    public UserRepository(
        KironDbContext context,
        HttpClient httpClient,
        IAppSettingsService appSettingsService,
        AuthenticationService authService
        )
        : base(context)
    {
        _context = context;
        _httpClient = httpClient;
        _appSettings = appSettingsService.GetAppSettings();
        _authService = authService;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = PasswordHelper.HashPassword(request.Password)
        };

        var validUser = await GetUserByUsernameAndPassword(user.Username, user.PasswordHash);
               
        var token = string.Empty;
        if (validUser != null)
        {
            token = _authService.GenerateJwtToken(user);
        }

        return token;
    }

    private async Task<User> GetUserByUsernameAndPassword(string username, string passwordHash)
    {
        return await _context.Users
         .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash);
    }

    public async Task RegisterUserAsync(LoginRequest request)
    {
        var passwordHash = PasswordHelper.HashPassword(request.Password);
        var existingUser = await GetUserByUsernameAndPassword(request.Username, passwordHash);
        if (existingUser != null)
        {
            return;
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash
        };


        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
