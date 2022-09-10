using System;
using System.Collections.Generic;
using System.Linq;

namespace WM.Assessment.Domain
{
    public abstract class Entity : IEntity
    {
        //prevent multiple events of the same type by using dictionary
        private readonly IDictionary<Type, IEvent> _events = new Dictionary<Type, IEvent>();

        public void AddEvent(IEvent @event)
        {
            _events[@event.GetType()] = @event;
        }

        public IReadOnlyCollection<IEvent> DequeueEvents()
        {
            var events = _events.Values.ToList();

            _events.Clear();
            return events;
        }

        public IReadOnlyCollection<IEvent> Events => _events.Values.ToList();
    }
}