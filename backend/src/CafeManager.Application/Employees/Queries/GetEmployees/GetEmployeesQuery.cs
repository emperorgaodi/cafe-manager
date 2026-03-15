using CafeManager.Domain.Repositories;
using MediatR;

namespace CafeManager.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery(Guid? CafeId) : IRequest<IEnumerable<EmployeeDto>>;

public record EmployeeDto(
    string Id,
    string Name,
    string EmailAddress,
    string PhoneNumber,
    string Gender,
    int DaysWorked,
    // Named 'Cafe' to match the spec field name exactly.
    // Serialises to "cafe" in the JSON response. Leave blank if not assigned.
    string? Cafe
);

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(request.CafeId, cancellationToken);

        return employees
            .Select(emp => new EmployeeDto(
                Id: emp.Id,
                Name: emp.Name,
                EmailAddress: emp.EmailAddress,
                PhoneNumber: emp.PhoneNumber,
                Gender: emp.Gender.ToString(),
                DaysWorked: emp.CafeEmployee?.DaysWorked ?? 0,
                Cafe: emp.CafeEmployee?.Cafe?.Name))   // null → omitted / blank per spec
            .OrderByDescending(dto => dto.DaysWorked);
    }
}
