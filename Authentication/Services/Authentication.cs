using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Authentication.Services
{
    public class Authentication
    {
        private readonly IConfiguration configuration;
        public Authentication(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GenerateJWTAuthetication(string userName, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtHeaderParameterNames.Jku, userName),
                new Claim(JwtHeaderParameterNames.Kid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userName)
            };


            claims.Add(new Claim(ClaimTypes.Role, role));
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtSettings:JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires =
                DateTime.Now.AddMinutes(
                    Convert.ToDouble(configuration["JwtSettings:JwtExpireMinutes"]));

            var token = new JwtSecurityToken(
                Convert.ToString(configuration["JwtSettings:JwtIssuer"]),
                Convert.ToString(configuration["JwtSettings:JwtAudience"]),
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:JwtKey"]);
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
                var jku = jwtToken.Claims.First(claim => claim.Type == "jku").Value;
                var userName = jwtToken.Claims.First(claim => claim.Type == "kid").Value;

                return userName;
            }
            catch
            {
                return null;
            }
        }

    }

}