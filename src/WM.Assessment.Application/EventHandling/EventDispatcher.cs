using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WM.Assessment.Domain;

namespace WM.Assessment.Application.EventHandling
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IEventRepository _repository;
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IEventRepository repository, IServiceProvider serviceProvider)
        {
            _repository = repository;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<T>(params T[] events) where T : IEvent
        {
            foreach (var @event in events)
            {
                if (@event == null)
                    throw new ArgumentNullException(nameof(@event), "Event cannot be null");

                //var appUser = _serviceProvider.GetService(typeof(AppUser)) as AppUser;
                await _repository.SaveAsync(new ApplicationEvent(@event));

                var eventType = @event.GetType();
                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                var handler = _serviceProvider.GetService(handlerType);

                if (handler == null)
                    continue;

                var method = handler.GetType()
                    .GetRuntimeMethods()
                    .First(x => x.Name.Equals("HandleAsync"));

                await (Task) method.Invoke(handler, new object[] {@event});
            }
        }
    }
}