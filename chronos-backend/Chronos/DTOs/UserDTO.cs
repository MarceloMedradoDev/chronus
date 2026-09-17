using Chronos.Enum;

namespace Chronos.DTOs
{
    public class UserDTO
    {
        public string? Name { get; set; }
        public string? Registration { get; set; }
        public string? Password { get; set; }
        public UserType? Type { get; set; }
    }

}