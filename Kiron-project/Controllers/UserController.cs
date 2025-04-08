using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserRepository _repository;
    

    public UserController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {
        await _repository.RegisterUserAsync(request);
        return Ok("User registered successfully.");
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var token = _repository.LoginAsync(request);
        if (token == null)
            return Unauthorized("Invalid credentials");
        
        return Ok(new { token });
    }
}
