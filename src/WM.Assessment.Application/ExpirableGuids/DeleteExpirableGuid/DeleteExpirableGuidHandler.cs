using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Domain.ExpirableGuids;

namespace WM.Assessment.Application.ExpirableGuids.DeleteExpirableGuid
{
    public class DeleteExpirableGuidHandler : IRequestHandler<DeleteExpirableGuidRequest>
    {
        private readonly IExpirableGuidRepository _repository;

        public DeleteExpirableGuidHandler(IExpirableGuidRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteExpirableGuidRequest request, CancellationToken cancellationToken)
        {
            var expirableGuid = await _repository.GetAsync(request.Guid);
            if (expirableGuid == null || expirableGuid.IsExpired)
                throw new NotFoundException($"Valid GUID not found for: {request.Guid}");

            expirableGuid.Delete();
            await _repository.DeleteAsync(expirableGuid);
            return await Unit.Task;
        }
    }
}