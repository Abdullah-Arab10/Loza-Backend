namespace Loza.Models.ResponseModels
{
    public class MyResponse
    {
        public List<object> Data { get; set; } = new List<object>();
        
        public ErrorModel Errors { get; set; }

    }
}
