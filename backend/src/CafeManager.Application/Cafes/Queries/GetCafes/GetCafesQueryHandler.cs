using CafeManager.Domain.Repositories;
using MediatR;

namespace CafeManager.Application.Cafes.Queries.GetCafes;

public class GetCafesQueryHandler : IRequestHandler<GetCafesQuery, IEnumerable<CafeDto>>
{
    private readonly ICafeRepository _cafeRepository;

    public GetCafesQueryHandler(ICafeRepository cafeRepository)
    {
        _cafeRepository = cafeRepository;
    }

    public async Task<IEnumerable<CafeDto>> Handle(GetCafesQuery request, CancellationToken cancellationToken)
    {
        var cafes = await _cafeRepository.GetAllAsync(request.Location, cancellationToken);

        return cafes
            .Select(cafe => new CafeDto(
                Id: cafe.Id,
                Name: cafe.Name,
                Description: cafe.Description,
                Employees: cafe.CafeEmployees.Count,
                Logo: cafe.Logo,
                Location: cafe.Location))
            .OrderByDescending(dto => dto.Employees);
    }
}
