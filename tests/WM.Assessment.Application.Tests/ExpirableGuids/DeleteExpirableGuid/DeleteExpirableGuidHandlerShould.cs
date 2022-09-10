using System;
using System.Threading;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Application.ExpirableGuids.DeleteExpirableGuid;
using WM.Assessment.Application.Tests.Fakes;
using WM.Assessment.Domain.ExpirableGuids;
using Xunit;

namespace WM.Assessment.Application.Tests.ExpirableGuids.DeleteExpirableGuid;

public class DeleteExpirableGuidHandlerShould
{
    [Fact]
    public async void Delete_given_valid_guid()
    {
        //setup
        //create expirable guid domain object and add to repo
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        var user = "Joe, Smith.";
        var expire = DateTimeOffset.UtcNow.AddDays(5);
        var domainObj = ExpirableGuid.Load(guid, user, expire);

        var repository = new MockExpirableGuidRepository();
        await repository.SaveAsync(domainObj);

        var sut = new DeleteExpirableGuidHandler(repository);
        await sut.Handle(new DeleteExpirableGuidRequest {Guid = guid}, CancellationToken.None);

        var actual = await repository.GetAsync(guid);
        Assert.Null(actual);
    }

    [Fact]
    public async void Throw_NotFoundException_given_invalid_guid()
    {
        var request = new DeleteExpirableGuidRequest
        {
            Guid = "xxx"
        };
        var sut = new DeleteExpirableGuidHandler(new MockExpirableGuidRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => sut.Handle(request, CancellationToken.None));
    }
}