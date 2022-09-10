using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using WM.Assessment.Application;
using WM.Assessment.Application.EventHandling;
using WM.Assessment.Application.EventHandling.GetEvents;
using WM.Assessment.Infrastructure.SqlDataAccess.Models;

namespace WM.Assessment.Infrastructure.SqlDataAccess
{
    public class EventRepository : IEventRepository
    {
        private readonly string _connection;

        public EventRepository(string connection)
        {
            _connection = connection;
        }

        public async Task SaveAsync(ApplicationEvent @event)
        {
            var sql = "exec dbo.save_json @id,@data";
            await using (var sqlConnection = new SqlConnection(_connection))
            {
                await sqlConnection.ExecuteAsync(sql, new
                {
                    table = "event",
                    id = @event.Id, data = JsonHelper.Serialize(@event)
                });
            }
        }

        public async Task<QueryResult<ApplicationEvent>> GetEventsAsync(GetEventsRequest request)
        {
            var events = new List<ApplicationEvent>();
            var sql = "exec dbo.get_json_all @startAfter";
            using (var sqlConnection = new SqlConnection(_connection))
            {
                var documents = await sqlConnection.QueryAsync<SqlDocument>(sql, new {startAfter = request.StartAfter});
                foreach (var document in documents)
                {
                    var @event = JsonConvert.DeserializeObject<ApplicationEvent>(document.Data);
                    events.Add(@event);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                events = events.Where(r => r.Name == request.Name).ToList();
            if (request.StartDate.HasValue)
                events = events.Where(r => r.DateOccurred >= request.StartDate.Value).ToList();
            if (request.EndDate.HasValue)
                events = events.Where(r => r.DateOccurred <= request.EndDate.Value).ToList();

            var sorter = new Sorter<ApplicationEvent>();
            events = sorter.Sort(events, request.Sort?.ToArray()).ToList();

            return new QueryResult<ApplicationEvent>
            {
                Items = events.Take(request.Limit), TotalRecords = events.Count, Limit = request.Limit,
                StartAfter = request.StartAfter
            };
        }
    }
}