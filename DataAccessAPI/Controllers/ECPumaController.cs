#region
using DataAccessAPI.ECPumaHelper;
using DataAccessAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.BusinessEntity.EC_Data;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.OEBSService;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static DataAccessAPI.Models.Helper;
using static Puma.Shared.PumaEnum;
using Ext = DataAccessAPI.Models;
using Int = Puma.Shared;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class ECPumaController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ECPumaController> _logger;
        private readonly FinnUtvalgService finnUtvalg;
        private readonly HentAntallsopplysninger hentAntallsopplysninger;
        private readonly OppdaterUtvalg oppdaterUtvalg;
        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IUtvalgListRepository _utvalgListRepository;
        private readonly IKapasitetRepository _kapasitetRepository;
        private static HttpClient _httpclient;
        /// <summary>
        /// The i oebs service repository
        /// </summary>
        private readonly IOEBSServiceRepository _iOEBSServiceRepository;

        private readonly IAppSettingRepository _appSettingRepository;

        //private readonly EventHubHelper eventHubHelper;
        #endregion

        #region Properties
        private static HttpClient httpclient
        {
            get
            {
                if (_httpclient != null)
                    return _httpclient;
                else
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpclient = client;
                }
                return _httpclient;
            }
            set { }
        }

        #endregion

        #region Constructors        

        public ECPumaController(ILogger<ECPumaController> logger, ILogger<FinnUtvalgService> loggerFinn, ILogger<MappingUtvalg> loggerMapping, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList, ILogger<HentAntallsopplysninger> loggerHentAntall, ILogger<MappingUtvalgsdetaljer> loggerUtvalgDetaljer, ILogger<MappingAntallsopplysninger> loggerMappingAntall, ILogger<OppdaterUtvalg> loggerOppdaterUtvalg, ILogger<KapasitetController> loggerkapasitet,
            IUtvalgRepository utvalgRepository, IKapasitetRepository kapasitetRepository, IUtvalgListRepository utvalgListRepository, IOEBSServiceRepository iOEBSServiceRepository, IAppSettingRepository appSettingRepository)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;

            _appSettingRepository = appSettingRepository;

            _utvalgListRepository = utvalgListRepository;

            _kapasitetRepository = kapasitetRepository;

            finnUtvalg = new FinnUtvalgService(loggerFinn, loggerMapping, loggerConfig, loggerUtvalg, loggerreol, loggerUtvalgList, _utvalgRepository, _utvalgListRepository);
            hentAntallsopplysninger = new HentAntallsopplysninger(loggerHentAntall, loggerMappingAntall, loggerConfig, loggerUtvalg, loggerreol, loggerUtvalgList, _utvalgRepository, _utvalgListRepository);
            oppdaterUtvalg = new OppdaterUtvalg(loggerOppdaterUtvalg, loggerUtvalgDetaljer, loggerConfig, loggerUtvalg, loggerreol, loggerUtvalgList, loggerkapasitet, _utvalgRepository, _kapasitetRepository, _utvalgListRepository);
            //eventHubHelper = new EventHubHelper(loggerEvent);
            _iOEBSServiceRepository = iOEBSServiceRepository ?? throw new ArgumentNullException(nameof(iOEBSServiceRepository));



        }
        #endregion

        #region Public Methods

        #region FinnUtvalg (bruker ikke kø, returnerer svar)

        /// <summary>
        /// Exposed Web-method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("FinnUtvalg", Name = nameof(FinnUtvalg))]
        public FinnUtvalgResponse FinnUtvalg([FromQuery] FinnUtvalgRequest request)
        {
            _logger.BeginScope("Inside into FinnUtvalg");
            FinnUtvalgResponse response = null;
            string ipAddress;
            DateTime startTimeTotal = DateTime.Now;
            FeilKode? feilKode = null;
            Models.Helper.UtvalgListe utvalgListe = new Models.Helper.UtvalgListe();


            string messageID = string.Format("FinnUtvalg_{0}_{1}",
                request == null ? string.Empty : request.FinnUtvalg.Id.ToString(),
                request == null ? string.Empty : request.FinnUtvalg.Type.ToString());

            try
            {
                ipAddress = GetIPAddress();

                //FileLogging.LogStart(messageID, ipAddress);
                _logger.LogInformation(messageID, ipAddress);
                if (request == null)
                {
                    throw new ArgumentNullException("FinnUtvalg: Request objekt er null");
                }

                if (request.FinnUtvalg == null)
                {
                    throw new ArgumentNullException("FinnUtvalg: Request.FinnUtvalg objekt er null");
                }

                //FileLogging.LogObject(messageID, FileLogging.TypeOfObject.RequestFromClient, request);
                _logger.LogInformation("Message ID Returned for FinnUtvalg Flow: ", messageID, TypeOfObject.RequestFromClient, "Request: ", request, "Responsetid Geodata", startTimeTotal, DateTime.Now);
                Ext.FinnUtvalgData finnUtvalgData = request.FinnUtvalg;
                //DateTime startTimeGeoData = DateTime.Now;
                List<Int.ECPumaData> utvalgListResult = finnUtvalg.FinnUtvalgIntegrasjon(finnUtvalgData.Kundenr, finnUtvalgData.Id, TranslateBothWays.UtvalgTypeKodeToInternal(finnUtvalgData.Type), finnUtvalgData.InkluderDetaljer, ref feilKode);

                //FileLogging.LogTimeUsed("Responsetid Geodata", startTimeGeoData, DateTime.Now);
                //FileLogging.LogTimeUsed("Responsetid Geodata", startTimeTotal, DateTime.Now);

                response = new FinnUtvalgResponse();

                if (feilKode.HasValue)
                {
                    response.Feilkode = (int)feilKode.Value;
                    response.Feilbeskrivelse = "Feilkode_FinnUtvalg: " + feilKode;
                }
                else
                {
                    foreach (Int.ECPumaData utvalg in utvalgListResult)
                    {
                        utvalgListe.Add(TranslateInternalToExternal.Utvalg(utvalg));
                    }

                    response.Utvalg = utvalgListe;
                }
                _logger.LogInformation("Message ID Returned for FinnUtvalg Flow: ", messageID, TypeOfObject.ResponseToClient, "Response Returned: ", response, "Responsetid Total", startTimeTotal, DateTime.Now);
                //FileLogging.LogObject(messageID, FileLogging.TypeOfObject.ResponseToClient, response);
                //FileLogging.LogTimeUsed("Responsetid Total", startTimeTotal, DateTime.Now);
                //FileLogging.LogEnd(messageID);

                _logger.LogDebug("Exiting from FinnUtvalg");
                return response;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error logger at DataAccessAPIController: FinnUtvalg ", exception.Message);
                throw new Exception("Error logger at DataAccessAPIController: FinnUtvalg " + messageID + exception.Message);
                //ValidationFault fault = ExceptionHandling.HandleException("DataAccessAPI", messageID, ex);
                //throw new FaultException<ValidationFault>(fault, "FinnUtvalg");
            }
        }

        #endregion

        #region HentUtvalgAntall (0381 Bruker ikke kø, returnerer svar)

        /// <summary>
        /// Exposed Web-method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("HentUtvalgAntall", Name = nameof(HentUtvalgAntall))]
        public HentUtvalgAntallResponse HentUtvalgAntall([FromQuery] HentUtvalgAntallRequest request)
        {
            _logger.BeginScope("Inside into HentUtvalgAntall");
            HentUtvalgAntallResponse response = null;
            string ipAddress;
            DateTime startTimeTotal = DateTime.Now;
            FeilKode? feilKode = null;
            string messageID = string.Format("HentUtvalgAntall_{0}_{1}",
                request == null ? string.Empty : request.UtvalgsId.Id.ToString(),
                request == null ? string.Empty : request.UtvalgsId.Type.ToString());

            try
            {
                ipAddress = GetIPAddress();

                //FileLogging.LogStart(messageID, ipAddress);
                _logger.LogInformation(messageID, ipAddress);

                if (request == null)
                {
                    throw new ArgumentNullException("HentUtvalgAntall: Request objekt er null");
                }

                if (request.UtvalgsId == null)
                {
                    throw new ArgumentNullException("HentUtvalgAntall: Request.UtvalgsId objekt er null");
                }

                //FileLogging.LogObject(messageID, FileLogging.TypeOfObject.RequestFromClient, request);
                _logger.LogInformation("Message ID Returned for HentUtvalgAntall Flow: ", messageID, TypeOfObject.RequestFromClient, "Request: ", request, "Responsetid Geodata", startTimeTotal, DateTime.Now);

                List<Antallsopplysninger> antallsopplysningerList = null;

                //DateTime startTimeGeoData = DateTime.Now;
                antallsopplysningerList = hentAntallsopplysninger.HentAntallsopplysningerIntegrasjon(TranslateBothWays.UtvalgsIdToInternal(request.UtvalgsId), ref feilKode);
                //FileLogging.LogTimeUsed("Responsetid Geodata", startTimeTotal, DateTime.Now);
                //FileLogging.LogTimeUsed("Responsetid Geodata", startTimeGeoData, DateTime.Now);

                response = new HentUtvalgAntallResponse();

                if (feilKode.HasValue)
                {
                    response.Feilkode = (int)feilKode.Value;
                    response.Feilbeskrivelse = "Feilkode_HentUtvalgAntall: " + feilKode;
                }
                else
                {
                    UtvalgantallListe utvalgAntallList = new UtvalgantallListe();
                    foreach (Antallsopplysninger antallsopplysninger in antallsopplysningerList)
                    {
                        utvalgAntallList.Add(TranslateInternalToExternal.AntallsopplysningerToUtvalgAntall(antallsopplysninger));
                    }

                    response.UtvalgAntall = utvalgAntallList;
                }
                _logger.LogInformation("Message ID Returned for HentUtvalgAntall Flow: ", messageID, TypeOfObject.ResponseToClient, "Response Returned: ", response, "Responsetid Total", startTimeTotal, DateTime.Now);
                //FileLogging.LogObject(messageID, FileLogging.TypeOfObject.ResponseToClient, response);
                //FileLogging.LogTimeUsed("Responsetid Total", startTimeTotal, DateTime.Now);
                //FileLogging.LogEnd(messageID);

                _logger.LogDebug("Exiting from HentUtvalgAntall");
                return response;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error logger at DataAccessAPIController: HentUtvalgAntall ", messageID, exception.Message);
                throw new Exception("Error logger at DataAccessAPIController: HentUtvalgAntall " + messageID + exception.Message);
                //ValidationFault fault = ExceptionHandling.HandleException("DataAccessAPI", messageID, ex);
                //throw new FaultException<ValidationFault>(fault, "HentUtvalgAntall");
            }
        }

        #endregion

        #region OppdaterOrdreStatus (0381 Bruker kø, returnerer svar)

        /// <summary>
        /// Exposed Web-method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("OppdaterOrdreStatus", Name = nameof(OppdaterOrdreStatus))]
        public OrdreStatusResponse OppdaterOrdreStatus(OrdreStatusRequest request)
        {
            _logger.BeginScope("Inside into OppdaterOrdreStatus");
            OrdreStatusResponse response = null;
            //MQ mq = new MQ();

            string messageID = string.Format("OppdaterOrdreStatus_{0}_{1}",
                request == null ? string.Empty : request.OrdreStatusData.Id.ToString(),
                request == null ? string.Empty : request.OrdreStatusData.Type.ToString());

            try
            {
                DateTime startTimeTotal = DateTime.Now;
                DateTime startTimeGeoData = DateTime.MinValue;

                //string ip = GetIPAddress();
                //FileLogging.LogStart(messageID, ip);

                if (request == null)
                {
                    throw new ArgumentNullException("OppdaterOrdreStatus: Request objekt er null");
                }

                if (request.OrdreStatusData == null)
                {
                    throw new ArgumentNullException("OppdaterOrdreStatus: Request.OrdreStatus objekt er null");
                }

                if (string.IsNullOrEmpty(request.CorrelationID))
                {
                    request.CorrelationID = Guid.NewGuid().ToString();
                }

                _logger.LogInformation(messageID, TypeOfObject.RequestFromClient, request);

                FeilKode? feilKode = null;
                Ext.OrdreStatusData ordreStatus = request.OrdreStatusData;

                // _ = finnUtvalg.FinnUtvalgIntegrasjon(null, ordreStatus.Id, TranslateBothWays.UtvalgTypeKodeToInternal(ordreStatus.Type), false, ref feilKode);

                if (feilKode.HasValue && feilKode.Value == FeilKode.UtvalgEksistererIkke)
                {
                    string error = string.Format("Feilkode_OppdaterOrdreStatus: {0}", feilKode);
                    throw new ApplicationException(error);
                }

                response = new OrdreStatusResponse();
                startTimeGeoData = DateTime.Now;

                if (!ordreStatus.ReturnerFordeling)
                {
                    OrdreStatusService status = TranslateExternalToInternal.OrdreStatus(ordreStatus);

                    oppdaterUtvalg.OppdaterUtvalgIntegrasjon(status);
                    _logger.LogInformation("Responsetid Geodata - Returnerfordeling = false", startTimeGeoData, DateTime.Now);

                    response.WillRespond = false;
                }

                return response;
            }
            catch (Exception exception)
            {
                // mq.QueueClose(false);
                _logger.LogError(exception, "Error logger at DataAccessAPIController: OppdaterOrdreStatus ", messageID, exception.Message);
                throw new Exception("Error logger at DataAccessAPIController: OppdaterOrdreStatus " + messageID + exception.Message);
                //ValidationFault fault = ExceptionHandling.HandleException("UtvalgService", messageID, ex);
                //throw new FaultException<ValidationFault>(fault, "OppdaterOrdreStatus");
            }
            finally
            {
                _logger.LogDebug("Exiting from OppdaterOrdreStatus: ", messageID);
            }
        }

        #endregion

        #region Find Customer , Interface 380
        /// <summary>
        /// Get the customer detail from EC 5 380 interface
        /// </summary>
        /// <param name="findCustomer">Reuest object for customer</param>
        /// <returns>Get Customer details</returns>
        [HttpPost("FindCustomer380", Name = nameof(FindCustomer380))]
        public async Task<FindCustomerResponse> FindCustomer380(FindCustomer findCustomer)
        {
            _logger.BeginScope("Inside into FindCustomer");

            var content = new StringContent(JsonConvert.SerializeObject(findCustomer), Encoding.UTF8, "application/json");

            _logger.LogDebug("FindCustomer_Request Object = >{0}", JsonConvert.SerializeObject(findCustomer));

            try
            {
                // AppSettings appsettings = new AppSettings();

                string ECUserName = await _appSettingRepository.GetAppSettingValue(AppSetting.ECUserName, false);
                string EC_API_URL = await _appSettingRepository.GetAppSettingValue(AppSetting.ECAPIURL, false);
                string ECPassword = await _appSettingRepository.GetAppSettingValue(AppSetting.ECPassword, true);
                if (_httpclient == null)
                {
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", ECUserName, ECPassword));
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }

                HttpResponseMessage responseMessage = await httpclient.PostAsync(string.Format("{0}{1}", EC_API_URL, "/T50802_FindCustomerFromPUMA_To_OeBS/T50802_FindCustomerFromPUMA_To_OEBS_RS"), content);
                responseMessage.EnsureSuccessStatusCode();

                if (responseMessage != null)
                {
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    _logger.LogInformation("FindCustomer_Response Object = >{0}", jsonString);
                    return JsonConvert.DeserializeObject<FindCustomerResponse>(jsonString);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error logger at DataAccessAPIController: FindCustomer ", exception.Message);
            }

            return null;
        }
        #endregion


        #region Agreement Lookup, Interface 389
        /// <summary>
        /// Get the Agreement Lookup from EC 5 389 interface
        /// </summary>
        /// <param name="agreementLookup">Reuest object for agreement lookup</param>
        /// <returns>Get agreement lookup details</returns>
        [HttpPost("AgreementLookup389", Name = nameof(AgreementLookup389))]
        public async Task<Puma.DataLayer.BusinessEntity.EC_Data.AvtaleOppslagResponse> AgreementLookup389(Puma.DataLayer.BusinessEntity.EC_Data.AgreementLookup agreementLookup)
        {
            #region Old Code
            //_logger.LogDebug("Inside into Agreement Lookup");
            //DataAccessAPI.Models.AvtaleOppslagResponse avtaleOppslagResponse = null;
            //var content = new StringContent(JsonConvert.SerializeObject(agreementLookup), Encoding.UTF8, "application/json");

            //_logger.LogDebug("AgreementLookup_Request Object = >{0}", JsonConvert.SerializeObject(agreementLookup));
            //try
            //{
            //    //AppSettings appsettings = new AppSettings();

            //    string ECUserName = await _appSettingRepository.GetAppSettingValue(AppSetting.ECUserName, false);
            //    string EC_API_URL = await _appSettingRepository.GetAppSettingValue(AppSetting.ECAPIURL, false);
            //    string ECPassword = await _appSettingRepository.GetAppSettingValue(AppSetting.ECPassword, true);

            //    if (_httpclient == null)
            //    {
            //        var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", ECUserName, ECPassword));
            //        httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //    }

            //    HttpResponseMessage responseMessage = await httpclient.PostAsync(string.Format("{0}{1}", EC_API_URL, "/AgreementLookup/389"), content);
            //    responseMessage.EnsureSuccessStatusCode();

            //    if (responseMessage != null)
            //    {
            //        var jsonString = await responseMessage.Content.ReadAsStringAsync();
            //        _logger.LogInformation("AgreementLookup_Response Object = >{0}", jsonString);
            //        //string terst = "{\"AvtaleOppslagResponse\":{\"KundeNr\":\"102399011\",\"statuskode\":null,\"AvtaleData\":null}}";
            //        var responseData = JsonConvert.DeserializeObject<DataAccessAPI.Models.AgreementLookupResponse>(jsonString);
            //        if (responseData != null)
            //        {
            //            avtaleOppslagResponse = responseData.AvtaleOppslagResponse;
            //        }
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error logger at DataAccessAPIController: AgreementLookup ", exception.Message);
            //}

            //return avtaleOppslagResponse; 
            #endregion
            return await _iOEBSServiceRepository.AgreementLookup389(agreementLookup);
        }
        #endregion

        #region OppdaterBasisUtvalgsfordeling (Bruker kø, debatcher, men returnerer void)

        /// <summary>
        /// Used in Business Analytiker for processing 384 Request
        /// </summary>
        /// <param name="request">Instance of order</param>
        /// <returns></returns>
        [HttpPost("OppdaterBasisUtvalgsfordeling", Name = nameof(OppdaterBasisUtvalgsfordeling))]
        public async Task OppdaterBasisUtvalgsfordeling(OppdaterUtvalgsfordelingRequest request)
        {
            _logger.BeginScope("Inside into OppdaterBasisUtvalgsfordelingAsync");

            string messageID = string.Empty;
            DateTime startTimeTotal = DateTime.Now;
            DateTime startTimeGeoData = DateTime.MinValue;
            Int.Utvalgsfordeling utvalgsfordeling;

            if (request == null)
            {
                _logger.LogError("OppdaterBasisUtvalgsfordeling: Request obect cannot be null");
            }
            else if (request.UtvalgsIdListe == null)
            {
                _logger.LogError("OppdaterBasisUtvalgsfordeling: Request.UtvalgsIdListe objekt er null");
            }
            else
            {
                messageID = string.Format("OppdaterBasisUtvalgsfordeling_{0}_{1}",
                request == null ? string.Empty : request.UtvalgsIdListe[0].Id.ToString(),
                request == null ? string.Empty : request.UtvalgsIdListe[0].Type.ToString());

                foreach (Ext.UtvalgsId utvalgsId in request.UtvalgsIdListe)
                {
                    if (utvalgsId != null)
                    {
                        utvalgsfordeling = oppdaterUtvalg.HentUtvalgsfordeling(utvalgsId.Id, TranslateBothWays.UtvalgTypeKodeToInternal(utvalgsId.Type));
                        _logger.LogInformation("Message ID Returned for HentUtvalgsfordeling Flow: ", messageID, TypeOfObject.RequestFromClient, "Request: ", request, "Responsetid Geodata", startTimeTotal, DateTime.Now);

                        DataAccessAPI.Models.Utvalgsfordeling utvalgsfordelingData = TranslateInternalToExternal.Utvalgsfordeling(utvalgsfordeling);
                        utvalgsfordelingData.Reason = "B"; // B -> Basis 

                        string message = Serialize.ToUTF8String(utvalgsfordelingData);

                        OrderStatusResponseEntity orderStatusResponseEntity = new OrderStatusResponseEntity()
                        {
                            CORRID = "OppdaterBasisUtvalgsfordeling",
                            SOURCE = "BusinessAnalytiker/ReolGenerator",
                            MSG = message
                        };

                        await _iOEBSServiceRepository.PostSelectionDistribution(orderStatusResponseEntity);
                    }
                }
            }
        }

        /// <summary>
        /// Used in Business Analytiker for processing 384 Request
        /// </summary>
        /// <param name="request">Instance of order</param>
        /// <returns></returns>
        [HttpPost("OppdaterUtvalgsfordeling", Name = nameof(OppdaterUtvalgsfordeling))]
        public async Task OppdaterUtvalgsfordeling(OppdaterUtvalgsfordelingRequest request)
        {
            _logger.BeginScope("Inside into OppdaterBasisUtvalgsfordelingAsync");

            string messageID = string.Empty;
            DateTime startTimeTotal = DateTime.Now;
            DateTime startTimeGeoData = DateTime.MinValue;
            Int.Utvalgsfordeling utvalgsfordeling;

            if (request == null)
            {
                _logger.LogError("OppdaterBasisUtvalgsfordeling: Request obect cannot be null");
            }
            else if (request.UtvalgsIdListe == null)
            {
                _logger.LogError("OppdaterBasisUtvalgsfordeling: Request.UtvalgsIdListe objekt er null");
            }
            else
            {
                messageID = string.Format("OppdaterBasisUtvalgsfordeling_{0}_{1}",
                request == null ? string.Empty : request.UtvalgsIdListe[0].Id.ToString(),
                request == null ? string.Empty : request.UtvalgsIdListe[0].Type.ToString());

                foreach (Ext.UtvalgsId utvalgsId in request.UtvalgsIdListe)
                {
                    if (utvalgsId != null)
                    {
                        utvalgsfordeling = oppdaterUtvalg.HentUtvalgsfordeling(utvalgsId.Id, TranslateBothWays.UtvalgTypeKodeToInternal(utvalgsId.Type));
                        _logger.LogInformation("Message ID Returned for HentUtvalgsfordeling Flow: ", messageID, TypeOfObject.RequestFromClient, "Request: ", request, "Responsetid Geodata", startTimeTotal, DateTime.Now);

                        DataAccessAPI.Models.Utvalgsfordeling utvalgsfordelingData = TranslateInternalToExternal.Utvalgsfordeling(utvalgsfordeling);
                        utvalgsfordelingData.Reason = "G";  // G->Gjenskapning 

                        string message = Serialize.ToUTF8String(utvalgsfordelingData);

                        OrderStatusResponseEntity orderStatusResponseEntity = new OrderStatusResponseEntity()
                        {
                            CORRID = "OppdaterUtvalgsfordeling",
                            SOURCE = "BusinessAnalytiker/ReolGenerator",
                            MSG = message
                        };

                        await _iOEBSServiceRepository.PostSelectionDistribution(orderStatusResponseEntity);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetIPAddress()
        {
            //OperationContext context = OperationContext.Current;
            //System.ServiceModel.Channels.MessageProperties prop = context.IncomingMessageProperties;
            //System.ServiceModel.Channels.RemoteEndpointMessageProperty endpoint = prop[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name] as System.ServiceModel.Channels.RemoteEndpointMessageProperty;
            //return endpoint.Address;
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return ip;
        }

        #endregion
    }

}
