using System.Threading.Tasks;
using WM.Assessment.Domain;

namespace WM.Assessment.Application.EventHandling
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<T>(params T[] events) where T : IEvent;
    }
}