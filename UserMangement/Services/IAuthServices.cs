using Microsoft.AspNetCore.Identity;
using UserMangement.Models;
using UserMangement.Models.DTOs;

namespace UserMangement.Services
{
    public interface IAuthServices
    {
        Task<AuthModel> Register(RegisterDTO request);
        Task<AuthModel> Login(LoginDTO request);
        Task<string> AddUserToRoles(AddUsertoRole request);
        Task<string> VerifiyEmail(string token);
        Task<string> ResetPassword(string email);
        Task<string> NewPassword(string token,string password);
        Task<string> AddRole( string role);
        Task<string> ChangePassword(ChangePasswordModel request);
        Task<string> DeleteRole(string role);
        Task<string> DeleteUser(string username);
        Task<string> BlockUser(string username , byte blockedDays);
        Task<string> UnBlockUser(string username);
        Task<List<UsersDTO>> GetAllUsers();
    }
}
