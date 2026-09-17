using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.Models
{
    public class ResponseModel<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
