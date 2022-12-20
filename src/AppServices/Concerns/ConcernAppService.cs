using AutoMapper;
using Cts.AppServices.UserServices;
using Cts.Domain.Concerns;

namespace Cts.AppServices.Concerns;

public sealed class ConcernAppService : IConcernAppService
{
    private readonly IConcernRepository _repository;
    private readonly IConcernManager _manager;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ConcernAppService(
        IConcernRepository repository,
        IConcernManager manager,
        IMapper mapper,
        IUserService userService)
    {
        _repository = repository;
        _manager = manager;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<ConcernUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default)
    {
        var item = await _repository.FindAsync(id, token);
        return _mapper.Map<ConcernUpdateDto>(item);
    }

    public async Task<IReadOnlyList<ConcernViewDto>> GetListAsync(CancellationToken token = default)
    {
        var list = (await _repository.GetListAsync(token)).OrderBy(e => e.Name).ToList();
        return _mapper.Map<List<ConcernViewDto>>(list);
    }

    public async Task<Guid> CreateAsync(string name, CancellationToken token = default)
    {
        var item = await _manager.CreateAsync(name, token);
        item.SetCreator((await _userService.GetCurrentUserAsync())?.Id);
        await _repository.InsertAsync(item, token: token);
        return item.Id;
    }

    public async Task UpdateAsync(ConcernUpdateDto resource, CancellationToken token = default)
    {
        var item = await _repository.GetAsync(resource.Id, token);

        if (item.Name != resource.Name.Trim())
            await _manager.ChangeNameAsync(item, resource.Name, token);
        item.Active = resource.Active;
        item.SetUpdater((await _userService.GetCurrentUserAsync())?.Id);

        await _repository.UpdateAsync(item, token: token);
    }

    public void Dispose() => _repository.Dispose();
}
