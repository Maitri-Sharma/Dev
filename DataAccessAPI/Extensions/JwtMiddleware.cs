using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessAPI.Extensions
{
    /// <summary>
    /// JwtMiddleware
    /// </summary>
    public class JwtMiddleware
    {
        /// <summary>
        /// The next
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;


        private readonly IAppSettingRepository _appSettingRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="JwtMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="appSettingRepository">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IAppSettingRepository appSettingRepository)
        {
            _next = next;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));

        }


        /// <summary>
        /// Invokes the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await ValidateTokenAndAttachToContext(context, token);

            await _next(context);
        }

        /// <summary>
        /// Validates the token and attach to context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        private async Task ValidateTokenAndAttachToContext(HttpContext context, string token)
        {
            try
            {
                await Task.Run(() => { });

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettingRepository.GetAppSettingValue(AppSetting.JWTSecret, true).Result);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var UserName = jwtToken.Claims.First(x => x.Type == "UserName").Value;

                context.Items["UserName"] = UserName;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("expired"))
                {
                    throw new Exception("Token Expired");
                }
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
