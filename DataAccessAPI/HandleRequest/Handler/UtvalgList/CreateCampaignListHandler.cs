using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// CreateCampaignListHandler
    /// </summary>
    public class CreateCampaignListHandler : IRequestHandler<RequestCreateCampaignList, ResponseCreateCampaignList>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;


        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CreateCampaignListHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCampaignListHandler" /> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">utvalgListRepository
        /// or
        /// mapper
        /// or
        /// mediator
        /// or
        /// logger</exception>
        public CreateCampaignListHandler(IUtvalgListRepository utvalgListRepository, IMapper mapper, IMediator mediator,
            ILogger<CreateCampaignListHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<ResponseCreateCampaignList> Handle(RequestCreateCampaignList request, CancellationToken cancellationToken)
        {
            //Get list Data 

            var listData = await _utvalgListRepository.GetUtvalgListWithAllReferences(request.ListId);

            if (listData == null)
                throw new Exception("Please pass valid data");



            RequestSaveUtvalgList requestSave = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(listData);

            requestSave.Name = request.Name;
            requestSave.BasedOn = request.BasedOn;
            requestSave.IsBasis = false;
            requestSave.userName = request.userName;
            requestSave.Antall = listData.Antall;
            requestSave.ParentListId = 0;


            requestSave.ListId = 0;
            //Call handler to save utvalg List
            var utvalgListData = await _mediator.Send(requestSave);

            if (utvalgListData == null)
            {
                _logger.LogError("Error during save utvalg list with request : " + JsonConvert.SerializeObject(requestSave) + "");
                throw new Exception(message: "Error during save utvalg list");
            }

            //After list created sucessfully update antall in DB

            await UpdateAntall(utvalgListData.ListId, request.Antall);

            if (listData != null)
            {
                utvalgListData.MemberLists = listData.MemberLists;
                utvalgListData.MemberUtvalgs = listData.MemberUtvalgs;
            }

            utvalgListData.Antall = Convert.ToInt64(request.Antall);


            return _mapper.Map<ResponseSearchUtvalgListSimpleById, ResponseCreateCampaignList>(utvalgListData);
        }

        /// <summary>
        /// Update antall in utvalg list
        /// </summary>
        /// <param name="utvalgListId"></param>
        /// <param name="antall"></param>
        /// <returns></returns>
        public async Task UpdateAntall(int utvalgListId, double antall)
        {
            await _utvalgListRepository.UpdateAntallForList(utvalgListId, antall);
        }
    }
}
