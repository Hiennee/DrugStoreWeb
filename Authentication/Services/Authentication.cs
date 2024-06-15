using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Identity;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Services
{
    public class Authentication
    {
        private readonly IConfiguration configuration;
        private readonly DrugStore_AuthenticationContext _context;
        private const double JWTExpireMinutes = 60;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Authentication(IConfiguration configuration, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor,
            DrugStore_AuthenticationContext context)
        {
            this.configuration = configuration;
            this._userManager = userManager;
            this._httpContextAccessor = httpContextAccessor;
            this._context = context;
        }
        public string GenerateJWTAuthentication()
        {
            var user = _httpContextAccessor.HttpContext.User;
            //var _user = _userManager.GetUserAsync(_userPrincipal);
            //_context.
            //gg
            var DateIssued = DateTime.Now;
            //var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:JwtKey"]);
            var claims = new List<Claim>
            {
                //_context.AspNetRoles.Where()
                new Claim(ClaimTypes.Name, user.Identity.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Identity.Name),
                //new Claim(ClaimTypes.Role, )
            };
            //SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor{
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.UtcNow.AddMinutes(JWTExpireMinutes),
            //    SigningCredentials = new SigningCredentials(algorithm: SecurityAlgorithms.HmacSha256, key: new SymmetricSecurityKey(key))
            //};
            //claims.Add(new Claim(ClaimTypes.Role, role));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("UHJTFRTYUY787FVGHMJYAERvlkuytnbf"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateIssued.AddMinutes(15);

            var token = new JwtSecurityToken(
                "Chan Hien",
                "Somebody",
                claims,
                expires: expires,
                signingCredentials: creds
            );
            _context.AspNetUserTokens.Add(new AspNetUserToken
            {
                UserId = _userManager.GetUserId(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                DateIssued = DateIssued,
            }); ;
            _context.SaveChanges();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("UHJTFRTYUY787FVGHMJYAERvlkuytnbf");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                var jwtToken = (JwtSecurityToken)validatedToken;
                var jku = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                var userName = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

                return userName;
            }
            catch
            {
                return null;
            }
        }

    }

}