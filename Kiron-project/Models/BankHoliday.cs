using System.Text.Json.Serialization;

public class BankHoliday
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public bool Bunting { get; set; }
    public string Region { get; set; }
}

public class RegionBankHolidays
{
    public string Region { get; set; }
    public List<BankHoliday> Events { get; set; }
}

public class BankHolidayApiResponse
{
    [JsonPropertyName("england-and-wales")]
    public RegionBankHolidays EnglandAndWales { get; set; }

    [JsonPropertyName("scotland")]
    public RegionBankHolidays Scotland { get; set; }

    [JsonPropertyName("northern-ireland")]
    public RegionBankHolidays NorthernIreland { get; set; }
}
