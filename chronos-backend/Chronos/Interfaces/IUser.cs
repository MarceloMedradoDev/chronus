using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.DTOs;
using Chronos.Models;

namespace Chronos.Interfaces
{
    public interface IUser
    {
        Task<ResponseModel<UserResponseDTO>> UpdateUser(int matricula, UserDTO updatedUser);
        Task<ResponseModel<UserResponseDTO>> SearchUser(string registration);
        Task<ResponseModel<UserToken>> CreateUser(UserDTO userDTO);
        Task<ResponseModel<Dictionary<string, string>>> Login(UserLoginDTO userLoginDTO);
        Task<ResponseModel<UserModel>> DeleteUser(int matricula);
    }

}