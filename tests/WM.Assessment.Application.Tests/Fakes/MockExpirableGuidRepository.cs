using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WM.Assessment.Domain.ExpirableGuids;

namespace WM.Assessment.Application.Tests.Fakes;

public class MockExpirableGuidRepository : IExpirableGuidRepository
{
    private List<ExpirableGuid> _guids;

    public MockExpirableGuidRepository()
    {
        _guids = new List<ExpirableGuid>();
    }

    public Task<ExpirableGuid> GetAsync(string guid)
    {
        var item = _guids.FirstOrDefault(x => x.Guid == guid);
        return Task.FromResult(item);
    }

    public Task SaveAsync(ExpirableGuid expirableGuid)
    {
        _guids.Add(expirableGuid);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ExpirableGuid expirableGuid)
    {
        _guids = _guids.Where(x => x.Guid != expirableGuid.Guid).ToList();
        return Task.CompletedTask;
    }
}