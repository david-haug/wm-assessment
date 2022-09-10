using System;

namespace WM.Assessment.Domain.ExpirableGuids.Events
{
    public class ExpirableGuidUpdated : IEvent
    {
        public ExpirableGuidUpdated(ExpirableGuid expirableGuid)
        {
            Id = System.Guid.NewGuid();
            DateOccurred = DateTimeOffset.UtcNow;
            Guid = expirableGuid.Guid;
            Expire = expirableGuid.Expire;
            User = expirableGuid.User;
        }

        public string Guid { get; }
        public DateTimeOffset Expire { get; }
        public string User { get; }

        public Guid Id { get; }
        public DateTimeOffset DateOccurred { get; }
    }
}