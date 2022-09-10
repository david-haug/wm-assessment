using System;
using System.Threading;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Application.ExpirableGuids.UpdateExpirableGuid;
using WM.Assessment.Application.Tests.Fakes;
using WM.Assessment.Domain.ExpirableGuids;
using Xunit;

namespace WM.Assessment.Application.Tests.ExpirableGuids.UpdateExpirableGuid;

public class UpdateExpirableGuidHandlerShould
{
    [Fact]
    public async void Update_user_and_expire()
    {
        //setup
        //create expirable guid domain object and add to repo
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        var user = "Joe, Smith.";
        var expire = DateTimeOffset.UtcNow.AddDays(5);
        var domainObj = ExpirableGuid.Load(guid, user, expire);

        var repository = new MockExpirableGuidRepository();
        await repository.SaveAsync(domainObj);

        var request = new UpdateExpirableGuidRequest
        {
            Guid = guid,
            User = "new user",
            Expire = DateTimeOffset.UtcNow.AddDays(20).ToUnixTimeSeconds().ToString()
        };

        var sut = new UpdateExpirableGuidHandler(repository);
        var actual = await sut.Handle(request, CancellationToken.None);

        Assert.Equal(request.User, actual.User);
        Assert.Equal(request.Expire, actual.Expire);
    }

    [Fact]
    public async void Throw_NotFoundException_given_invalid_guid()
    {
        var request = new UpdateExpirableGuidRequest
        {
            Guid = "xxx",
            User = "new user",
            Expire = DateTimeOffset.UtcNow.AddDays(20).ToUnixTimeSeconds().ToString()
        };
        var sut = new UpdateExpirableGuidHandler(new MockExpirableGuidRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => sut.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async void Throw_ArgumentException_given_invalid_expire()
    {
        //setup
        //create expirable guid domain object and add to repo
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        var user = "Joe, Smith.";
        var expire = DateTimeOffset.UtcNow.AddDays(5);
        var domainObj = ExpirableGuid.Load(guid, user, expire);

        var repository = new MockExpirableGuidRepository();
        await repository.SaveAsync(domainObj);

        var sut = new UpdateExpirableGuidHandler(repository);
        var request = new UpdateExpirableGuidRequest
        {
            Guid = guid,
            User = "test user",
            Expire = "xxx"
        };
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(request, CancellationToken.None));
    }
}