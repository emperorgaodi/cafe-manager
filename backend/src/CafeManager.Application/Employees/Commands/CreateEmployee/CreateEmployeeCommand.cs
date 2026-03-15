using CafeManager.Domain.Entities;
using CafeManager.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace CafeManager.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(string Name, string EmailAddress, string PhoneNumber, string Gender, Guid? CafeId) : IRequest<string>;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Name must be at least 6 characters.")
            .MaximumLength(30).WithMessage("Name must not exceed 30 characters.");
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
        
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^[89]\d{7}$")
            .WithMessage("Phone must start with 8 or 9 and be 8 digits.");
        RuleFor(x => x.Gender).NotEmpty().Must(g => g == "Male" || g == "Female")
            .WithMessage("Gender must be 'Male' or 'Female'.");
    }
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, string>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICafeRepository _cafeRepository;

    public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, ICafeRepository cafeRepository)
    {
        _employeeRepository = employeeRepository;
        _cafeRepository = cafeRepository;
    }

    public async Task<string> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var gender = Enum.Parse<Gender>(request.Gender);
        var employee = Employee.Create(request.Name, request.EmailAddress, request.PhoneNumber, gender);

        if (request.CafeId.HasValue)
        {
            if (!await _cafeRepository.ExistsAsync(request.CafeId.Value, cancellationToken))
                throw new KeyNotFoundException($"Café '{request.CafeId}' was not found.");

            employee.AssignToCafe(request.CafeId.Value);
        }

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return employee.Id;
    }
}
