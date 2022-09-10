using System;
using System.Threading;
using System.Threading.Tasks;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Application.ExpirableGuids.CreateExpirableGuid;
using WM.Assessment.Application.Tests.Fakes;
using WM.Assessment.Domain;
using WM.Assessment.Domain.ExpirableGuids;
using Xunit;

namespace WM.Assessment.Application.Tests.ExpirableGuids.CreateExpirableGuid;

public class CreateExpirableGuidHandlerShould
{
    [Fact]
    public async void Return_ExpirableGuidResponse_given_valid_request()
    {
        var request = new CreateExpirableGuidRequest
        {
            Guid = Guid.NewGuid().ToString("N").ToUpper(),
            User = "test user",
            Expire = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString()
        };
        var sut = new CreateExpirableGuidHandler(new MockExpirableGuidRepository());
        var actual = await sut.Handle(request, CancellationToken.None);

        Assert.Equal(request.Guid, actual.Guid);
        Assert.Equal(request.User, actual.User);
        Assert.Equal(request.Expire, actual.Expire);
    }

    [Fact]
    public async void Return_ExpirableGuidResponse_with_expire_in_30_days_when_not_given()
    {
        var request = new CreateExpirableGuidRequest
        {
            Guid = Guid.NewGuid().ToString("N").ToUpper(),
            User = "test user"
        };

        //should be 30 days
        var expectedExpire = DateTimeOffset.UtcNow.AddDays(30);

        var sut = new CreateExpirableGuidHandler(new MockExpirableGuidRepository());
        var actual = await sut.Handle(request, CancellationToken.None);
        Assert.True(expectedExpire.ToUnixTimeSeconds() <= Convert.ToInt64(actual.Expire));
    }

    [Fact]
    public async void Return_ExpirableGuidResponse_with_guid_when_not_given()
    {
        var request = new CreateExpirableGuidRequest
        {
            User = "test user"
        };

        var sut = new CreateExpirableGuidHandler(new MockExpirableGuidRepository());
        var actual = await sut.Handle(request, CancellationToken.None);
        Assert.NotEmpty(actual.Guid);
    }

    [Fact]
    public async Task Throw_Exception_given_invalid_guid()
    {
        var sut = new CreateExpirableGuidHandler(new MockExpirableGuidRepository());
        var request = new CreateExpirableGuidRequest
        {
            Guid = "abc",
            User = "test user"
        };
        await Assert.ThrowsAsync<DomainException>(() => sut.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Throw_BadRequestException_given_existing_guid()
    {
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        var user = "Joe, Smith.";
        var expire = DateTimeOffset.UtcNow.AddDays(5);
        var domainObj = ExpirableGuid.Load(guid, user, expire);

        var repository = new MockExpirableGuidRepository();
        await repository.SaveAsync(domainObj);
        var sut = new CreateExpirableGuidHandler(repository);
        var request = new CreateExpirableGuidRequest
        {
            Guid = guid,
            User = "test user"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Throw_BadRequestException_given_invalid_expire()
    {
        var sut = new CreateExpirableGuidHandler(new MockExpirableGuidRepository());
        var request = new CreateExpirableGuidRequest
        {
            User = "test user",
            Expire = "xxx"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.Handle(request, CancellationToken.None));
    }
}