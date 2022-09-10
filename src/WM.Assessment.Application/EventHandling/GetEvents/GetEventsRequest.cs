using System;
using System.Collections.Generic;
using MediatR;

namespace WM.Assessment.Application.EventHandling.GetEvents
{
    public class GetEventsRequest : IRequest<QueryResult<ApplicationEvent>>
    {
        public string Name { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int Limit { get; set; }
        public string StartAfter { get; set; }

        public IEnumerable<SortItem> Sort { get; set; }
        //public AppUser User { get; set; }
    }
}