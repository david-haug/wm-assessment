using System;
using WM.Assessment.Domain;

namespace WM.Assessment.Application.EventHandling
{
    public class ApplicationEvent
    {
        public ApplicationEvent()
        {
        }

        public ApplicationEvent(IEvent @event)
        {
            Id = @event.Id.ToString();
            DateOccurred = @event.DateOccurred;
            Name = @event.GetType().Name;
            Event = @event;
            //User = appUser;
        }

        public string Id { get; set; }
        public DateTimeOffset DateOccurred { get; set; }
        public string Name { get; set; }

        public object Event { get; set; }
        //public AppUser User { get; set; }
    }
}