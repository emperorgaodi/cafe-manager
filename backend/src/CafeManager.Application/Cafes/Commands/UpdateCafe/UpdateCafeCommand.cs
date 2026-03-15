using CafeManager.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace CafeManager.Application.Cafes.Commands.UpdateCafe;

public record UpdateCafeCommand(Guid Id, string Name, string Description, string Location, string? Logo) : IRequest;

public class UpdateCafeCommandValidator : AbstractValidator<UpdateCafeCommand>
{
    public UpdateCafeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Name must be at least 6 characters.")
            .MaximumLength(10).WithMessage("Name must not exceed 10 characters.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Location).NotEmpty();
    }
}

public class UpdateCafeCommandHandler : IRequestHandler<UpdateCafeCommand>
{
    private readonly ICafeRepository _cafeRepository;

    public UpdateCafeCommandHandler(ICafeRepository cafeRepository)
    {
        _cafeRepository = cafeRepository;
    }

    public async Task Handle(UpdateCafeCommand request, CancellationToken cancellationToken)
    {
        var cafe = await _cafeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Café '{request.Id}' was not found.");

        cafe.Update(request.Name, request.Description, request.Location, request.Logo);
        _cafeRepository.Update(cafe);
        await _cafeRepository.SaveChangesAsync(cancellationToken);
    }
}
