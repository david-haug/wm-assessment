using System.Collections.Generic;

namespace WM.Assessment.Domain
{
    public interface IEntity
    {
        IReadOnlyCollection<IEvent> Events { get; }
        void AddEvent(IEvent @event);
        IReadOnlyCollection<IEvent> DequeueEvents();
    }
}