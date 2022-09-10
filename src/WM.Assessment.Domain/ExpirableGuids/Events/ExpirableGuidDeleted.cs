using System;

namespace WM.Assessment.Domain.ExpirableGuids.Events
{
    public class ExpirableGuidDeleted : IEvent
    {
        public ExpirableGuidDeleted(ExpirableGuid expirableGuid)
        {
            Id = System.Guid.NewGuid();
            DateOccurred = DateTimeOffset.UtcNow;
            Guid = expirableGuid.Guid;
        }

        public string Guid { get; set; }
        public Guid Id { get; }
        public DateTimeOffset DateOccurred { get; }
    }
}