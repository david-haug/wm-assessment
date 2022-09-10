using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Domain.ExpirableGuids;

namespace WM.Assessment.Application.ExpirableGuids.UpdateExpirableGuid
{
    public class UpdateExpirableGuidHandler : IRequestHandler<UpdateExpirableGuidRequest, ExpirableGuidResponse>
    {
        private readonly IExpirableGuidRepository _repository;

        public UpdateExpirableGuidHandler(IExpirableGuidRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExpirableGuidResponse> Handle(UpdateExpirableGuidRequest request,
            CancellationToken cancellationToken)
        {
            var expirableGuid = await _repository.GetAsync(request.Guid);
            if (expirableGuid == null || expirableGuid.IsExpired)
                throw new NotFoundException($"Valid GUID not found for: {request.Guid}");

            //update expire
            var updatedExpire = expirableGuid.Expire;
            if (!string.IsNullOrWhiteSpace(request.Expire))
            {
                if (long.TryParse(request.Expire, out var seconds))
                    updatedExpire = DateTimeOffset.FromUnixTimeSeconds(seconds);
                else
                    throw new ArgumentException("Expire not valid.");
            }

            expirableGuid.Update(request.User, updatedExpire);
            await _repository.SaveAsync(expirableGuid);

            return new ExpirableGuidResponse
            {
                Guid = expirableGuid.Guid,
                User = expirableGuid.User,
                Expire = expirableGuid.Expire.ToUnixTimeSeconds().ToString()
            };
        }
    }
}