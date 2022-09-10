using MediatR;

namespace WM.Assessment.Application.ExpirableGuids.UpdateExpirableGuid
{
    public class UpdateExpirableGuidRequest : IRequest<ExpirableGuidResponse>
    {
        public string Guid { get; set; }
        public string User { get; set; }
        public string Expire { get; set; }
    }
}