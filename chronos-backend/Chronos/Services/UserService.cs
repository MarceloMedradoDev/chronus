using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Context;
using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using Chronos.Services.Token;
using Microsoft.EntityFrameworkCore;
using Chronos.Utils;

namespace Chronos.Services
{
    public class UserService : IUser
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwt;
        public UserService(AppDbContext context, IJwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }
        public async Task<ResponseModel<UserResponseDTO>> UpdateUser(int matricula, UserDTO updatedUser)
        {
            var resposta = new ResponseModel<UserResponseDTO>();

            try
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Registration == matricula);
                if (user == null)
                {
                    resposta.Message = "Usuário não encontrado.";
                    resposta.Success = false;
                    return resposta;
                }

                if (!string.IsNullOrWhiteSpace(updatedUser.Name))
                    user.Name = updatedUser.Name;

                if (!string.IsNullOrWhiteSpace(updatedUser.Registration))
                    user.Registration = Int32.Parse(updatedUser.Registration);

                if (updatedUser.Type != null)
                    user.Type = updatedUser.Type;

                if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                    user.Password = HashHelper.ComputeSha256Hash(updatedUser.Password);

                await _context.SaveChangesAsync();

                resposta.Data = new UserResponseDTO
                {
                    Registration = user.Registration,
                    Name = user.Name,
                    Type = user.Type
                };
                resposta.Message = "Usuário atualizado com sucesso.";
                resposta.Success = true;
            }
            catch (Exception ex)
            {
                resposta.Message = ex.Message;
                resposta.Success = false;
            }

            return resposta;
        }


        public async Task<ResponseModel<UserResponseDTO>> SearchUser(string registration)
        {
            var resposta = new ResponseModel<UserResponseDTO>();
            try
            {
                var User = await _context.User.FirstOrDefaultAsync(x => x.Registration == Int32.Parse(registration));

                if (User == null)
                {
                    resposta.Message = "Nenhum registro localizado";
                    resposta.Success = false;
                    return resposta;
                }

                resposta.Data = new UserResponseDTO()
                {
                    Name = User.Name,
                    Registration = User.Registration,
                    Type = User.Type
                };
                resposta.Message = "Usuário exibido com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<UserToken>> CreateUser(UserDTO userDTO)
        {
            var resposta = new ResponseModel<UserToken>();

            try
            {
                if (!int.TryParse(userDTO.Registration, out int registration))
                {
                    resposta.Success = false;
                    resposta.Message = "Matrícula inválida";
                    return resposta;
                }

                if (string.IsNullOrWhiteSpace(userDTO.Password))
                {
                    resposta.Success = false;
                    resposta.Message = "Senha obrigatória";
                    return resposta;
                }

                var user = await _context.User
                    .FirstOrDefaultAsync(x => x.Registration == registration);

                if (user != null)
                {
                    resposta.Success = false;
                    resposta.Message = "Usuário já cadastrado";
                    return resposta;
                }

                var userEntity = new UserModel()
                {
                    Name = userDTO.Name ?? string.Empty,
                    Registration = registration,
                    Password = HashHelper.ComputeSha256Hash(userDTO.Password),
                    Type = userDTO.Type
                };

                _context.User.Add(userEntity);
                await _context.SaveChangesAsync();

                var token = _jwt.GenerateToken(userEntity);

                resposta.Data = new UserToken
                {
                    Token = token
                };

                resposta.Message = "Usuário criado com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                resposta.Message = e.InnerException?.Message ?? e.Message;
                resposta.Success = false;
            }

            return resposta;
        }


        public async Task<ResponseModel<Dictionary<string, string>>> Login(UserLoginDTO UserLoginDTO)
        {
            var resposta = new ResponseModel<Dictionary<string, string>>();

            try
            {
                var User = await _context.User.FirstOrDefaultAsync(x => x.Registration == Int32.Parse(UserLoginDTO.Registration));

                if (User == null)
                {
                    resposta.Success = false;
                    resposta.Message = "Usuário não existe";
                    return resposta;
                }

                var inputPasswordHash = HashHelper.ComputeSha256Hash(UserLoginDTO.Password);
                if (!User.Password.SequenceEqual(inputPasswordHash))
                {
                    resposta.Success = false;
                    resposta.Message = "Senha incorreta";
                    return resposta;
                }

                var token = _jwt.GenerateToken(User);

                Dictionary<string, string> userLoginResponse = new Dictionary<string, string>
                {
                    { "Id", User.Id.ToString() },
                    { "Token", token },
                    { "Type", User.Type.ToString() }
                };

                resposta.Data = userLoginResponse;
                resposta.Message = "Login realizado com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<UserModel>> DeleteUser(int matricula)
        {
            var resposta = new ResponseModel<UserModel>();

            try
            {
                var User = await _context.User.FirstOrDefaultAsync(x => x.Registration == matricula);

                if (User == null)
                {
                    resposta.Message = "Usuário não encontrado";
                    resposta.Success = false;
                    return resposta;
                }

                _context.User.Remove(User);
                await _context.SaveChangesAsync();

                resposta.Message = "Usuário excluído com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }
    }
}