using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Domain.ExpirableGuids;

namespace WM.Assessment.Application.ExpirableGuids.GetExpirableGuid
{
    public class GetExpirableGuidHandler : IRequestHandler<GetExpirableGuidRequest, ExpirableGuidResponse>
    {
        private readonly IExpirableGuidRepository _repository;

        public GetExpirableGuidHandler(IExpirableGuidRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExpirableGuidResponse> Handle(GetExpirableGuidRequest request,
            CancellationToken cancellationToken)
        {
            var expirableGuid = await _repository.GetAsync(request.Guid);
            if (expirableGuid == null || expirableGuid.IsExpired)
                throw new NotFoundException($"Valid GUID not found for: {request.Guid}");

            return new ExpirableGuidResponse
            {
                Guid = expirableGuid.Guid,
                User = expirableGuid.User,
                Expire = expirableGuid.Expire.ToUnixTimeSeconds().ToString()
            };
        }
    }
}