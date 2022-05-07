namespace TSDC.ApiHelper
{
    public class BaseResult<T>
    {
        public string Message { get; set; }

        public T Data { get; set; }

        public int StatusCode { get; set; }

        public int TotalCount { get; set; }

        public bool Status { get; set; }

        public string Token { get; set; }
    }
}
