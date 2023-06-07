namespace Loza.Models
{
    public class OperationsResult
    {
        public List<object> Data { get; set; } = new List<object>();
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
