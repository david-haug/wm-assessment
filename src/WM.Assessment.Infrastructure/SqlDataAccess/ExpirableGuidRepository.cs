using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WM.Assessment.Application.EventHandling;
using WM.Assessment.Domain.ExpirableGuids;
using WM.Assessment.Infrastructure.SqlDataAccess.Models;

namespace WM.Assessment.Infrastructure.SqlDataAccess
{
    public class ExpirableGuidRepository : IExpirableGuidRepository
    {
        private readonly string _connection;
        private readonly IEventDispatcher _eventDispatcher;

        public ExpirableGuidRepository(string connection, IEventDispatcher eventDispatcher)
        {
            _connection = connection;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<ExpirableGuid> GetAsync(string guid)
        {
            ExpirableGuid expirableGuid = null;
            var sql = "exec guid_get_by_id @guid";
            await using (var sqlConnection = new SqlConnection(_connection))
            {
                var dto = await sqlConnection.QueryFirstOrDefaultAsync<ExpirableGuidDto>(sql, new {guid});
                if (dto != null)
                    expirableGuid = ExpirableGuid.Load(dto.Guid, dto.User, dto.Expire);
            }

            return expirableGuid;
        }

        public async Task SaveAsync(ExpirableGuid expirableGuid)
        {
            var sql = "exec guid_save @guid, @user, @expire";

            await using (var sqlConnection = new SqlConnection(_connection))
            {
                await sqlConnection.ExecuteAsync(sql, new
                {
                    expirableGuid.Guid,
                    expirableGuid.User,
                    expirableGuid.Expire
                });
            }

            await _eventDispatcher.DispatchAsync(expirableGuid.DequeueEvents().ToArray());
        }


        public async Task DeleteAsync(ExpirableGuid expirableGuid)
        {
            var sql = "exec guid_delete @guid";
            await using (var sqlConnection = new SqlConnection(_connection))
            {
                await sqlConnection.ExecuteAsync(sql, new {guid = expirableGuid.Guid});
            }

            await _eventDispatcher.DispatchAsync(expirableGuid.DequeueEvents().ToArray());
        }
    }
}