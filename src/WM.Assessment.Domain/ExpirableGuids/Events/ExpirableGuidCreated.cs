using System;

namespace WM.Assessment.Domain.ExpirableGuids.Events
{
    public class ExpirableGuidCreated : IEvent
    {
        public ExpirableGuidCreated(ExpirableGuid expirableGuid)
        {
            Id = System.Guid.NewGuid();
            DateOccurred = DateTimeOffset.UtcNow;

            Guid = expirableGuid.Guid;
            Expire = expirableGuid.Expire;
            User = expirableGuid.User;
        }

        //only logging properties relevant to the event
        public string Guid { get; }
        public DateTimeOffset Expire { get; }
        public string User { get; }

        public Guid Id { get; }
        public DateTimeOffset DateOccurred { get; }
    }
}