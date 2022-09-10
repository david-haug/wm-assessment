namespace WM.Assessment.Api.Models
{
    public class QueryModel
    {
        public int? Limit { get; set; }
        public string? StartAfter { get; set; }
        public string[]? Sort { get; set; }
    }
}