using Asp.Versioning;
using CafeManager.Application.Employees.Commands.CreateEmployee;
using CafeManager.Application.Employees.Commands.DeleteEmployee;
using CafeManager.Application.Employees.Commands.UpdateEmployee;
using CafeManager.Application.Employees.Queries.GetEmployees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CafeManager.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>GET /api/v1/employees?cafe= — Returns all employees sorted by days worked.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployees([FromQuery] Guid? cafe, CancellationToken ct)
    {
        _logger.LogInformation("Fetching employees. CafeFilter={CafeId}", cafe?.ToString() ?? "none");
        var employees = await _mediator.Send(new GetEmployeesQuery(cafe), ct);
        return Ok(employees);
    }

    /// <summary>POST /api/v1/employees — Creates a new employee.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(
            new CreateEmployeeCommand(request.Name, request.EmailAddress, request.PhoneNumber, request.Gender, request.CafeId), ct);

        _logger.LogInformation("Employee created. Id={EmployeeId}", id);
        return CreatedAtAction(nameof(GetEmployees), new { id }, new { id });
    }

    /// <summary>PUT /api/v1/employees/{id} — Updates an employee's details and café assignment.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployee(string id, [FromBody] UpdateEmployeeRequest request, CancellationToken ct)
    {
        await _mediator.Send(
            new UpdateEmployeeCommand(id, request.Name, request.EmailAddress, request.PhoneNumber, request.Gender, request.CafeId), ct);

        _logger.LogInformation("Employee updated. Id={EmployeeId}", id);
        return NoContent();
    }

    /// <summary>DELETE /api/v1/employees/{id} — Deletes an employee.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployee(string id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id), ct);
        _logger.LogInformation("Employee deleted. Id={EmployeeId}", id);
        return NoContent();
    }
}

public record CreateEmployeeRequest(string Name, string EmailAddress, string PhoneNumber, string Gender, Guid? CafeId);
public record UpdateEmployeeRequest(string Name, string EmailAddress, string PhoneNumber, string Gender, Guid? CafeId);
