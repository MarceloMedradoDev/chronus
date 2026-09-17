using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public int Registration { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string TotalHours { get; set; } = string.Empty;
        public bool IsCompleteDay { get; set; } = true;
        public DateTime Day { get; set; }
    }
}
