using CafeManager.Domain.Entities;
using CafeManager.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace CafeManager.Application.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(string Id, string Name, string EmailAddress, string PhoneNumber, string Gender, Guid? CafeId) : IRequest;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(6).MaximumLength(30);
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^[89]\d{7}$")
            .WithMessage("Phone must start with 8 or 9 and be 8 digits.");
        RuleFor(x => x.Gender).NotEmpty().Must(g => g == "Male" || g == "Female");
    }
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICafeRepository _cafeRepository;

    public UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository, ICafeRepository cafeRepository)
    {
        _employeeRepository = employeeRepository;
        _cafeRepository = cafeRepository;
    }

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee '{request.Id}' was not found.");

        employee.Update(request.Name, request.EmailAddress, request.PhoneNumber, Enum.Parse<Gender>(request.Gender));

        if (request.CafeId.HasValue)
        {
            if (!await _cafeRepository.ExistsAsync(request.CafeId.Value, cancellationToken))
                throw new KeyNotFoundException($"Café '{request.CafeId}' was not found.");

            employee.AssignToCafe(request.CafeId.Value);
        }
        else
        {
            employee.UnassignFromCafe();
        }

        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
    }
}
