using Microsoft.AspNetCore.Mvc;
using Questionarios.Application.DTOs;
using Questionarios.Application.Services;

namespace Questionarios.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponsesController : ControllerBase
{
    private readonly IResponseService _responseService;

    public ResponsesController(IResponseService responseService)
    {
        _responseService = responseService;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ResponseCreateDto dto, CancellationToken ct)
    {
        var response = await _responseService.SubmitAsync(dto, ct);
        return Ok(response);
    }
}
