using System.Threading.Tasks;

namespace WM.Assessment.Domain.ExpirableGuids
{
    public interface IExpirableGuidRepository
    {
        Task<ExpirableGuid> GetAsync(string guid);
        Task SaveAsync(ExpirableGuid expirableGuid);
        Task DeleteAsync(ExpirableGuid expirableGuid);
    }
}