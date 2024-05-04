using BinaryBookshelfServer.Data.Dto;
using BinaryBookshelfServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BinaryBookshelfServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<BinaryBookshelfUser> userManager, JwtHandler jwtHandler)
        : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            BinaryBookshelfUser? user = await userManager.FindByNameAsync(loginRequest.UserName);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Unauthorized(new LoginResult
                {
                    Success = false,
                    Message = "Invalid username or password."
                });
            }
            JwtSecurityToken token = await jwtHandler.GetTokenAsync(user);
            string jwtTokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResult
            {
                Success = true,
                Message = "Login successful",
                Token = jwtTokenString
            });
        }
    }
}
