﻿using Core.Common.Model.Transaccion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Core.Common.Util.Helper.Autenticacion
{
    public static class JwtHelper
    {
        /// <summary>
        /// Almacena la clave secreta definida en la configuración de la APP
        /// </summary>
        private static string SecretKey { get; set; }

        /// <summary>
        /// Define tiempo de duración de token
        /// </summary>
        private static int ExpireMinutes { get; set; }

        /// <summary>
        /// Metodo para Setear Configuración del servicio de JWT
        /// </summary>
        /// <param name="builder"></param>
        public static void ConfigurarServicioJWT(WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json");
            var secretKey = builder.Configuration.GetSection("JWT").GetSection("key").Value.ToString();
            var expireMinutes = builder.Configuration.GetSection("JWT").GetSection("DurationInMinutes").Value;
            ExpireMinutes = Convert.ToInt32(expireMinutes);
            SecretKey = secretKey;
            var keybytes = Encoding.UTF8.GetBytes(secretKey);

            builder.Services.AddAuthentication(c =>
            {
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(c =>
            {
                c.RequireHttpsMetadata = false;
                c.SaveToken = true;
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keybytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        /// <summary>
        /// Metodo para Generar un JWT
        /// </summary>
        /// <param name="claimsList">Lista de claim para crear Token
        /// Como el nombre, correo, contraseña
        /// Recibe <ClaimTypes, Value><ClaimTypes, Value>
        /// </param>
        /// <returns>Token</returns>
        public static string GenerarJWT(Dictionary<string, string> claimsList)
        {
            var keybytes = Encoding.ASCII.GetBytes(SecretKey);
            var claims = new ClaimsIdentity();

            foreach (var claim in claimsList)
            {
                claims.AddClaim(new Claim(claim.Key, claim.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(ExpireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keybytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            var tokenReturnable = tokenHandler.WriteToken(tokenConfig);
            return tokenReturnable;
        }

        /// <summary>
        /// Metodo para desencriptar jwt
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static JwtSecurityToken DesencriptarJWT(HttpRequest request)
        {
            var token = request.Headers["Authorization"].ToString().Split(' ')[1];

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            return jwtSecurityToken;
        }

        /// <summary>
        /// Metodo para desencriptar jwt
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static JwtSecurityToken DesencriptarJWT(string tokenEncriptado)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(tokenEncriptado);

            return jwtSecurityToken;
        }

        /// <summary>
        /// Obtiene Claims para obtener datos de JWT
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static string GetClaim(JwtSecurityToken securityToken, string claim)
        {
            var jti = securityToken.Claims.First(c => c.Type == claim).Value;
            return jti;
        }

        public static void CheckJWT(HttpRequest request, TransaccionBase transaccion)
        {
            if (!string.IsNullOrEmpty(request.Headers["Authorization"].ToString()))
            {
                var token = DesencriptarJWT(request);
                transaccion.Credenciales = new Model.General.Credenciales
                {
                    Usuario = GetClaim(token, "Usuario"),
                    Codigo = GetClaim(token, "Entidad"),
                    Clave = GetClaim(token, "Password"),
                    Tipo = GetClaim(token, "Tipo")
                };
            }//crendeical nulo
        }
    }
}
