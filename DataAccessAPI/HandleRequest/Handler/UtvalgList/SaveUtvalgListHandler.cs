using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
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
    public class SaveUtvalgListHandler : IRequestHandler<RequestSaveUtvalgList, ResponseSearchUtvalgListSimpleById>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveUtvalgListHandler> _logger;

        private readonly IMapper _mapper;

        public SaveUtvalgListHandler(IUtvalgListRepository utvalgListRepository, ILogger<SaveUtvalgListHandler> logger,  IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResponseSearchUtvalgListSimpleById> Handle(RequestSaveUtvalgList request, CancellationToken cancellationToken)
        {
            ResponseSearchUtvalgListSimpleById response = null;
            if (request.OrdreType == OrdreType.O) throw new Exception(Constants.errMsgSaveUtvalgWithOrdre);



            // Finner evt parentlistid
            _logger.LogDebug("Calling GetParentUtvalgListId from Repository");

            Puma.Shared.UtvalgList utvalgListData;
            utvalgListData = _mapper.Map<RequestSaveUtvalgList, Puma.Shared.UtvalgList>(request);
            if (request.ParentListId > 0)
            {
                utvalgListData.ParentList = new Puma.Shared.UtvalgList() { ListId = request.ParentListId };
            }
            _logger.LogDebug("Calling SaveUtvalgListData from Repository");
            long utvalgListId = await _utvalgListRepository.SaveUtvalgListData(utvalgListData, request.userName);

            // request.AntallWhenLastSaved = request.Antall;
            _logger.LogDebug("Calling GetUtvalgListSimple from Repository");
            utvalgListData = await _utvalgListRepository.GetUtvalgListSimple(Convert.ToInt32(utvalgListId));

            response = _mapper.Map<Puma.Shared.UtvalgList, ResponseSearchUtvalgListSimpleById>(utvalgListData);


            await GetListsToRefreshDueToUpdate(utvalgListData);

            return response;
        }


        private async Task GetListsToRefreshDueToUpdate(Puma.Shared.UtvalgList utvalgList)
        {
            List<long> ids = new List<long>();
            if (utvalgList.IsBasis)
            {

                ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisList(utvalgList));



                ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisListChild(utvalgList));

            }

            await _utvalgListRepository.SendBasisUtvalgFordelingToQue(ids, "L");



        }

    }
}


