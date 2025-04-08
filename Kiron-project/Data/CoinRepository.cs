using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System.Text.Json;
using System.Threading;

public class CoinRepository : Repository<Coin>
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ICacheService _cacheService;
    private readonly KironDbContext _context;
    private readonly IAppSettings _appSettings;

    public CoinRepository(
        KironDbContext context, 
        ICacheService cacheService,
        IAppSettingsService appSettingsService)
        : base(context)
    {
        _context = context;
        _cacheService = cacheService;
        _appSettings = appSettingsService.GetAppSettings();
    }

    public async Task<CoinApiResponse> GetCoinStatsAsync(CoinQueryParams filter)
    {
        try
        {
            var client = new RestClient(_appSettings.CoinUrl);
            RestRequest restRequest = new RestRequest("", Method.Get);
            restRequest.AddHeader("accept", "application/json");

            AddQueryParams(restRequest, filter);

            var response = await client.ExecuteAsync(restRequest);
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new Exception("Failed to fetch Coin Stats.");
            }


            var result = JsonSerializer.Deserialize<CoinApiResponse>(response.Content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }
        catch (Exception ex) 
        {
            throw new Exception("Failed to fetch Coin Stats.");
        }
    }

    private void AddQueryParams(RestRequest request, CoinQueryParams filter)
    {
        void Add(string key, object value)
        {
            if (value != null)
                request.AddQueryParameter(key, value.ToString());
        }

        Add("page", filter.Page);
        Add("limit", filter.Limit);
        Add("currency", filter.Currency);
        Add("name", filter.Name);
        Add("symbol", filter.Symbol);
        Add("blockchains", filter.Blockchains);
        Add("includeRiskScore", filter.IncludeRiskScore?.ToString().ToLower());
        Add("categories", filter.Categories);
        Add("sortBy", filter.SortBy);
        Add("sortDir", filter.SortDir);

        Add("marketCap~greaterThan", filter.MarketCapGreaterThan);
        Add("marketCap~equals", filter.MarketCapEquals);
        Add("marketCap~lessThan", filter.MarketCapLessThan);

        Add("fullyDilutedValuation~greaterThan", filter.FullyDilutedValuationGreaterThan);
        Add("fullyDilutedValuation~equals", filter.FullyDilutedValuationEquals);
        Add("fullyDilutedValuation~lessThan", filter.FullyDilutedValuationLessThan);

        Add("volume~greaterThan", filter.VolumeGreaterThan);
        Add("volume~equals", filter.VolumeEquals);
        Add("volume~lessThan", filter.VolumeLessThan);

        Add("priceChange1h~greaterThan", filter.PriceChange1hGreaterThan);
        Add("priceChange1h~equals", filter.PriceChange1hEquals);
        Add("priceChange1h~lessThan", filter.PriceChange1hLessThan);

        Add("priceChange1d~greaterThan", filter.PriceChange1dGreaterThan);
        Add("priceChange1d~equals", filter.PriceChange1dEquals);
        Add("priceChange1d~lessThan", filter.PriceChange1dLessThan);

        Add("priceChange7d~greaterThan", filter.PriceChange7dGreaterThan);
        Add("priceChange7d~equals", filter.PriceChange7dEquals);
        Add("priceChange7d~lessThan", filter.PriceChange7dLessThan);

        Add("availableSupply~greaterThan", filter.AvailableSupplyGreaterThan);
        Add("availableSupply~equals", filter.AvailableSupplyEquals);
        Add("availableSupply~lessThan", filter.AvailableSupplyLessThan);

        Add("totalSupply~greaterThan", filter.TotalSupplyGreaterThan);
        Add("totalSupply~equals", filter.TotalSupplyEquals);
        Add("totalSupply~lessThan", filter.TotalSupplyLessThan);

        Add("rank~greaterThan", filter.RankGreaterThan);
        Add("rank~equals", filter.RankEquals);
        Add("rank~lessThan", filter.RankLessThan);

        Add("price~greaterThan", filter.PriceGreaterThan);
        Add("price~equals", filter.PriceEquals);
        Add("price~lessThan", filter.PriceLessThan);

        Add("riskScore~greaterThan", filter.RiskScoreGreaterThan);
        Add("riskScore~equals", filter.RiskScoreEquals);
        Add("riskScore~lessThan", filter.RiskScoreLessThan);
    }

}
