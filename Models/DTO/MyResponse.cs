namespace LozaApi.Models.DTO
{
    public class MyResponse
    {
        public List<object> Data { get; set; } = new List<object>();
        
        public List<ErrorModel> Errors { get; set; }=new List<ErrorModel>();

    }
}
