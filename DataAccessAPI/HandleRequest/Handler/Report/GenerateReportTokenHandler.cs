using DataAccessAPI.HandleRequest.Request.Report;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.BusinessEntity.Report;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Repository.KspuDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Report
{
    /// <summary>
    /// GenerateReportTokenHandler
    /// </summary>
    public class GenerateReportTokenHandler : IRequestHandler<RequestGenerateReportToken, string>
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _config;

        private readonly IAppSettingRepository _appSettingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateReportTokenHandler"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="appSettingRepository">The App setting repo.</param>
        /// <exception cref="System.ArgumentNullException">config</exception>
        public GenerateReportTokenHandler(IConfiguration config, IAppSettingRepository appSettingRepository)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<string> Handle(RequestGenerateReportToken request, CancellationToken cancellationToken)
        {
            //AppSettings appsettings = new AppSettings();
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, _config.GetValue<string>("MapTokenGenerateURL")) { Content = TokenRequestContent() };
            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri(_config.GetValue<string>("MapTokenGenerateURL"));

            var t = await httpClient.SendAsync(msg);
            var responseToken = await t.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(responseToken))
            {
                GenerateToken generateToken = new GenerateToken();
                generateToken = JsonConvert.DeserializeObject<GenerateToken>(responseToken);
                return generateToken.token;
            }
            return string.Empty;

            FormUrlEncodedContent TokenRequestContent()
            {
                var parameters = new Dictionary<string, string>
                {
                    {"username", _appSettingRepository.GetAppSettingValue(AppSetting.ArcGisAdminUserName,true).Result},
                    {"password", _appSettingRepository.GetAppSettingValue(AppSetting.ArcGisAdminPassword,true).Result},
                    {"client", "referer" },
                    {"referer", _config.GetValue<string>("MapTokenGenerateURL") },
                    {"expiration", "1440" },
                    {"f", "pjson" },
                };

                var content = new FormUrlEncodedContent(parameters);
                return content;
            }
        }
    }
}
