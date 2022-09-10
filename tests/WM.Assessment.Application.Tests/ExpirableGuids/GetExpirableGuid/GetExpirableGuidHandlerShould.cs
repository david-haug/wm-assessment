using System;
using System.Threading;
using System.Threading.Tasks;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Application.ExpirableGuids.GetExpirableGuid;
using WM.Assessment.Application.Tests.Fakes;
using WM.Assessment.Domain.ExpirableGuids;
using Xunit;

namespace WM.Assessment.Application.Tests.ExpirableGuids.GetExpirableGuid;

public class GetExpirableGuidHandlerShould
{
    [Fact]
    public async void Return_ExpirableGuidResponse()
    {
        //setup
        //create expirable guid domain object and add to repo
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        var user = "Joe, Smith.";
        var expire = DateTimeOffset.UtcNow.AddDays(5);
        var domainObj = ExpirableGuid.Load(guid, user, expire);

        var repository = new MockExpirableGuidRepository();
        repository.SaveAsync(domainObj);

        var sut = new GetExpirableGuidHandler(repository);
        var actual = await sut.Handle(new GetExpirableGuidRequest {Guid = guid}, CancellationToken.None);
        Assert.Equal(guid, actual.Guid);
        Assert.Equal(user, actual.User);
        Assert.Equal(expire.ToUnixTimeSeconds().ToString(), actual.Expire);
    }

    [Fact]
    public async Task Throw_NotFoundException_given_invalid_guid()
    {
        var repository = new MockExpirableGuidRepository();
        var sut = new GetExpirableGuidHandler(repository);
        var request = new GetExpirableGuidRequest {Guid = "fake"};
        await Assert.ThrowsAsync<NotFoundException>(() => sut.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Throw_NotFoundException_when_expired()
    {
        //setup
        //create expirable guid domain object and add to repo
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        var user = "Joe, Smith.";
        var expire = DateTimeOffset.UtcNow.AddDays(-1); //set expired
        var domainObj = ExpirableGuid.Load(guid, user, expire);

        var repository = new MockExpirableGuidRepository();
        repository.SaveAsync(domainObj);

        var sut = new GetExpirableGuidHandler(repository);
        var request = new GetExpirableGuidRequest {Guid = guid};
        await Assert.ThrowsAsync<NotFoundException>(() => sut.Handle(request, CancellationToken.None));
    }
}