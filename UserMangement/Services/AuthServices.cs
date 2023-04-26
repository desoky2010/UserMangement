using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserMangement.Context;
using UserMangement.Helpers;
using UserMangement.Models;
using UserMangement.Models.DTOs;

namespace UserMangement.Services
{
    public class AuthServices : IAuthServices
    {
        #region props
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<JWT> _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailService;
        #endregion
        #region ctor
        public AuthServices(ApplicationContext context,UserManager<ApplicationUser> userManager,IOptions<JWT> jwt,RoleManager<IdentityRole> roleManager,IMailService mailService)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt;
            _roleManager = roleManager;
            _mailService = mailService;
        }

        
        #endregion

        #region public methods
        public async Task<AuthModel> Login(LoginDTO request)
        {
           var user = await _userManager.FindByEmailAsync(request.Email);
            
            if(user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new AuthModel { Message = "User name or password is wrong" };
            }
            if (user.IsDeleted == true)
                return new AuthModel { Message = "You have been Deleted from system " };
            if (user.BlockedAt > DateTime.Now)
                return new AuthModel { Message = $"You have been Blocked till {user.BlockedAt}" };

            if( _userManager.Users.Any(x=>x.VerifiedAt == null)) {
                return new AuthModel { Message = $"You need to verify your email first please check your email : {user.Email}" };
            }
            if(request.OTP == null)
            {
                user.LoginToken = GenerateOTP();
                user.LoginTokenExpire = DateTime.Now.AddMinutes(120);
                await _context.SaveChangesAsync();
                SendOTP(user.Email, user.LoginToken, user.LoginTokenExpire.ToString(), user.UserName);
                return new AuthModel { Message=$"We have sent an OTP TO your Email {user.Email}"};
            }
            if(user.LoginToken != request.OTP || user.LoginTokenExpire < DateTime.Now)
            {
                user.LoginToken = GenerateOTP();
                user.LoginTokenExpire = DateTime.Now.AddMinutes(120);
                await _context.SaveChangesAsync();
                return new AuthModel { Message = $"You have entered an invalid OTP and we sent another one to  {user.Email}" };
            }
            user.LoginToken = null;
            await _context.SaveChangesAsync();
            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            return new AuthModel
            {
                IsAuthnticated =true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName,
                ExpireDate= jwtSecurityToken.ValidTo,
                Roles = roles.ToList(),
                Message= $"Welcome back {user.UserName}"
            };
        }

        public async Task<AuthModel> Register(RegisterDTO request)
        {
            
            if(await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new AuthModel { Message = "Email is already used choose another one " };
            }
            if(await _userManager.FindByNameAsync(request.UserName) != null)
            {
                return new AuthModel { Message = "UserName is already used choose another one " };
            }
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                VerficationEmailToken = CreateRandomToken(32)
            };
           var result =  await _userManager.CreateAsync(user,request.Password);
            if(!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += error + "  ";

                }
                return new AuthModel { Message = errors};
            }
            await _userManager.AddToRoleAsync(user, "User");
            SendVerifiyMails(user.Email , user.VerficationEmailToken, user.UserName);
            var jwtToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Message = " Email created Succesfully",
                Email = user.Email,
                ExpireDate = jwtToken.ValidTo,
                IsAuthnticated = true,
                Roles = new List<string> { "User"},
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                UserName = request.UserName
            };
            
        }

        public async Task<string> AddUserToRoles(AddUsertoRole request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return "User not found";
            if (!await _roleManager.RoleExistsAsync(request.roleName))
                return " role is invalid";
            if (await _userManager.IsInRoleAsync(user, request.roleName))
                return $"{user.UserName} is already have this role {request.roleName}";
            var result = await _userManager.AddToRoleAsync(user, request.roleName);
            if (result.Succeeded)
                return string.Empty;
            return "something went wrong";
        }

        public async Task<string> VerifiyEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.VerficationEmailToken== token);
            if (user == null)
                return "Invalid Token";
            user.VerifiedAt= DateTime.Now;
            await _context.SaveChangesAsync();
            return $"Email {user.Email} Verified successfully ";
        }

        public async Task<string> ResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "invalide email";
            user.PasswordResetToken = CreateRandomToken(64);
            user.ResetTokenExpire= DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();
             await _userManager.RemovePasswordAsync(user);
            SendResetMail(user.Email , user.PasswordResetToken);
            return $"we have sent token to reset your password to your email {user.Email} be carefull this token will expire at {user.ResetTokenExpire}";
        }
        public async Task<string> NewPassword (string token,string password)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x=>x.PasswordResetToken== token);
            if (user == null && user.ResetTokenExpire < DateTime.Now)
                return "invalid or expired token ";
            var result = await _userManager.AddPasswordAsync(user,password);
            user.PasswordResetToken = null;
            user.ResetTokenExpire = null;
            await _context.SaveChangesAsync();
            return "password successfully changed ";
        }

        public async Task<string> AddRole( string rolename)
        {
          if(!await _roleManager.RoleExistsAsync(rolename))
            {
                var role = new IdentityRole(rolename);
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Faild To create role {rolename}");
                }
            }
            return "Role Created Successfully";
        }

        public async Task<string> ChangePassword(ChangePasswordModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return "User not found ";
            var result = await _userManager.ChangePasswordAsync(user,request.OldPassword,request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += error + "  ";

                }
                return errors;
            }
            return "Password changed successfully";
        }

        public async Task<string> DeleteRole(string rolename)
        {
            if(await _roleManager.RoleExistsAsync(rolename))
            {
                var role = new IdentityRole(rolename);
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Faild To Delete role {rolename}");
                }
            }
            return "Faild to delete this Role ";
        }
        public async Task<string> DeleteUser(string username)
        {
            var user =await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new  Exception("user not found");
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return $"{user.UserName} have been Deleted Successfully";

        }

        public async Task<string> BlockUser(string username,byte BlockedDays)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return "User not found ";
           // user.UserBlock = true;
            user.BlockedAt= DateTime.Now.AddDays(BlockedDays);
            await _context.SaveChangesAsync();
            return $"{user.UserName} is blocked till {user.BlockedAt}";
        }
        public async Task<string> UnBlockUser(string username)
        {
           var user = await _userManager.FindByNameAsync(username) ;
            if (user == null)
                return "User not found ";
            user.BlockedAt = DateTime.Now.AddYears(-1000);
            await _context.SaveChangesAsync();
            return $"{user.UserName} has been unblocked successfully";
        }

        public async Task<List<UsersDTO>> GetAllUsers()
        {
            var users = await _context.Users.Where(x=>x.IsDeleted == false).ToListAsync();
            if(users == null )
                return new List<UsersDTO>();
            var list = users.Select(x => new UsersDTO
            {
                Email = x.Email,
                UserName = x.UserName,
                BlockedAt = x.BlockedAt,
                VerifiedAt = x.VerifiedAt
            }).ToList();
            return list;
        }



        #endregion




        #region Private Methods

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));
            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid" , user.Id)
            }.Union(userClaims).Union(roleClaims);
            var SymmetricSecuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key));
            var signinCredenials = new SigningCredentials(SymmetricSecuritykey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Value.Issuer,
                audience: _jwt.Value.Audience,
                claims: Claims,
                expires: DateTime.Now.AddDays(_jwt.Value.DuriationInDays),
                signingCredentials: signinCredenials
                );
            return jwtSecurityToken;
        }
        
        private string CreateRandomToken(byte numsofbytes)
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(numsofbytes));
        }

        private async void SendVerifiyMails(string email , string token , string username)
        {
            var filepath = $"{Directory.GetCurrentDirectory()}\\temps\\VerfiyEmail.html";
            var str = new StreamReader(filepath);
            var mailtext = str.ReadToEnd();
            str.Close();
            mailtext = mailtext.Replace("[User]", username).Replace("[Token]", token);
            await _mailService.SendEmail(email, "Verfication Mail", mailtext);
        }

        private async void SendResetMail(string email, string token)
        {
            var filepath = $"{Directory.GetCurrentDirectory()}\\temps\\ResetPassword.html";
            var str = new StreamReader(filepath);
            var mailtext = str.ReadToEnd();
            str.Close();
            mailtext = mailtext.Replace("{TOKEN}", token);
            await _mailService.SendEmail(email, "Reset Password", mailtext);
        }
        private static string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }
        private async void SendOTP(string email, string OTP,string expireDate,string username)
        {
            var filepath = $"{Directory.GetCurrentDirectory()}\\temps\\LoginToken.html";
            var str = new StreamReader(filepath);
            var mailtext = str.ReadToEnd();
            str.Close();
            mailtext = mailtext.Replace("[OTP]", OTP).
                Replace("[EXPIRATION_TIME]",expireDate).
                Replace("[USER_NAME]",username);
            await _mailService.SendEmail(email, "Verfiy login", mailtext);
        }






        #endregion
    }
}
