using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Enum;
using Chronos.Models;

namespace Chronos.DTOs
{
    public class UserResponseDTO
    {
        public int Registration { get; set; }
        public string Name { get; set; } = string.Empty;
        public UserType? Type { get; set; }

    }
}