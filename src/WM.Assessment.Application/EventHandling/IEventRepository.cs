using System.Threading.Tasks;
using WM.Assessment.Application.EventHandling.GetEvents;

namespace WM.Assessment.Application.EventHandling
{
    public interface IEventRepository
    {
        Task SaveAsync(ApplicationEvent @event);
        Task<QueryResult<ApplicationEvent>> GetEventsAsync(GetEventsRequest request);
    }
}