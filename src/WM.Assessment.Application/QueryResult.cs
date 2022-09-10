using System.Collections.Generic;

namespace WM.Assessment.Application
{
    public class QueryResult<T>
    {
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        ///     Total number of records found by query regardless of limit
        /// </summary>
        public int TotalRecords { get; set; }

        public int Limit { get; set; }
        public string StartAfter { get; set; }
    }
}