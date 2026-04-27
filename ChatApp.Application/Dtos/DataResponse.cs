using ChatApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos
{
    public class DataResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static DataResponse<T> Success(T data, string message = "")
            => new()
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };

        public static DataResponse<T> Failure(string message = "", List<string>? errors = null)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
    }
}
