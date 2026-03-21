using System.Security.Claims;
using HowGoodIsMyExcuse.Api.DTOs;
using HowGoodIsMyExcuse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HowGoodIsMyExcuse.Api.Controllers;

[ApiController]
[Route("api/excuses")]
public class ExcuseController : ControllerBase
{
    private readonly IExcuseService _excuseService;

    public ExcuseController(IExcuseService excuseService)
    {
        _excuseService = excuseService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SubmitExcuse([FromBody] SubmitExcuseRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { error = "Invalid token." });

        try
        {
            var result = await _excuseService.SubmitExcuse(request, userId.Value);
            return CreatedAtAction(nameof(GetExcuseById), new { id = result.Id }, result);
        }
        catch (GroqServiceException ex)
        {
            return StatusCode(502, new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, 50);
        var currentUserId = GetCurrentUserId();
        var result = await _excuseService.GetLeaderboard(page, pageSize, currentUserId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExcuseById(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _excuseService.GetExcuseById(id, currentUserId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Excuse not found." });
        }
    }

    [HttpPost("{id:guid}/vote")]
    [Authorize]
    public async Task<IActionResult> VoteOnExcuse(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { error = "Invalid token." });

        try
        {
            var result = await _excuseService.VoteOnExcuse(id, userId.Value);
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Already voted.")
        {
            return Conflict(new { error = "You have already voted on this excuse." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Excuse not found." });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}