using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/bank-holidays")]
public class BankHolidaysController : ControllerBase
{
    private readonly BankHolidayRepository _repository;

    public BankHolidaysController(BankHolidayRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("getAndSaveFromApiAsync")]
    public async Task<IActionResult> RefreshFromApi()
    {
        var saved = await _repository.GetAndSaveFromApiAsync();
        return Ok(saved);
    }

    [HttpGet("regions")]
    public async Task<IActionResult> GetRegions()
    {
        var regions = await _repository.GetAllRegionsAsync();
        return Ok(regions);
    }

    [HttpGet("region/{region}")]
    public async Task<IActionResult> GetByRegion(string region)
    {
        var holidays = await _repository.GetByRegionAsync(region);
        return Ok(holidays);
    }
}
