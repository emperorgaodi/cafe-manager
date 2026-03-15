using CafeManager.Domain.Repositories;
using MediatR;

namespace CafeManager.Application.Cafes.Commands.DeleteCafe;

public record DeleteCafeCommand(Guid Id) : IRequest;

public class DeleteCafeCommandHandler : IRequestHandler<DeleteCafeCommand>
{
    private readonly ICafeRepository _cafeRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteCafeCommandHandler(ICafeRepository cafeRepository, IEmployeeRepository employeeRepository)
    {
        _cafeRepository = cafeRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task Handle(DeleteCafeCommand request, CancellationToken cancellationToken)
    {
        var cafe = await _cafeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Café '{request.Id}' was not found.");

        // The spec requires deleting all employees that belong to this café.
        // The DB cascade only removes CafeEmployee join records — Employee rows must be
        // deleted explicitly before the café is removed.
        var assignedEmployees = cafe.CafeEmployees.Select(ce => ce.Employee).ToList();
        foreach (var employee in assignedEmployees)
        {
            _employeeRepository.Delete(employee);
        }

        _cafeRepository.Delete(cafe);
        await _cafeRepository.SaveChangesAsync(cancellationToken);
    }
}
