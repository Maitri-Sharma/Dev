using Puma.Infrastructure.Interface.KsupDB.OEBSService;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using Flurl.Http;
using Puma.DataLayer.BusinessEntity.EC_Data;
using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static Puma.Shared.PumaEnum;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Net;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.Logger;
using System.Net.Http.Headers;
using Puma.Infrastructure.Interface.MemoryCache;
using Puma.CustomException;

namespace Puma.Infrastructure.Repository.KspuDB.OEBSService
{
    /// <summary>
    /// OEBSServiceRepository
    /// </summary>
    /// <seealso cref="Puma.Infrastructure.Interface.KsupDB.OEBSService.IOEBSServiceRepository" />
    public class OEBSServiceRepository : IOEBSServiceRepository
    {

        private readonly IConfiguration _configuration;
        private readonly string OEBSUrl;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly IAppSettingRepository _appSettingRepository;
        private readonly ILogger<OEBSServiceRepository> _logger;

        private readonly ILoggerRepository _loggerRepository;

        private readonly IManageCache _manageCache;
        public OEBSServiceRepository(IConfiguration configuration, ILogger<OEBSServiceRepository> logger, IAppSettingRepository appSettingRepository,
            ILoggerRepository loggerRepository, IManageCache manageCache)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));

            _loggerRepository = loggerRepository ?? throw new ArgumentNullException(nameof(loggerRepository));

            OEBSUrl = _appSettingRepository.GetAppSettingValue(AppSetting.OEBSUrl).Result;
            //clientId = _configuration.GetSection("Client_Id").Value;
            //clientSecret = _configuration.GetSection("Client_Secret").Value;

            clientId = _appSettingRepository.GetAppSettingValue(AppSetting.ClientId, true).Result;
            clientSecret = _appSettingRepository.GetAppSettingValue(AppSetting.ClientSecret, true).Result;

            _manageCache = manageCache;

        }

        /// <summary>
        /// Gets the order status.
        /// </summary>
        public async Task<List<OrdreStatusDataEntity>> GetOrderStatus()
        {

            #region Uncomment this code if want  to  test static data
            //string text = File.ReadAllText(@"C:\Smit\Source\Dev\Puma.Infrastructure\Repository\KspuDB\OEBSService\ResponseFile.txt");
            ////text = text.Replace("\"", "'");
            //List<string> lstOrders = JsonConvert.DeserializeObject<List<string>>(text);
            #endregion

            try
            {
                string token = await GetToken();

                _logger.LogDebug("Token Generated Sucessfully for 383 service");

                // _ = _loggerRepository.LogDebug("Token Generated Sucessfully for 383 service");


                string finalURL = OEBSUrl + "PUMA/383";

                var lstOrders = await finalURL.WithOAuthBearerToken(token)
                    .GetJsonAsync<List<string>>();

                _logger.LogDebug("Succesfully get data from 383 service. :" + JsonConvert.SerializeObject(lstOrders));


                //await GetToken();

                #region Actuall Code that needs to check
                //    Root s = new Root()
                //    {
                //        MyArray = new List<string>() {
                //@"<?xml version='1.0' encoding='UTF-8'?>
                //    <tns:OrdreStatusRequest xmlns:q1='htt://Posten.KSPU.UtvalgService.DataContracts/2007/05' xmlns:dateformat='no.ergo.ec.ordre.util.DateFormatter' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:tns='http://Posten.KSPU.UtvalgService.ServiceContracts/2007/05' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                //      <tns:OrdreStatus>
                //        <q1:Id>30261</q1:Id>
                //        <q1:Type>Liste</q1:Type>
                //        <q1:Kildesystem>KSPU</q1:Kildesystem>
                //        <q1:OEBSType>T</q1:OEBSType>
                //        <q1:OEBSRef>8926710</q1:OEBSRef>
                //        <q1:Status>R</q1:Status>
                //        <q1:Kommentar/>
                //        <q1:Innleveringsdato>2021-10-11T00:00:00</q1:Innleveringsdato>
                //        <q1:EndretAv>224067</q1:EndretAv>
                //        <q1:ReturnerFordeling>false</q1:ReturnerFordeling>
                //        <q1:DistribusjonsDato>2021-10-18T00:00:00</q1:DistribusjonsDato>
                //        <q1:OmdelingsType>2</q1:OmdelingsType>
                //      </tns:OrdreStatus>
                //    </tns:OrdreStatusRequest>"
                //        }
                //    };

                //    string data = JsonConvert.SerializeObject(s);
                //    if (data != null)
                //    {

                //    } 
                #endregion

                List<OrdreStatusDataEntity> response = new List<OrdreStatusDataEntity>();

                foreach (var orders in lstOrders)
                {
                    StringReader sr = new StringReader(orders.Trim());
                    DataSet ds = new DataSet();
                    ds.ReadXml(sr);
                    if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow rowData in ds.Tables[1].Rows)
                        {
                            OrdreStatusDataEntity ordreStatusDataEntity = new OrdreStatusDataEntity()
                            {
                                DistribusjonsDato = Convert.ToDateTime(rowData["DistribusjonsDato"]),
                                EndretAv = Convert.ToString(rowData["EndretAv"]),
                                Id = Convert.ToInt32(rowData["Id"]),
                                Innleveringsdato = Convert.ToDateTime(rowData["Innleveringsdato"]),
                                Kildesystem = Convert.ToString(rowData["Kildesystem"]),
                                Kommentar = Convert.ToString(rowData["Kommentar"]),
                                OEBSRef = Convert.ToString(rowData["OEBSRef"]),
                                OEBSType = Convert.ToString(rowData["OEBSType"]),
                                OmdelingsType = Convert.ToString(rowData["OmdelingsType"]),
                                ReturnerFordeling = Convert.ToBoolean(rowData["ReturnerFordeling"]),
                                SourceType = Convert.ToString(rowData["Type"]),
                                Status = Convert.ToString(rowData["Status"]),
                                Type = (UtvalgsTypeKode)Enum.Parse(typeof(UtvalgsTypeKode), Convert.ToString(rowData["Type"]), true)
                            };
                            response.Add(ordreStatusDataEntity);

                            _ = _loggerRepository.LogDebug("Data fetched from OEBS for Id" + ordreStatusDataEntity.Id + " - " + JsonConvert.SerializeObject(ordreStatusDataEntity));

                        }



                    }
                }

                _logger.LogDebug("Return data to process :" + JsonConvert.SerializeObject(response));


                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError("Error in 383 service : " + ex.Message, ex);
                _ = _loggerRepository.LogError("Error in 383 service : " + ex.Message, ex);

                throw;
            }
        }

        /// <summary>
        /// Posts the selection distribution.
        /// </summary>
        public async Task PostSelectionDistribution(OrderStatusResponseEntity orderStatusRequestEntity)
        {
            try
            {
                string token = await GetToken();
                _logger.LogDebug("Token Generated Sucessfully for 384 service");

                // _ = _loggerRepository.LogDebug("Token Generated Sucessfully for 384 service");


                string finalURL = OEBSUrl + "PUMA/384";

                _logger.LogDebug("Data sent to process from OEBS : " + JsonConvert.SerializeObject(orderStatusRequestEntity));
                _ = _loggerRepository.LogDebug("Data sent to process from OEBS : " + JsonConvert.SerializeObject(orderStatusRequestEntity));


                _ = await finalURL.WithOAuthBearerToken(token)
                    .PostJsonAsync(orderStatusRequestEntity);
            }
            catch (Exception ex)
            {

                _logger.LogError("Error in 384 service : " + ex.Message, ex);
                _ = _loggerRepository.LogError("Error in 384 service : " + ex.Message, ex);

            }
        }

        public async Task<string> GetToken()
        {
            try
            {

                byte[] byte1 = Encoding.ASCII.GetBytes("grant_type=client_credentials");

                HttpWebRequest oRequest = WebRequest.Create(OEBSUrl + "oauth/token") as HttpWebRequest;
                oRequest.Accept = "application/json";
                oRequest.Method = "POST";
                oRequest.ContentType = "application/x-www-form-urlencoded";
                oRequest.ContentLength = byte1.Length;
                oRequest.KeepAlive = false;
                oRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(clientId + ":" + clientSecret)));
                Stream newStream = oRequest.GetRequestStream();
                newStream.Write(byte1, 0, byte1.Length);

                WebResponse oResponse = oRequest.GetResponse();

                using (var reader = new StreamReader(oResponse.GetResponseStream(), Encoding.UTF8))
                {
                    var oJsonReponse = await reader.ReadToEndAsync();
                    var payload = JObject.Parse(oJsonReponse);
                    return payload.Value<string>("access_token");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in oauth token generate : " + ex.Message, ex);
                _ = _loggerRepository.LogError("Error in oauth token generate : " + ex.Message, ex);

                throw;
            }

        }



        public async Task<AvtaleOppslagResponse> AgreementLookup389(AgreementLookup agreementLookup)
        {
            _logger.LogDebug("Inside into Agreement Lookup");
            agreementLookup.Header.MessageId = Guid.NewGuid().ToString();
            //If data is already in there then return data from cache
            var agreementLookUpData = await _manageCache.GetFromCache<AvtaleOppslagResponse>(agreementLookup.BrukerNavn);
            if (agreementLookUpData != null && !string.IsNullOrWhiteSpace(agreementLookUpData.KundeNr))
                return agreementLookUpData;
            else
            {
                AvtaleOppslagResponse avtaleOppslagResponse = null;
                var content = new StringContent(JsonConvert.SerializeObject(agreementLookup), Encoding.UTF8, "application/json");
                HttpClient httpclient = new HttpClient();
                _logger.LogDebug("AgreementLookup_Request Object = >{0}", JsonConvert.SerializeObject(agreementLookup));

                //AppSettings appsettings = new AppSettings();

                string ECUserName = await _appSettingRepository.GetAppSettingValue(AppSetting.ECUserName, false);
                string EC_API_URL = await _appSettingRepository.GetAppSettingValue(AppSetting.ECAPIURL, false);
                string ECPassword = await _appSettingRepository.GetAppSettingValue(AppSetting.ECPassword, true);
                var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", ECUserName, ECPassword));


                httpclient.DefaultRequestHeaders.Accept.Clear();
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage responseMessage = await httpclient.PostAsync(string.Format("{0}{1}", EC_API_URL, "/AgreementLookup/389"), content);
                responseMessage.EnsureSuccessStatusCode();

                if (responseMessage != null)
                {
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    _logger.LogInformation("AgreementLookup_Response Object = >{0}", jsonString);
                    //string terst = "{\"AvtaleOppslagResponse\":{\"KundeNr\":\"102399011\",\"statuskode\":null,\"AvtaleData\":null}}";
                    var responseData = JsonConvert.DeserializeObject<AgreementLookupResponse>(jsonString);
                    if (responseData != null)
                    {
                        if (responseData?.AvtaleOppslagResponse?.statuskode == 700 || string.IsNullOrWhiteSpace(responseData?.AvtaleOppslagResponse?.KundeNr))
                        {
                            List<PumaExceptionModel> pumaExceptionModels = new List<PumaExceptionModel>();
                            PumaExceptionModel pumaExceptionModel = new PumaExceptionModel()
                            {
                                Code = 401,
                                Message = "Enter valid credentials"
                            };

                            pumaExceptionModels.Add(pumaExceptionModel);
                            throw new PumaException(pumaExceptionModels);
                        }

                        avtaleOppslagResponse = responseData.AvtaleOppslagResponse;

                        await _manageCache.SetCache(agreementLookup.BrukerNavn, avtaleOppslagResponse);
                    }
                }


                return avtaleOppslagResponse;
            }
        }





        public class Root
        {
            public List<string> MyArray { get; set; }
        }
    }
}
