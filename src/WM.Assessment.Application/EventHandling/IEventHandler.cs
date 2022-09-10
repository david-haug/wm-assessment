using System.Threading.Tasks;
using WM.Assessment.Domain;

namespace WM.Assessment.Application.EventHandling
{
    public interface IEventHandler<in T> where T : IEvent
    {
        Task HandleAsync(T @event);
    }
}