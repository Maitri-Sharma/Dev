using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
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
    public class SaveUtvalgListDistributionDataHandler : IRequestHandler<RequestSaveUtvalgListDistributionData, bool>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="utvalgListRepository"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public SaveUtvalgListDistributionDataHandler(IUtvalgListRepository utvalgListRepository, ILogger<SaveUtvalgListHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// Handle method
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<bool> Handle(RequestSaveUtvalgListDistributionData request, CancellationToken cancellationToken)
        {
            // Finner evt parentlistid

            Puma.Shared.UtvalgList utvalgListData;
            utvalgListData = _mapper.Map<RequestSaveUtvalgListDistributionData, Puma.Shared.UtvalgList>(request);

            _logger.LogDebug("Calling UpdateUtvalgListDistributionInfo from Repository");
            _ = await _utvalgListRepository.UpdateUtvalgListDistributionInfo(utvalgListData, request.userName);

            //If there is list of routes that needs to delete
            if (request.ruteId?.Any() == true && request.ruteId.Count > 0)
            {
                int basedOnId = await GetBasedOnId(request.ListId);
                if (basedOnId == 0)
                    basedOnId = request.ListId;

                #region Delete routes info from selections 
                await DeleteSelectionRoutes(basedOnId, request.ruteId);
                #endregion

                //After deleting routes need to update antal count in selection and list both
                await UpdateAntallInformation(basedOnId);
            }
            return true;
        }
        /// <summary>
        /// Delete routes from selection
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="routeIds"></param>
        /// <returns></returns>
        public async Task DeleteSelectionRoutes(long Id, List<long> routeIds)
        {
            //First fetch selection Ids beased on List Id

            List<long> utvalgIds = _utvalgListRepository.GetUtvalgIdsFromListId(Id);
            if (utvalgIds?.Any() == true)
            {
                //call method to delete routes from utvalg
                await _utvalgListRepository.DeleteRoutesOfUtvalgs("" + String.Join(", ", utvalgIds) + "", "" + String.Join(", ", routeIds) + "");
            }
        }

        public async Task UpdateAntallInformation(long listId)
        {
            //First fetch all details of list
            var listData = await _utvalgListRepository.GetUtvalgListWithAllReferences(Convert.ToInt32(listId));
            if (listData != null)
            {
                //First update antall of List
                await _utvalgListRepository.UpdateAntallInformation("L", listData.ListId, listData.Antall);

                //Update antall in member utvalg
                if (listData.MemberUtvalgs?.Any() == true)
                {
                    await UpdateAntallInformationOfUtvalgs(listData.MemberUtvalgs);
                }

                //Update antall for parent list

                if (listData.ParentList != null)
                {
                    await _utvalgListRepository.UpdateAntallInformation("L", listData.ParentList.ListId, listData.ParentList.Antall);
                }

                if (listData.MemberLists?.Any() == true)
                {
                    //Update antall information for Memeber list
                    foreach (var itemMemeberList in listData.MemberLists)
                    {
                        await _utvalgListRepository.UpdateAntallInformation("L", itemMemeberList.ListId, itemMemeberList.Antall);
                    }

                    //Update antall for all memeber utvalg of all memberlist

                    await UpdateAntallInformationOfUtvalgs(listData.MemberLists.SelectMany(d => d.MemberUtvalgs).ToList());
                }
            }
        }

        public async Task UpdateAntallInformationOfUtvalgs(List<Puma.Shared.Utvalg> utvalg)
        {

            foreach (var itemMemberUtvalg in utvalg)
            {
                await _utvalgListRepository.UpdateAntallInformation("U", itemMemberUtvalg.UtvalgId, itemMemberUtvalg.TotalAntall);
            }
        }



        /// <summary>
        /// if List is based on then it will return based on id or else it will return 0 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task<int> GetBasedOnId(int listId)
        {

            Puma.Shared.UtvalgList l = await _utvalgListRepository.GetUtvalgListNoChild(listId);
            if (l.BasedOn > 0)
                return l.BasedOn;
            return 0;

        }
    }

}
