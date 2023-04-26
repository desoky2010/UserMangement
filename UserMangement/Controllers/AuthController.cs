using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserMangement.Models.DTOs;
using UserMangement.Services;

namespace UserMangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDTO request)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var result = await _authServices.Register(request);
            if (!result.IsAuthnticated)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authServices.Login(request);
            if (!result.IsAuthnticated)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("add-user-to-role")]
        public async Task<IActionResult> AddToRole(AddUsertoRole request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authServices.AddUserToRoles(request);
            if(!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(request);
        }

        [HttpPost("verifiy-email")]
        public async Task<IActionResult> VerifiyEmail(string token)
        {
            var result = await _authServices.VerifiyEmail(token);
            if (string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var result = await _authServices.ResetPassword(email);
            if (string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("change-password-from-reset")]
        public async Task<IActionResult> ChangePasswordFromReset(ResetPasswordModel request)
        {
            var result = await _authServices.NewPassword(request.Token, request.Password);
            if(string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("create-roles")]
        public async Task<IActionResult> CreateNewRole(string role)
        {
            var result = await _authServices.AddRole(role);
            if(string.IsNullOrEmpty(result) || result.Contains("Error"))
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authServices.ChangePassword(request);
            if (string.IsNullOrEmpty(result) || result.Contains("Error"))
                return BadRequest(result);
            
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("delete-role")]
        public async Task<IActionResult> DeleteRole(string role)
        {
            var result = await _authServices.DeleteRole(role);
            
            if (result.Contains("Faild"))
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var result = await _authServices.DeleteUser(username);
            if(result.Contains("User not found"))
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("block-user")]
        public async Task<IActionResult> BlockUser(string username , byte Days)
        {
            var result = await _authServices.BlockUser(username,Days);
           
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("unblock-user")]
        public async Task<IActionResult> unBlockUser(string username)
        {
            var result = await _authServices.UnBlockUser(username);
            if (result.Contains("User not found"))
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authServices.GetAllUsers();
            return Ok(result);
        }
    }
}
