using MediatR;

namespace WM.Assessment.Application.ExpirableGuids.GetExpirableGuid
{
    public class GetExpirableGuidRequest : IRequest<ExpirableGuidResponse>
    {
        public string Guid { get; set; }
    }
}