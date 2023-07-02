namespace Loza.Models.ResponseModels
{
    public class OperationsResult
    {
        public int statusCode { get; set; }
        public bool isError { get; set; }
        public object Data { get; set; } 
        public ErrorModel Errors { get; set; } = new ErrorModel();
    }
}
