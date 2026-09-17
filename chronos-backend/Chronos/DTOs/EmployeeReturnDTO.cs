using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.DTOs
{
    public class EmployeeReturnDTO
    {
        public string Role { get; set; } = string.Empty;
        public string TotalHours { get; set; } = string.Empty;
        public bool IsCompleteDay { get; set; }
    }
}
