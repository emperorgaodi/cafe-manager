using Asp.Versioning;
using CafeManager.Application.Cafes.Commands.CreateCafe;
using CafeManager.Application.Cafes.Commands.DeleteCafe;
using CafeManager.Application.Cafes.Commands.UpdateCafe;
using CafeManager.Application.Cafes.Queries.GetCafes;
using CafeManager.API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CafeManager.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CafesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CafesController> _logger;

    public CafesController(IMediator mediator, ILogger<CafesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>GET /api/v1/cafes?location= — Returns all cafés sorted by employee count.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CafeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCafes([FromQuery] string? location, CancellationToken ct)
    {
        _logger.LogInformation("Fetching cafés. LocationFilter={Location}", location ?? "none");
        var cafes = await _mediator.Send(new GetCafesQuery(location), ct);
        return Ok(cafes);
    }

    /// <summary>POST /api/v1/cafes — Creates a new café.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCafe([FromForm] CreateCafeRequest request, CancellationToken ct)
    {
        string? logoPath = null;
        if (request.Logo != null)
        {
            await FileUploadValidator.ValidateAsync(request.Logo);
            logoPath = await SaveLogoAsync(request.Logo);
        }

        var id = await _mediator.Send(new CreateCafeCommand(request.Name, request.Description, request.Location, logoPath), ct);
        _logger.LogInformation("Café created. Id={CafeId}", id);
        return CreatedAtAction(nameof(GetCafes), new { id }, new { id });
    }

    /// <summary>PUT /api/v1/cafes/{id} — Updates an existing café.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCafe(Guid id, [FromForm] UpdateCafeRequest request, CancellationToken ct)
    {
        string? logoPath = null;
        if (request.Logo != null)
        {
            await FileUploadValidator.ValidateAsync(request.Logo);
            logoPath = await SaveLogoAsync(request.Logo);
        }

        await _mediator.Send(new UpdateCafeCommand(id, request.Name, request.Description, request.Location, logoPath ?? request.ExistingLogo), ct);
        _logger.LogInformation("Café updated. Id={CafeId}", id);
        return NoContent();
    }

    /// <summary>DELETE /api/v1/cafes/{id} — Deletes a café and all its employees.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCafe(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCafeCommand(id), ct);
        _logger.LogInformation("Café deleted. Id={CafeId}", id);
        return NoContent();
    }

    private static async Task<string> SaveLogoAsync(IFormFile logo)
    {
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(logo.FileName).ToLowerInvariant()}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await logo.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }
}

public record CreateCafeRequest(string Name, string Description, string Location, IFormFile? Logo);
public record UpdateCafeRequest(string Name, string Description, string Location, IFormFile? Logo, string? ExistingLogo);
