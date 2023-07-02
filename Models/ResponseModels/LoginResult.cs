using Loza.Models.ResponseModels;

public class LoginResult : OperationsResult
{
    public int statusCode { get; set; }
    public bool isError { get; set; }
    public new tokenResponse Data { get; set; }
    public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
}