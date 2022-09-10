using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace WM.Assessment.Application.EventHandling.GetEvents
{
    public class GetEventsHandler : IRequestHandler<GetEventsRequest, QueryResult<ApplicationEvent>>
    {
        private readonly IEventRepository _repository;

        public GetEventsHandler(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<ApplicationEvent>> Handle(GetEventsRequest request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetEventsAsync(request);
        }
    }
}