using System;

namespace WM.Assessment.Infrastructure.SqlDataAccess.Models
{
    public class ExpirableGuidDto
    {
        public string Guid { get; set; }
        public string User { get; set; }
        public DateTimeOffset Expire { get; set; }
    }
}