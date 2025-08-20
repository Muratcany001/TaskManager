using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM.BLL.ViewModels
{
    public class ResultViewModel<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }

        public static ResultViewModel<T> Success(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new ResultViewModel<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = statusCode,
                Errors = null
            };
        }

        public static ResultViewModel<T> Failure(string message, List<string> errors = null, int statusCode = 200)
        {
            return new ResultViewModel<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }
        public static ResultViewModel<T> NotFound(string message = "Resource not found", int statusCode = 200)
        {
            return new ResultViewModel<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default,
                StatusCode = statusCode,
                Errors = new List<string> { "The requested resource was not found." }
            };
        }

    }
}
