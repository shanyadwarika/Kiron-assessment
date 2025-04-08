public class CoinApiResponse
{
    public Meta Meta { get; set; }
    public List<Coin> Result { get; set; }
}

public class Meta
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int ItemCount { get; set; }
    public int PageCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public class Coin
{
    public string Id { get; set; }
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int Rank { get; set; }
    public double Price { get; set; }
    public double PriceBtc { get; set; }
    public double Volume { get; set; }
    public double MarketCap { get; set; }
    public double AvailableSupply { get; set; }
    public double TotalSupply { get; set; }
    public double FullyDilutedValuation { get; set; }
    public double PriceChange1h { get; set; }
    public double PriceChange1d { get; set; }
    public double PriceChange1w { get; set; }
    public string WebsiteUrl { get; set; }
    public string RedditUrl { get; set; }
    public string TwitterUrl { get; set; }
    public string ContractAddress { get; set; }
    public int? Decimals { get; set; }
    public List<string> Explorers { get; set; }

    // Optional advanced stats
    public double? LiquidityScore { get; set; }
    public double? VolatilityScore { get; set; }
    public double? MarketCapScore { get; set; }
    public double? RiskScore { get; set; }
    public double? AvgChange { get; set; }
}
