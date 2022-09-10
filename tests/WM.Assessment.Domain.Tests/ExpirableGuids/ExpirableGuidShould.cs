using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WM.Assessment.Domain.ExpirableGuids;
using WM.Assessment.Domain.ExpirableGuids.Events;
using Xunit;

namespace WM.Assessment.Domain.Tests.ExpirableGuids
{
    public class ExpirableGuidShould
    {
        [Fact]
        public void Create_given_valid_arguments()
        {
            //arrange
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";
            var expires = DateTimeOffset.UtcNow;

            //act
            var sut = ExpirableGuid.Create(id, user, expires);

            //Assert
            Assert.Equal(id, sut.Guid);
            Assert.Equal(user, sut.User);
            Assert.Equal(expires, sut.Expire);
        }

        [Theory]
        [ClassData(typeof(InvalidGuidData))]
        public void Create_should_throw_DomainException_when_guid_is_not_valid(string guid)
        {
            var user = "user";
            var expires = DateTimeOffset.UtcNow;

            var ex = Assert.Throws<DomainException>(() => ExpirableGuid.Create(guid, user, expires));
            Assert.StartsWith("Guid is not valid", ex.Message);
        }

        [Fact]
        public void Create_with_guid_when_guid_null()
        {
            var user = "user";
            var sut = ExpirableGuid.Create(user, null);
            Assert.NotEmpty(sut.Guid);
            Assert.True(Guid.TryParse(sut.Guid, out var g));
        }

        [Fact]
        public void Create_with_expires_in_30_days_when_expires_null()
        {
            //arrange
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";

            var now = DateTimeOffset.UtcNow.AddDays(30);
            //act
            var sut = ExpirableGuid.Create(id, user, null);

            //Assert
            Assert.Equal(id, sut.Guid);
            Assert.Equal(user, sut.User);
            Assert.True(now < sut.Expire);
        }

        [Fact]
        public void Load_given_valid_arguments()
        {
            //arrange
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";
            var expires = DateTimeOffset.UtcNow;

            //act
            var sut = ExpirableGuid.Load(id, user, expires);

            //Assert
            Assert.Equal(id, sut.Guid);
            Assert.Equal(user, sut.User);
            Assert.Equal(expires, sut.Expire);
        }

        [Theory]
        [ClassData(typeof(InvalidGuidData))]
        public void Load_should_throw_DomainException_when_guid_is_not_valid(string guid)
        {
            var user = "user";
            var expires = DateTimeOffset.UtcNow;

            var ex = Assert.Throws<DomainException>(() => ExpirableGuid.Load(guid, user, expires));
            Assert.StartsWith("Guid is not valid", ex.Message);
        }

        [Fact]
        public void Have_IsExpired_equals_true_when_expires_less_than_now()
        {
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";
            var expires = DateTimeOffset.UtcNow.AddDays(-1);

            var sut = ExpirableGuid.Load(id, user, expires);

            Assert.True(sut.IsExpired);
        }

        [Fact]
        public void Have_IsExpired_equals_false_when_expires_greater_than_now()
        {
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";
            var expires = DateTimeOffset.UtcNow.AddDays(1);

            var sut = ExpirableGuid.Load(id, user, expires);

            Assert.False(sut.IsExpired);
        }

        [Fact]
        public void Update_user_and_expires()
        {
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var oldUser = "user";
            var oldExpires = DateTimeOffset.UtcNow;

            var sut = ExpirableGuid.Load(id, oldUser, oldExpires);
            var newUser = "newUser";
            var newDate = DateTimeOffset.UtcNow.AddDays(1);
            sut.Update(newUser, newDate);

            Assert.Equal(id, sut.Guid);
            Assert.Equal(newUser, sut.User);
            Assert.Equal(newDate, sut.Expire);
        }

        [Fact]
        public void Add_ExpirableGuidCreated_event_when_created()
        {
            //arrange
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";
            var expires = DateTimeOffset.UtcNow;

            //act
            var sut = ExpirableGuid.Create(id, user, expires);

            var events = sut.DequeueEvents();
            var @event = (ExpirableGuidCreated) events.FirstOrDefault(e => e is ExpirableGuidCreated);
            Assert.NotNull(@event);

            //event has correct data
            Assert.True(@event.Id != Guid.Empty);
            Assert.Equal(id, @event.Guid);
            Assert.Equal(user, @event.User);
            Assert.Equal(expires, @event.Expire);
        }

        [Fact]
        public void Add_ExpirableGuidDeleted_event_when_deleted()
        {
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var user = "user";
            var expires = DateTimeOffset.UtcNow.AddDays(1);

            var sut = ExpirableGuid.Load(id, user, expires);
            sut.Delete();

            var events = sut.DequeueEvents();
            var @event = (ExpirableGuidDeleted) events.FirstOrDefault(e => e is ExpirableGuidDeleted);
            Assert.NotNull(@event);

            //event has correct data
            Assert.True(@event.Id != Guid.Empty);
            Assert.Equal(id, @event.Guid);
        }

        [Fact]
        public void Add_ExpirableGuidUpdated_event_when_updated()
        {
            var id = Guid.NewGuid().ToString("N").ToUpper();
            var oldUser = "user";
            var oldExpires = DateTimeOffset.UtcNow;

            var sut = ExpirableGuid.Load(id, oldUser, oldExpires);
            var newUser = "newUser";
            var newDate = DateTimeOffset.UtcNow.AddDays(1);
            sut.Update(newUser, newDate);

            var events = sut.DequeueEvents();
            var @event = (ExpirableGuidUpdated) events.FirstOrDefault(e => e is ExpirableGuidUpdated);
            Assert.NotNull(@event);

            //event has correct data
            Assert.True(@event.Id != Guid.Empty);
            Assert.Equal(id, @event.Guid);
            Assert.Equal(newUser, @event.User);
            Assert.Equal(newDate, @event.Expire);
        }
    }

    public class InvalidGuidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {""}; //blank
            yield return new object[] {"ABC123456789"}; //less than 32
            yield return new object[] {"1234567890-1234567890-1234567890"}; //32 characters
            yield return new object[] {Guid.NewGuid().ToString()}; //guid with dashes
            yield return new object[] {Guid.NewGuid().ToString().ToLower()}; //guid no dashes, lowercase
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}