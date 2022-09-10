using System;
using WM.Assessment.Api.Models;

namespace WM.Assessment.Api.Events
{
    public class GetEventsModel : QueryModel
    {
        public string? Name { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}