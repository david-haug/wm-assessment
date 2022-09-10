using System.Collections.Generic;
using Newtonsoft.Json;

namespace WM.Assessment.Api.Models
{
    public class ErrorModel
    {
        public int Status { get; set; }
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ErrorDetailModel> Errors { get; set; }
    }

    public class ErrorDetailModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}