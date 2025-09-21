namespace Application.Dto
{
    public class ResponseDto<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public static ResponseDto<T> Success(T data, string message = "The Operation was successful") =>
            new() { IsSuccess = true, Message = message, Data = data };

        public static ResponseDto<T> Fail(string message) =>
            new() { IsSuccess = false, Message = message, Data = default };
    }
}