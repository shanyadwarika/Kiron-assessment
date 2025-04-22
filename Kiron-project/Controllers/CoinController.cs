using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CoinController : ControllerBase
{
    private readonly CoinRepository _repository;
    private readonly ICacheService _cacheLayer;

    public CoinController(CoinRepository repository, ICacheService cacheLayer)
    {
        _repository = repository;
        _cacheLayer = cacheLayer;
    }

    [HttpGet("getCoinStats")]
    public async Task<IActionResult> GetCoinStatsAsync([FromQuery] CoinQueryParams queryParams)
    {
        var result = await _repository.GetCoinStatsAsync(queryParams);
        if (result == null) return StatusCode(500, "Unable to fetch coin stats.");
        return Ok(result);
    }
}