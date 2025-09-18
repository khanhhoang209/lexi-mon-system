namespace LexiMon.Service.ApiResponse;

public class ResponseData<T> : ServiceResponse
{
    public T? Data { get; set; }
}