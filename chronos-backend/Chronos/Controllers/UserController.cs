using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Chronos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        public UserController(IUser user)
        {
            _user = user;
        }

        [HttpGet("get")]
        [EnableRateLimiting("GlobalLimiter")]
        public async Task<ActionResult<ResponseModel<UserController>>> SearchUser(string registration)
        {
            var user = await _user.SearchUser(registration);
            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ResponseModel<UserController>>> CreateUser(UserDTO userDTO)
        {
            var user = await _user.CreateUser(userDTO);
            return Ok(user);
        }

        [HttpPost("update")]
        public async Task<ActionResult<ResponseModel<UserController>>> UpDateUser(int id, UserDTO userObject)
        {
            var user = await _user.UpdateUser(id, userObject);
            return Ok(user);
        }

        [HttpPost("login")]
        [EnableRateLimiting("LoginLimiter")]
        public async Task<ActionResult<ResponseModel<UserController>>> Login(UserLoginDTO userDto)
        {
            var login = await _user.Login(userDto);
            return Ok(login);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<ResponseModel<UserController>>> Delete(int id)
        {
            var delete = await _user.DeleteUser(id);
            return Ok(delete);
        }

    }
}