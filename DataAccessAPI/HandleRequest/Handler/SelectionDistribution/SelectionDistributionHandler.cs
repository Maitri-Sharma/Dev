using AutoMapper;
using DataAccessAPI.Controllers;
using DataAccessAPI.ECPumaHelper;
using DataAccessAPI.HandleRequest.Request.SelectionDistribution;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.Models;
using Hangfire.Console;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Puma.DataLayer.BusinessEntity.EC_Data;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.OEBSService;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Infrastructure.Interface.Logger;
using Puma.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utvalgsfordeling = Puma.Shared.Utvalgsfordeling;

namespace DataAccessAPI.HandleRequest.Handler.SelectionDistribution
{
    /// <summary>
    /// SelectionDistributionHandler
    /// </summary>
    public class SelectionDistributionHandler : IRequestHandler<RequestSelectionDistribution, bool>
    {
        /// <summary>
        /// The i oebs service repository
        /// </summary>
        private readonly IOEBSServiceRepository _iOEBSServiceRepository;


        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        private readonly IMapper _mapper;




        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ECPumaController> logger;
        /// <summary>
        /// The logger finn
        /// </summary>
        private readonly ILogger<FinnUtvalgService> loggerFinn;
        /// <summary>
        /// The logger mappin
        /// </summary>
        private readonly ILogger<MappingUtvalg> loggerMappin;
        /// <summary>
        /// The logger configuration
        /// </summary>
        private readonly ILogger<ConfigController> loggerConfig;
        /// <summary>
        /// The logger utvalg
        /// </summary>
        private readonly ILogger<UtvalgController> loggerUtvalg;
        /// <summary>
        /// The loggerreol
        /// </summary>
        private readonly ILogger<ReolController> loggerreol;
        /// <summary>
        /// The logger utvalg list
        /// </summary>
        private readonly ILogger<UtvalgListController> loggerUtvalgList;
        /// <summary>
        /// The logger hent antall
        /// </summary>
        private readonly ILogger<HentAntallsopplysninger> loggerHentAntall;
        /// <summary>
        /// The logger utvalg detaljer
        /// </summary>
        private readonly ILogger<MappingUtvalgsdetaljer> loggerUtvalgDetaljer;
        /// <summary>
        /// The logger mapping antall
        /// </summary>
        private readonly ILogger<MappingAntallsopplysninger> loggerMappingAntall;
        /// <summary>
        /// The logger oppdater utvalg
        /// </summary>
        private readonly ILogger<OppdaterUtvalg> loggerOppdaterUtvalg;
        /// <summary>
        /// The loggerkapasitet
        /// </summary>
        private readonly ILogger<KapasitetController> loggerkapasitet;

        /// <summary>
        /// The loggerkapasitet
        /// </summary>
        private readonly ILogger<SelectionDistributionHandler> _loggerSelection;
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository utvalgRepository;
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository kapasitetRepository;
        /// <summary>
        /// The utvalg list repository
        /// </summary>
        private readonly IUtvalgListRepository utvalgListRepository;

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// The oppdater utvalg
        /// </summary>
        private readonly OppdaterUtvalg oppdaterUtvalg;

        private readonly IConfiguration _configuration;

        private readonly ILoggerRepository _loggerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDistributionHandler" /> class.
        /// </summary>
        /// <param name="iOEBSServiceRepository">The i oebs service repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerFinn">The logger finn.</param>
        /// <param name="loggerMappin">The logger mappin.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="loggerUtvalg">The logger utvalg.</param>
        /// <param name="loggerreol">The loggerreol.</param>
        /// <param name="loggerUtvalgList">The logger utvalg list.</param>
        /// <param name="loggerHentAntall">The logger hent antall.</param>
        /// <param name="loggerUtvalgDetaljer">The logger utvalg detaljer.</param>
        /// <param name="loggerMappingAntall">The logger mapping antall.</param>
        /// <param name="loggerOppdaterUtvalg">The logger oppdater utvalg.</param>
        /// <param name="loggerkapasitet">The loggerkapasitet.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="loggerSelection">The logger selection.</param>
        /// <param name="config"></param>
        /// <param name="loggerRepository"></param>
        /// <exception cref="System.ArgumentNullException">
        /// iOEBSServiceRepository
        /// or
        /// mapper
        /// or
        /// loggerSelection
        /// or
        /// logger
        /// or
        /// loggerFinn
        /// or
        /// loggerMappin
        /// or
        /// loggerConfig
        /// or
        /// loggerUtvalg
        /// or
        /// loggerreol
        /// or
        /// loggerUtvalgList
        /// or
        /// loggerHentAntall
        /// or
        /// loggerUtvalgDetaljer
        /// or
        /// loggerMappingAntall
        /// or
        /// loggerOppdaterUtvalg
        /// or
        /// loggerkapasitet
        /// or
        /// utvalgRepository
        /// or
        /// kapasitetRepository
        /// or
        /// utvalgListRepository
        /// or
        /// mediator
        /// </exception>

        public SelectionDistributionHandler(IOEBSServiceRepository iOEBSServiceRepository,
            IMapper mapper, ILogger<ECPumaController> logger, ILogger<FinnUtvalgService> loggerFinn,
            ILogger<MappingUtvalg> loggerMappin, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList, ILogger<HentAntallsopplysninger> loggerHentAntall, ILogger<MappingUtvalgsdetaljer> loggerUtvalgDetaljer, ILogger<MappingAntallsopplysninger> loggerMappingAntall, ILogger<OppdaterUtvalg> loggerOppdaterUtvalg, ILogger<KapasitetController> loggerkapasitet, IUtvalgRepository utvalgRepository, IKapasitetRepository kapasitetRepository, IUtvalgListRepository utvalgListRepository, IMediator mediator,
            ILogger<SelectionDistributionHandler> loggerSelection, IConfiguration config, ILoggerRepository loggerRepository)
        {
            _iOEBSServiceRepository = iOEBSServiceRepository ?? throw new ArgumentNullException(nameof(iOEBSServiceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _loggerSelection = loggerSelection ?? throw new ArgumentNullException(nameof(loggerSelection));


            //loggerSelection
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.loggerFinn = loggerFinn ?? throw new ArgumentNullException(nameof(loggerFinn));
            this.loggerMappin = loggerMappin ?? throw new ArgumentNullException(nameof(loggerMappin));
            this.loggerConfig = loggerConfig ?? throw new ArgumentNullException(nameof(loggerConfig));
            this.loggerUtvalg = loggerUtvalg ?? throw new ArgumentNullException(nameof(loggerUtvalg));
            this.loggerreol = loggerreol ?? throw new ArgumentNullException(nameof(loggerreol));
            this.loggerUtvalgList = loggerUtvalgList ?? throw new ArgumentNullException(nameof(loggerUtvalgList));
            this.loggerHentAntall = loggerHentAntall ?? throw new ArgumentNullException(nameof(loggerHentAntall));
            this.loggerUtvalgDetaljer = loggerUtvalgDetaljer ?? throw new ArgumentNullException(nameof(loggerUtvalgDetaljer));
            this.loggerMappingAntall = loggerMappingAntall ?? throw new ArgumentNullException(nameof(loggerMappingAntall));
            this.loggerOppdaterUtvalg = loggerOppdaterUtvalg ?? throw new ArgumentNullException(nameof(loggerOppdaterUtvalg));
            this.loggerkapasitet = loggerkapasitet ?? throw new ArgumentNullException(nameof(loggerkapasitet));
            this.utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            this.kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            this.utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            oppdaterUtvalg = new OppdaterUtvalg(loggerOppdaterUtvalg, loggerUtvalgDetaljer, loggerConfig, loggerUtvalg,
                loggerreol, loggerUtvalgList, loggerkapasitet,
                this.utvalgRepository, this.kapasitetRepository, this.utvalgListRepository);
            _configuration = config ?? throw new ArgumentNullException(nameof(config));
            _loggerRepository = loggerRepository ?? throw new ArgumentNullException(nameof(loggerRepository));

        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> Handle(RequestSelectionDistribution request, CancellationToken cancellationToken)
        {
            try
            {


                //First will call oebs service that retrurns order status data
                request.context.WriteLine("Calling 383 Service");
                var orderStatusList = await _iOEBSServiceRepository.GetOrderStatus();
                if (orderStatusList?.Any() == true)
                {
                    ECPumaController eCPumaController = new ECPumaController(logger, loggerFinn, loggerMappin, loggerConfig, loggerUtvalg, loggerreol,
                                                   loggerUtvalgList, loggerHentAntall, loggerUtvalgDetaljer, loggerMappingAntall, loggerOppdaterUtvalg, loggerkapasitet,
                                                   utvalgRepository, kapasitetRepository, utvalgListRepository, _iOEBSServiceRepository, null);
                    foreach (var orderStatusEntity in orderStatusList)
                    {
                        OrdreStatusRequestEntity orderStatusData = new OrdreStatusRequestEntity()
                        {
                            OrdreStatusData = orderStatusEntity
                        };
                        request.context.WriteLine("Processing Order" + orderStatusEntity.Id);
                        _ = _loggerRepository.LogInformation("Processing Order" + orderStatusEntity.Id);

                        OrdreStatusRequest ordreStatusRequest = _mapper.Map<OrdreStatusRequestEntity, OrdreStatusRequest>(orderStatusData);

                        if (ordreStatusRequest != null)
                        {
                            try
                            {
                                _loggerSelection.LogDebug("Processing Order " + JsonConvert.SerializeObject(ordreStatusRequest));
                                _ = _loggerRepository.LogInformation("Processing Order " + JsonConvert.SerializeObject(ordreStatusRequest));
                                eCPumaController.OppdaterOrdreStatus(ordreStatusRequest);
                            }
                            catch (Exception ex)
                            {
                                _loggerSelection.LogError(ex.Message, ex);
                                request.context.WriteLine("Error during process record" + orderStatusEntity.Id + orderStatusEntity.Type + " : " + ex.Message);

                                _ = _loggerRepository.LogError("Error during process record" + orderStatusEntity.Id + orderStatusEntity.Type + " : " + ex.Message, ex);
                            }

                            request.context.WriteLine("Processing complete for order" + orderStatusEntity.Id);

                            _ = _loggerRepository.LogInformation("Processing complete for order" + orderStatusEntity.Id);


                            if (ordreStatusRequest.OrdreStatusData.ReturnerFordeling)
                            {
                                try
                                {
                                    _loggerSelection.LogDebug("Processing Order for posting data " + JsonConvert.SerializeObject(ordreStatusRequest));
                                    _ = _loggerRepository.LogInformation("Processing Order for posting data " + JsonConvert.SerializeObject(ordreStatusRequest));

                                    await ProcessFor384Service(ordreStatusRequest.OrdreStatusData);
                                }
                                catch (Exception ex)
                                {
                                    _loggerSelection.LogError(ex.Message, ex);
                                    request.context.WriteLine("Error during Post record" + orderStatusEntity.Id + orderStatusEntity.Type + " : " + ex.Message);
                                    _ = _loggerRepository.LogError("Error during Post record" + orderStatusEntity.Id + orderStatusEntity.Type + " : " + ex.Message, ex);


                                }
                            }
                        }
                    }
                }
                request.context.WriteLine("No data found for processing");

            }
            catch (Exception ex)
            {

                request.context.WriteLine("Error during Processing job: " + ex.Message);
                _ = _loggerRepository.LogError("Error during Processing job: " + ex.Message,ex);

            }

            return true;
        }


        /// <summary>
        /// Processes the for384 service.
        /// </summary>
        /// <param name="ordreStatusData">The ordre status data.</param>
        /// <exception cref="System.ArgumentNullException">fordeling</exception>
        private async Task ProcessFor384Service(OrdreStatusData ordreStatusData)
        {
            try
            {

                OrdreStatusService status = TranslateExternalToInternal.OrdreStatus(ordreStatusData);

                Utvalgsfordeling utvalgsfordeling = oppdaterUtvalg.OppdaterUtvalgIntegrasjon(status);

                if (utvalgsfordeling == null)
                {
                    throw new ArgumentNullException("fordeling");
                }

                DataAccessAPI.Models.Utvalgsfordeling utvalgsfordelingData = TranslateInternalToExternal.Utvalgsfordeling(utvalgsfordeling);
                utvalgsfordelingData.Reason = "O";


                //Tranformation logic

                #region Transformation logic implemented at EConnect Side
                //we have implemented only required logic is implemented as its working fine
                //if (PUMAmsg / Id == null) -- > OEBSmsg / Id = "0"

                //if (PUMAmsg / Type == null) -- > OEBSmsg / Type = "Liste"

                //if (PUMAmsg / DatoOppdatert == null) -- > OEBSmsg / DatoOppdatert = "1970-01-01T00:00:00.0Z"

                //if (PUMAmsg / Utvalgsdetaljer / UtvalgDetaljer / Fordelinger / Fordeling / TeamKomplett == "true") -- > OEBSmsg / Utvalgsdetaljer / UtvalgDetaljer / Fordelinger / Fordeling / TeamKomplett = "Y"

                //if (PUMAmsg / Utvalgsdetaljer / UtvalgDetaljer / Fordelinger / Fordeling / TeamKomplett == "false") -- > OEBSmsg / Utvalgsdetaljer / UtvalgDetaljer / Fordelinger / Fordeling / TeamKomplett = "N"

                //if (PUMAmsg / Utvalgsdetaljer / UtvalgDetaljer / Fordelinger / Fordeling / Prs-- > the field to be padded with "000000"

                //e.g. 45-- > 000045 
                #endregion


                string message = Serialize.ToUTF8String(utvalgsfordelingData);

                OrderStatusResponseEntity orderStatusResponseEntity = new OrderStatusResponseEntity()
                {
                    CORRID = "Utvalgsfordeling",
                    SOURCE = "PUMA",
                    MSG = message
                };

                await _iOEBSServiceRepository.PostSelectionDistribution(orderStatusResponseEntity);
            }
            catch (Exception ex)
            {

                _loggerSelection.LogError(ex.Message, ex);
            }
        }


    }
}
