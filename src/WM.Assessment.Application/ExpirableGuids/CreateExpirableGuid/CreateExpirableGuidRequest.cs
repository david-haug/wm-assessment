using MediatR;

namespace WM.Assessment.Application.ExpirableGuids.CreateExpirableGuid
{
    public class CreateExpirableGuidRequest : IRequest<ExpirableGuidResponse>
    {
        public string? Guid { get; set; }
        public string User { get; set; }
        public string? Expire { get; set; }
    }
}