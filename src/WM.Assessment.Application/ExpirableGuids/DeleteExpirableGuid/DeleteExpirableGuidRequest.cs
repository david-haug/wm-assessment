using MediatR;

namespace WM.Assessment.Application.ExpirableGuids.DeleteExpirableGuid
{
    public class DeleteExpirableGuidRequest : IRequest
    {
        public string Guid { get; set; }
    }
}