using Jarvis.Service.Abstract;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Jarvis.Service.Concrete
{
    public class JwtHandler: IJwtHandler
    {
        
        public bool ValidateToken(string _publicKey,string token)
        {

            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_publicKey), out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://app.dev.conduit.com/api",
                ValidAudience = "https://app.dev.conduit.com",
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
