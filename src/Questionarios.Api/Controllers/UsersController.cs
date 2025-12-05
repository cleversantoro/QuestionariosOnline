using Microsoft.AspNetCore.Mvc;
using Questionarios.Application.DTOs;
using Questionarios.Application.Services;

namespace Questionarios.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetAsync(id, ct);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto, CancellationToken ct)
    {
        var user = await _userService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto, CancellationToken ct)
    {
        await _userService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _userService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto, CancellationToken ct)
    {
        var user = await _userService.AuthenticateAsync(dto, ct);
        if (user is null)
            return Unauthorized();

        return Ok(user);
    }
}
