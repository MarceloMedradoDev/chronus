using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.DTOs
{
    public class UserLoginDTO
    {
        public string Registration { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}