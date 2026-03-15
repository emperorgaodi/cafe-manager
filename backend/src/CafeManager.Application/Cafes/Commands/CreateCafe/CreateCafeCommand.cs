using CafeManager.Domain.Entities;
using CafeManager.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace CafeManager.Application.Cafes.Commands.CreateCafe;

public record CreateCafeCommand(string Name, string Description, string Location, string? Logo) : IRequest<Guid>;

public class CreateCafeCommandValidator : AbstractValidator<CreateCafeCommand>
{
    public CreateCafeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Name must be at least 6 characters.")
            .MaximumLength(10).WithMessage("Name must not exceed 10 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(256).WithMessage("Description must not exceed 256 characters.");

        RuleFor(x => x.Location).NotEmpty();
    }
}

public class CreateCafeCommandHandler : IRequestHandler<CreateCafeCommand, Guid>
{
    private readonly ICafeRepository _cafeRepository;

    public CreateCafeCommandHandler(ICafeRepository cafeRepository)
    {
        _cafeRepository = cafeRepository;
    }

    public async Task<Guid> Handle(CreateCafeCommand request, CancellationToken cancellationToken)
    {
        var cafe = Cafe.Create(request.Name, request.Description, request.Location, request.Logo);
        await _cafeRepository.AddAsync(cafe, cancellationToken);
        await _cafeRepository.SaveChangesAsync(cancellationToken);
        return cafe.Id;
    }
}
