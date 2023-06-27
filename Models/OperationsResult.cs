namespace Loza.Models
{
    public class OperationsResult
    {
        public int statusCode { get; set; }
        public bool isError { get; set; }
        public List<object> Data { get; set; } = new List<object>();
        public ErrorModel Errors { get; set; } = new ErrorModel();
    }
}
