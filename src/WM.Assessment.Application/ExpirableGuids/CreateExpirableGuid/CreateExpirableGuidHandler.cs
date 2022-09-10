using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Domain.ExpirableGuids;

namespace WM.Assessment.Application.ExpirableGuids.CreateExpirableGuid
{
    public class CreateExpirableGuidHandler : IRequestHandler<CreateExpirableGuidRequest, ExpirableGuidResponse>
    {
        private readonly IExpirableGuidRepository _repository;

        public CreateExpirableGuidHandler(IExpirableGuidRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExpirableGuidResponse> Handle(CreateExpirableGuidRequest request,
            CancellationToken cancellationToken)
        {
            //error if guid already exists
            var existingGuid = await _repository.GetAsync(request.Guid);
            if (existingGuid != null)
                throw new BadRequestException($"GUID exists for {request.Guid}");

            DateTimeOffset? expires = null;
            //convert request Expire to DateTimeOffset if value present
            if (!string.IsNullOrWhiteSpace(request.Expire))
            {
                if (long.TryParse(request.Expire, out var seconds))
                    expires = DateTimeOffset.FromUnixTimeSeconds(seconds);
                else
                    throw new BadRequestException("Expire not valid.");
            }

            //create domain object
            var expirableGuid = string.IsNullOrWhiteSpace(request.Guid)
                ? ExpirableGuid.Create(request.User, expires)
                : ExpirableGuid.Create(request.Guid, request.User, expires);

            //save domain object
            await _repository.SaveAsync(expirableGuid);

            //return response
            return new ExpirableGuidResponse
            {
                Guid = expirableGuid.Guid,
                User = expirableGuid.User,
                Expire = expirableGuid.Expire.ToUnixTimeSeconds().ToString()
            };
        }
    }
}