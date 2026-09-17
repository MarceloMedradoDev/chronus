using Chronos.Enum;

namespace Chronos.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required byte[] Password { get; set; }
        public int Registration { get; set; }
        public UserType? Type { get; set; }
    }
}