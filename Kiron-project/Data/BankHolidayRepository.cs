using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class BankHolidayRepository : Repository<BankHoliday>
{
    private readonly HttpClient _httpClient;
    private readonly KironDbContext _context;
    private readonly IAppSettings _appSettings;
    private readonly ICacheService _cacheService;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public BankHolidayRepository(
        KironDbContext context,
        HttpClient httpClient,
        IAppSettingsService appSettingsService,
        ICacheService cacheService)
        : base(context)
    {
        _context = context;
        _httpClient = httpClient;
        _appSettings = appSettingsService.GetAppSettings();
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<List<BankHoliday>> GetAndSaveFromApiAsync()
    {
        string BankHolidaysCacheKey = "AllBankHolidays";
        var cached = _cacheService.Get<List<BankHoliday>>(BankHolidaysCacheKey);
        if (cached != null)
            return cached;

        await _semaphore.WaitAsync();
        try
        {
            // Double-check inside the lock
            cached = _cacheService.Get<List<BankHoliday>>(BankHolidaysCacheKey);
            if (cached != null)
                return cached;

            BankHolidayApiResponse bankHolidaysResponse = await GetBankHolidaysAsync();

            if (bankHolidaysResponse == null) return new List<BankHoliday>();

            // Map regions and their events dynamically
            var regions = new Dictionary<string, RegionBankHolidays>
        {
            { "england-and-wales", bankHolidaysResponse.EnglandAndWales },
            { "scotland", bankHolidaysResponse.Scotland },
            { "northern-ireland", bankHolidaysResponse.NorthernIreland }
        };

            var events = regions
                .Where(r => r.Value?.Events != null)
                .SelectMany(r => r.Value.Events.Select(e => new BankHoliday
                {
                    Title = e.Title,
                    Date = e.Date,
                    Notes = e.Notes,
                    Bunting = e.Bunting,
                    Region = r.Key
                }))
                .ToList();

            await _context.BankHoliday.AddRangeAsync(events);
            await _context.SaveChangesAsync();
            _cacheService.Set(BankHolidaysCacheKey, events, TimeSpan.FromMinutes(30));

            return events;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<BankHolidayApiResponse> GetBankHolidaysAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            var uri = _appSettings.BankHolidaysUrl;
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                var status = response.StatusCode;

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<BankHolidayApiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data;
            }
        }

    }

    public async Task<List<string>> GetAllRegionsAsync()
    {
        const string cacheKey = "AllRegions";
        var cached = _cacheService.Get<List<string>>(cacheKey);
        if (cached != null)
            return cached;

        await _semaphore.WaitAsync();

        try
        {
            // Check again in case it was set while waiting
            cached = _cacheService.Get<List<string>>(cacheKey);

            if (cached != null)
                return cached;

            var regions = await _context.BankHoliday
                                    .Select(b => b.Region)
                                    .Distinct()
                                    .ToListAsync();

            _cacheService.Set(cacheKey, regions, TimeSpan.FromMinutes(30));
            return regions;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<BankHoliday>> GetByRegionAsync(string region)
    {
        var cacheKey = $"Holidays_{region}";
        var cached = _cacheService.Get<List<BankHoliday>>(cacheKey);
        if (cached != null)
            return cached;

        await _semaphore.WaitAsync();
        try
        {
            cached = _cacheService.Get<List<BankHoliday>>(cacheKey);
            if (cached != null)
                return cached;

            var holidays = await _context.BankHoliday
                                    .Where(b => b.Region.ToLower() == region.ToLower())
                                    .ToListAsync();

            _cacheService.Set(cacheKey, holidays, TimeSpan.FromMinutes(30));
            return holidays;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
