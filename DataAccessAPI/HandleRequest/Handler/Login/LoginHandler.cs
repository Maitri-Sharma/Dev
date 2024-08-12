using DataAccessAPI.HandleRequest.Request.Login;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.OEBSService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Login
{
    /// <summary>
    /// LoginHandler
    /// </summary>
    public class LoginHandler : IRequestHandler<RequestLogin, string>
    {
        /// <summary>
        /// App Setting Repository
        /// </summary>
        private readonly IAppSettingRepository _appSettingRepository;

        /// <summary>
        /// OEBS Repository
        /// </summary>
        private readonly IOEBSServiceRepository _oEBSServiceRepository;

        RequestLogin requestLogin;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandler"/> class.
        /// </summary>
        /// <param name="appSettingRepository">The configuration.</param>
        /// <param name="oEBSServiceRepository">The OEBS Repo.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public LoginHandler(IAppSettingRepository appSettingRepository, IOEBSServiceRepository oEBSServiceRepository)
        {
            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));
            _oEBSServiceRepository = oEBSServiceRepository ?? throw new ArgumentNullException(nameof(oEBSServiceRepository));

        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<string> Handle(RequestLogin request, CancellationToken cancellationToken)
        {
            requestLogin = request;

            if (request.IsFromKundeWeb)
                await ValidateOEBSUser();

            //TODO : Post production need to test azure ad authentication for that uncomment below code.
            //From UI they need to pass idToken of azure ad
            //else
            //    await ValidateAZureAD(request.Token);
            // generate token that is valid for 1 day
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettingRepository.GetAppSettingValue(AppSetting.JWTSecret, true).Result);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("UserName", request.UserName) }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validate OEBS user for kundeweb
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ValidateOEBSUser()
        {
            _ = await _oEBSServiceRepository.AgreementLookup389(new Puma.DataLayer.BusinessEntity.EC_Data.AgreementLookup()
            {
                BrukerNavn = requestLogin.UserName,
                Key = requestLogin.Token,
                Header = new Puma.DataLayer.BusinessEntity.EC_Data.Header()
            });

            return true;
        }

        /// <summary>
        /// Validate azure ad token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAZureAD(string token)
        {
            try
            {
                // TODO : Get tenant and audiance from azure key vault(need to add new secret in azure)
                string myTenant = "a1ae5425-0bde-496e-8c5a-8a06b0d94277";
                var myAudience = "d3a8c3e7-3bfc-435c-8eb2-daf6a8b412b7";
                var myIssuer = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/v2.0", myTenant);
                var stsDiscoveryEndpoint = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/.well-known/openid-configuration", myTenant);
                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
                var config = await configManager.GetConfigurationAsync();

                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = myAudience,
                    ValidIssuer = myIssuer,
                    IssuerSigningKeys = config.SigningKeys,
                    ValidateLifetime = true,

                };

                var validatedToken = (SecurityToken)new JwtSecurityToken();
                // Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)  
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                #region Commented code of checking request user name with user name we get from claim
                ////Just added one more check that user name we have in request and that we claimed from azure ad token is same.
                //if (validatedToken != null && ((System.IdentityModel.Tokens.Jwt.JwtSecurityToken)validatedToken).Claims?.Any() == true)
                //{
                //    string userNameClaim = ((System.IdentityModel.Tokens.Jwt.JwtSecurityToken)validatedToken).Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value;
                //    if (!string.IsNullOrWhiteSpace(userNameClaim) && userNameClaim.ToLower() == userName.ToLower())
                //        return true;

                //} 
                #endregion

            }
            catch (Exception)
            {
                throw new Exception("Invalid Credentials");
            }
            return false;
        }
    }
}
