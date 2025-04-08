using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NavigationController : ControllerBase
{
    private readonly NavigationRepository _repository;

    public NavigationController(NavigationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("getNavigation")]
    public async Task<IActionResult> GetNavigation()
    {
        var result = await _repository.GetNavigationAsync();
        return Ok(result);
    }
}