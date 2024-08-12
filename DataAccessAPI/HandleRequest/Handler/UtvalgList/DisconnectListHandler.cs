using AutoMapper;
using DataAccessAPI.Controllers;
using DataAccessAPI.ECPumaHelper;
using DataAccessAPI.HandleRequest.Request.SelectionDistribution;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
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
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DataAccessAPI.Models.Helper;
using static Puma.Shared.PumaEnum;
using Utvalgsfordeling = Puma.Shared.Utvalgsfordeling;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// DisconnectListHandler
    /// </summary>
    public class DisconnectListHandler : IRequestHandler<RequestDisconnectList, ResponseDisconnectList>
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
        private readonly ILogger<DisconnectListHandler> _loggerSelection;
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

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;



        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectListHandler" /> class.
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
        /// <param name="loggerSelection">The logger selection.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">iOEBSServiceRepository
        /// or
        /// mapper
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
        /// loggerSelection
        /// or
        /// utvalgRepository
        /// or
        /// kapasitetRepository
        /// or
        /// utvalgListRepository
        /// or
        /// mediator
        /// or
        /// oppdaterUtvalg
        /// or
        /// configuration</exception>
        public DisconnectListHandler(IOEBSServiceRepository iOEBSServiceRepository, IMapper mapper, ILogger<ECPumaController> logger, ILogger<FinnUtvalgService> loggerFinn, ILogger<MappingUtvalg> loggerMappin, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList, ILogger<HentAntallsopplysninger> loggerHentAntall, ILogger<MappingUtvalgsdetaljer> loggerUtvalgDetaljer, ILogger<MappingAntallsopplysninger> loggerMappingAntall, ILogger<OppdaterUtvalg> loggerOppdaterUtvalg, ILogger<KapasitetController> loggerkapasitet, ILogger<DisconnectListHandler> loggerSelection, IUtvalgRepository utvalgRepository, IKapasitetRepository kapasitetRepository, IUtvalgListRepository utvalgListRepository, IMediator mediator, IConfiguration configuration)
        {
            _iOEBSServiceRepository = iOEBSServiceRepository ?? throw new ArgumentNullException(nameof(iOEBSServiceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            _loggerSelection = loggerSelection ?? throw new ArgumentNullException(nameof(loggerSelection));
            this.utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            this.kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            this.utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            oppdaterUtvalg = new OppdaterUtvalg(loggerOppdaterUtvalg, loggerUtvalgDetaljer, loggerConfig, loggerUtvalg,
                 loggerreol, loggerUtvalgList, loggerkapasitet,
                 this.utvalgRepository, this.kapasitetRepository, this.utvalgListRepository);
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<ResponseDisconnectList> Handle(RequestDisconnectList request, CancellationToken cancellationToken)
        {


            #region Old Code
            // request.UtvalgList.WasBasedOn = request.UtvalgList.BasedOn;
            // request.UtvalgList.BasedOn = 0;
            // request.UtvalgList.ParentList = null;

            // _ = await utvalgListRepository.SaveUtvalgListData(request.UtvalgList, request.UserName);

            //// DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central european Time"));

            // string suffix = "_" + DateTime.Now.ToString("ddMMyy") + "_" + DateTime.Now.ToString("HH:mm");

            // //copy & save all child lists
            // await utvalgListRepository.CopySaveUtvalgListCopies(request.UtvalgList, request.UtvalgList, request.UserName, suffix, true, true);

            // //copy & save all child utvalg
            // await utvalgListRepository.CopySaveUtvalgCopies(request.UtvalgList, request.UtvalgList.MemberUtvalgs, request.UserName, suffix, true, true);

            // //remove all basiscontent from list
            // request.UtvalgList.MemberLists = (from ml in request.UtvalgList.MemberLists where !ml.IsBasis select ml).ToList();
            // request.UtvalgList.MemberUtvalgs = (from ul in request.UtvalgList.MemberUtvalgs where !ul.IsBasis select ul).ToList();


            // // OEBS må få beskjed om å oppdatere utvalgsfordelingen for denne listen.
            // List<UtvalgIdWrapper> idList = new List<UtvalgIdWrapper>();
            // idList.Add(new UtvalgIdWrapper(request.UtvalgList.ListId, UtvalgType.UtvalgList));
            // // UtvalgServiceProxy.UtvalgServiceProxy.OppdaterUtvalgsfordelingFromDisconnectList(idList);
            // await OppdaterUtvalgsfordelingAsync(idList, true);
            // var utvalgListData = await utvalgListRepository.GetUtvalgListWithAllReferences(request.UtvalgList.ListId); 
            #endregion

            var utvalgListData = await utvalgListRepository.GetUtvalgListWithAllReferences(Convert.ToInt32(request.ListId));
            if (!(utvalgListData.BasedOn > 0))
                throw new Exception("To disconnect list , list must be based on other list");
            if (utvalgListData == null)
            {
                throw new Exception("Please pass valid list id");

            }

            RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(utvalgListData);


            requestSaveUtvalgList.ParentListId = 0;
            requestSaveUtvalgList.userName = request.UserName;
            requestSaveUtvalgList.WasBasedOn = utvalgListData.BasedOn;
            requestSaveUtvalgList.BasedOn = 0;
            requestSaveUtvalgList.IsBasis = false;
            if (utvalgListData.OrdreType == 0)
            {
                requestSaveUtvalgList.Thickness = 0;
                requestSaveUtvalgList.Weight = 0;
            }
            var newListData = await _mediator.Send(requestSaveUtvalgList);

            Puma.Shared.UtvalgList listData = _mapper.Map<ResponseSearchUtvalgListSimpleById, Puma.Shared.UtvalgList>(newListData);
            listData.MemberUtvalgs = new List<Puma.Shared.Utvalg>();


            string prefix = "_" + DateTime.Now.ToString("ddMMyy") + "_" + DateTime.Now.ToString("HHmm");



            //Create new selection of each existin selection
            #region Old Code to save selections
            //foreach (var itemUtvalg in utvalgListData?.MemberUtvalgs)
            //{
            //    string utvalgName = await _utvalgListRepository.CreateNewNameWithSuffixForced(itemUtvalg.Name, 50, false, new List<string>(), prefix);

            //    RequestSaveUtvalg requestSaveUtvalg = new RequestSaveUtvalg();
            //    requestSaveUtvalg.utvalg = itemUtvalg;
            //    requestSaveUtvalg.utvalg.Name = utvalgName;
            //    requestSaveUtvalg.utvalg.KundeNummer = request.kundeNumber;
            //    requestSaveUtvalg.utvalg.ListId = newListData.ListId.ToString();
            //    requestSaveUtvalg.userName = request.userName;
            //    requestSaveUtvalg.utvalg.UtvalgId = 0;
            //    var utvalgData = await _mediator.Send(requestSaveUtvalg);

            //    listData.MemberUtvalgs.Add(_mapper.Map<ResponseSaveUtvalg, Puma.Shared.Utvalg>(utvalgData));
            //} 
            #endregion


            if (utvalgListData?.MemberUtvalgs?.Any() == true)
            {
                RequestListSaveUtvalgs requestListSaveUtvalgs = new RequestListSaveUtvalgs();
                requestListSaveUtvalgs.utvalgs = new List<Puma.Shared.Utvalg>();
                foreach (var itemUtvalg in utvalgListData?.MemberUtvalgs)
                {
                    string utvalgName = await utvalgListRepository.CreateNewNameWithSuffixForced(itemUtvalg.Name, 50, false, new List<string>(), prefix);

                    Puma.Shared.Utvalg utvalg = new();
                    utvalg = itemUtvalg;
                    utvalg.Name = utvalgName;
                    utvalg.KundeNummer = utvalgListData.KundeNummer;
                    utvalg.ListId = newListData.ListId.ToString();
                    utvalg.UtvalgId = 0;
                    utvalg.IsBasis = false;
                    if (utvalgListData.OrdreType == 0)
                    {
                        utvalg.Thickness = 0;
                        utvalg.Weight = 0;
                    }
                    //Note (30.03.2023): We have implemented tthis code as we have discussed with management when
                    ////we disconnect kamp list and if that kamp has order information then all of its memeber list and selection should have same information
                    utvalg.OrdreReferanse = utvalgListData.OrdreReferanse;
                    utvalg.OrdreStatus = utvalgListData.OrdreStatus;
                    utvalg.OrdreType = utvalgListData.OrdreType;
                    utvalg.Thickness = utvalgListData.Thickness;
                    utvalg.Weight = utvalgListData.Weight;
                    //var utvalgData = await _mediator.Send(requestSaveUtvalg);
                    requestListSaveUtvalgs.utvalgs.Add(utvalg);
                    //listData.MemberUtvalgs.Add(_mapper.Map<ResponseSaveUtvalg, Puma.Shared.Utvalg>(utvalgData));
                }
                requestListSaveUtvalgs.userName = request.UserName;

                List<ResponseSaveUtvalg> lstUtvalgResponse = await _mediator.Send(requestListSaveUtvalgs);
                //Set List name in utvalg as needed in response
                foreach (var itemUtvalg in lstUtvalgResponse)
                {
                    itemUtvalg.ListName = newListData.Name;
                }
                listData.MemberUtvalgs.AddRange(_mapper.Map<List<ResponseSaveUtvalg>, List<Puma.Shared.Utvalg>>(lstUtvalgResponse));
            }

            //Create Member list
            if (utvalgListData.MemberLists?.Any() == true)
            {
                listData.MemberLists = new List<Puma.Shared.UtvalgList>();
                foreach (var item in utvalgListData.MemberLists)
                {
                    listData.MemberLists.Add(await MemberListCreation(item, utvalgListData.KundeNummer, prefix, newListData.ListId, request.UserName, utvalgListData));
                }
            }


            //After everything is saved update its parent antall
            await utvalgListRepository.UpdateAntallInList(newListData.ListId);

            // // OEBS må få beskjed om å oppdatere utvalgsfordelingen for denne listen.
            List<UtvalgIdWrapper> idList = new List<UtvalgIdWrapper>();
            idList.Add(new UtvalgIdWrapper(request.ListId, UtvalgType.UtvalgList));
            // UtvalgServiceProxy.UtvalgServiceProxy.OppdaterUtvalgsfordelingFromDisconnectList(idList);
            await OppdaterUtvalgsfordelingAsync(idList, true);

            //var getNewListData = await _utvalgListRepository.GetUtvalgListWithAllReferences(newListData.ListId);

            // return _mapper.Map<Puma.Shared.UtvalgList, ResponseCreateCopyOfUtalgList>(listData);

            return _mapper.Map<Puma.Shared.UtvalgList, ResponseDisconnectList>(listData);




        }

        /// <summary>
        /// Oppdaters the utvalgsfordeling asynchronous.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="dueToChangesInBasisUtvalg">if set to <c>true</c> [due to changes in basis utvalg].</param>
        /// <exception cref="System.Exception">Feil ved kall av UtvalgService.OppdaterUtvalgsfordeling: " + ex.Message</exception>
        private async Task OppdaterUtvalgsfordelingAsync(IEnumerable<UtvalgIdWrapper> ids, bool dueToChangesInBasisUtvalg)
        {
            ECPumaController eCPumaController = new ECPumaController(logger, loggerFinn, loggerMappin, loggerConfig, loggerUtvalg, loggerreol,
                                                   loggerUtvalgList, loggerHentAntall, loggerUtvalgDetaljer, loggerMappingAntall, loggerOppdaterUtvalg, loggerkapasitet,
                                                   utvalgRepository, kapasitetRepository, utvalgListRepository, _iOEBSServiceRepository, null);

            if (ids == null || ids.Count() < 1) return;
            try
            {
                // Remove duplicate ids
                List<UtvalgIdWrapper> newIds = new List<UtvalgIdWrapper>();
                foreach (UtvalgIdWrapper id in ids)
                    if (!newIds.Any(i => i.Id == id.Id && i.TypeUtvalg == id.TypeUtvalg))
                        newIds.Add(id);

                OppdaterUtvalgsfordelingRequest request = new OppdaterUtvalgsfordelingRequest();
                request.UtvalgsIdListe = new UtvalgsIdListe();
                foreach (UtvalgIdWrapper id in newIds)
                {
                    DataAccessAPI.Models.UtvalgsId uid = new DataAccessAPI.Models.UtvalgsId();
                    uid.Id = id.Id;
                    if (id.TypeUtvalg == UtvalgType.Utvalg)
                        uid.Type = Models.Helper.UtvalgsTypeKode.Utvalg;
                    else
                        uid.Type = Models.Helper.UtvalgsTypeKode.Liste;
                    request.UtvalgsIdListe.Add(uid);
                }

                if (dueToChangesInBasisUtvalg)
                    await eCPumaController.OppdaterBasisUtvalgsfordeling(request);
                else
                    await eCPumaController.OppdaterUtvalgsfordeling(request);

            }
            catch (Exception ex)
            {
                throw new Exception("Feil ved kall av UtvalgService.OppdaterUtvalgsfordeling: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Members the list creation.
        /// </summary>
        /// <param name="utvalgList">The utvalg list.</param>
        /// <param name="kundeNumber">The kunde number.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="ParentListId">The parent list identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="utvalgListData">parent List Data.</param>
        public async Task<Puma.Shared.UtvalgList> MemberListCreation(Puma.Shared.UtvalgList utvalgList, string kundeNumber, string prefix, int ParentListId, string userName, Puma.Shared.UtvalgList utvalgListData)
        {
            Puma.Shared.UtvalgList memberListData = new Puma.Shared.UtvalgList();

            string utvalgListName = await utvalgListRepository.CreateNewNameWithSuffixForced(utvalgList.Name, 50, true, new List<string>(), prefix);

            RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(utvalgList);
            requestSaveUtvalgList.Name = utvalgListName;
            requestSaveUtvalgList.KundeNummer = kundeNumber;
            requestSaveUtvalgList.ParentListId = ParentListId;
            requestSaveUtvalgList.userName = userName;
            requestSaveUtvalgList.IsBasis = false;
            requestSaveUtvalgList.ListId = 0;
            if (utvalgList.OrdreType == 0)
            {
                requestSaveUtvalgList.Thickness = 0;
                requestSaveUtvalgList.Weight = 0;
            }
            //Note (30.03.2023): We have implemented tthis code as we have discussed with management when
            ////we disconnect kamp list and if that kamp has order information then all of its memeber list and selection should have same information
            requestSaveUtvalgList.OrdreReferanse = utvalgListData.OrdreReferanse;
            requestSaveUtvalgList.OrdreStatus = utvalgListData.OrdreStatus;
            requestSaveUtvalgList.OrdreType = utvalgListData.OrdreType;
            requestSaveUtvalgList.Thickness = utvalgListData.Thickness;
            requestSaveUtvalgList.Weight = utvalgListData.Weight;
            var listData = await _mediator.Send(requestSaveUtvalgList);

            memberListData = _mapper.Map<ResponseSearchUtvalgListSimpleById, Puma.Shared.UtvalgList>(listData);
            memberListData.MemberUtvalgs = new List<Puma.Shared.Utvalg>();



            if (utvalgList?.MemberUtvalgs?.Any() == true)
            {
                RequestListSaveUtvalgs requestListSaveUtvalgs = new RequestListSaveUtvalgs();
                requestListSaveUtvalgs.utvalgs = new List<Puma.Shared.Utvalg>();
                foreach (var itemUtvalg in utvalgList?.MemberUtvalgs)
                {
                    string utvalgName = await utvalgListRepository.CreateNewNameWithSuffixForced(itemUtvalg.Name, 50, false, new List<string>(), prefix);

                    Puma.Shared.Utvalg utvalg = new();

                    //RequestSaveUtvalg requestSaveUtvalg = new RequestSaveUtvalg();
                    utvalg = itemUtvalg;
                    utvalg.Name = utvalgName;
                    utvalg.KundeNummer = kundeNumber;
                    utvalg.ListId = listData.ListId.ToString();
                    utvalg.IsBasis = false;
                    if (utvalgList.OrdreType == 0)
                    {
                        utvalg.Thickness = 0;
                        utvalg.Weight = 0;
                    }
                    //Note (30.03.2023): We have implemented tthis code as we have discussed with management when
                    ////we disconnect kamp list and if that kamp has order information then all of its memeber list and selection should have same information
                    utvalg.OrdreReferanse = utvalgListData.OrdreReferanse;
                    utvalg.OrdreStatus = utvalgListData.OrdreStatus;
                    utvalg.OrdreType = utvalgListData.OrdreType;
                    utvalg.Thickness = utvalgListData.Thickness;
                    utvalg.Weight = utvalgListData.Weight;
                    // userName = userName;
                    utvalg.UtvalgId = 0;

                    requestListSaveUtvalgs.utvalgs.Add(utvalg);

                    //var utvalgData = await _mediator.Send(requestSaveUtvalg);

                    //memberListData.MemberUtvalgs.Add(_mapper.Map<ResponseSaveUtvalg, Puma.Shared.Utvalg>(utvalgData));

                }

                requestListSaveUtvalgs.userName = userName;

                List<ResponseSaveUtvalg> lstUtvalgResponse = await _mediator.Send(requestListSaveUtvalgs);


                //Set List name in utvalg as needed in response
                foreach (var itemUtvalg in lstUtvalgResponse)
                {
                    itemUtvalg.ListName = listData.Name;
                }

                memberListData.MemberUtvalgs.AddRange(_mapper.Map<List<ResponseSaveUtvalg>, List<Puma.Shared.Utvalg>>(lstUtvalgResponse));
            }

            if (utvalgList.MemberLists?.Any() == true)
            {
                memberListData.MemberLists = new List<Puma.Shared.UtvalgList>();
                foreach (var itemMemberLists in utvalgList.MemberLists)
                {
                    memberListData.MemberLists.Add(await MemberListCreation(itemMemberLists, kundeNumber, prefix, listData.ListId, userName, utvalgListData));
                }
            }

            return memberListData;
        }
    }
}
