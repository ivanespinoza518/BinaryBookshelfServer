using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using BinaryBookshelfServer.Data.Models;

namespace BinaryBookshelfServer
{
    public class JwtHandler(IConfiguration configuration, UserManager<BinaryBookshelfUser> userManager)
    {
        // Generates the token.
        public async Task<JwtSecurityToken> GetTokenAsync(BinaryBookshelfUser user) =>
            new(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: await GetClaimsAsync(user),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpirationTimeInMinutes"])),
                signingCredentials: GetSigningCredentials()
                );

        // Token encryption and encoding. Token needs to be encrypted both ways.
        private SigningCredentials GetSigningCredentials()
        {
            byte[] key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecurityKey"]!);
            SymmetricSecurityKey secret = new(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256); // Two way encryption
        }

        private async Task<List<Claim>> GetClaimsAsync(BinaryBookshelfUser user)
        {
            List<Claim> claims = [new Claim(ClaimTypes.Name, user.UserName!)];
            claims.AddRange(from role in await userManager.GetRolesAsync(user) select new Claim(ClaimTypes.Role, role));
            return claims;
        }
    }
}
