using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Puma.DataLayer.BusinessEntity;
using Puma.Shared;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// DeleteUtvalgByUtvalgIdHandler
    /// </summary>
    public class DeleteUtvalgByUtvalgIdHandler : IRequestHandler<RequestDeleteUtvalgByUtvalgId, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DeleteUtvalgByUtvalgIdHandler> _logger;

        /// <summary>
        /// The utvalg list repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUtvalgByUtvalgIdHandler" /> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <exception cref="System.ArgumentNullException">utvalgRepository
        /// or
        /// logger
        /// or
        /// utvalgListRepository</exception>
        public DeleteUtvalgByUtvalgIdHandler(IUtvalgRepository utvalgRepository, ILogger<DeleteUtvalgByUtvalgIdHandler> logger, IUtvalgListRepository utvalgListRepository)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Exception">
        /// </exception>
        public async Task<bool> Handle(RequestDeleteUtvalgByUtvalgId request, CancellationToken cancellationToken)
        {
            //First get utvalg data
            _logger.LogDebug("Calling GetUtvalg from Repository");
            Puma.Shared.Utvalg utvalg = await _utvalgRepository.GetUtvalg(request.UtvalgId);
            if (utvalg == null)
            {
                _logger.LogError(Constants.errMsgDeleteUtvalgError + " Mld: No Data Found");
                throw new Exception(Constants.errMsgDeleteUtvalgError);
            }

            if (utvalg != null)
            {
                if (utvalg.OrdreType == Puma.Shared.PumaEnum.OrdreType.O)
                {
                    throw new Exception(Constants.errMsgDeleteUtvalgWithOrdre) { };
                }
                else if (utvalg.OrdreType == Puma.Shared.PumaEnum.OrdreType.T)
                {
                    throw new Exception(Constants.errMsgDeleteUtvalgWithTilbud) { };
                }

                //Check campaing list exist for this utvalg or not

                if (await CheckCampaignList(utvalg))
                {
                    throw new Exception(Constants.errMsgCampaingListExists) { };
                }

                //If all above conditions working fine delete utvalg
                _logger.LogDebug("Calling DeleteUtvalg from Repository");
                await _utvalgRepository.DeleteUtvalg(request.UtvalgId, request.UserName);
                if (!string.IsNullOrWhiteSpace(utvalg.ListId))
                {
                    _logger.LogDebug("Calling GetUtvalgList from Repository");
                    if (Convert.ToInt32(utvalg.ListId) > 0)
                    {
                        Puma.Shared.UtvalgList utvList = await _utvalgListRepository.GetUtvalgList(Convert.ToInt32(utvalg.ListId));
                        if (utvList != null)
                        {
                            await DeleteUpdatedListIfEmpty(utvList, request.UserName);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks the campaign list.
        /// </summary>
        /// <param name="utvlgData">The utvlg data.</param>
        /// <returns></returns>
        public async Task<bool> CheckCampaignList(Puma.Shared.Utvalg utvlgData)
        {
            if (utvlgData.IsBasis && utvlgData.UtvalgId > 0)
            {
                if (utvlgData.UtvalgsBasedOnMe == null)
                {
                    _logger.LogDebug("Calling GetUtvalgCampaigns from Repository");
                    utvlgData.UtvalgsBasedOnMe = await _utvalgRepository.GetUtvalgCampaigns(utvlgData.UtvalgId);
                }

                if (utvlgData.UtvalgsBasedOnMe?.Any() == true)
                {
                    return utvlgData.UtvalgsBasedOnMe.Where(x => x.OrdreType == Puma.Shared.PumaEnum.OrdreType.T).Any();
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes the updated list if empty.
        /// </summary>
        /// <param name="oldParentList">The old parent list.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUpdatedListIfEmpty(Puma.Shared.UtvalgList oldParentList, string userName)
        {
            List<long> ids = new List<long>();
            if (oldParentList.IsBasis)
            {
                //Get List CampainData
                _logger.LogDebug("Calling GetListsToRefreshDueToUpdateToBasisList from Repository");
                ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisList(oldParentList));
                _logger.LogDebug("Calling GetListsToRefreshDueToUpdateToBasisListChild from Repository");
                ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisListChild(oldParentList));

                //If found any data call for SendBasisUtvalgFordelingToQue
                _logger.LogDebug("Calling SendBasisUtvalgFordelingToQue from Repository");
                await _utvalgListRepository.SendBasisUtvalgFordelingToQue(ids, "L");
            }
            if (ids.Count == 0)
            {
                _logger.LogDebug("Calling CheckAndDeleteUtvalgListIfEmpty from Repository");
                await _utvalgListRepository.CheckAndDeleteUtvalgListIfEmpty(oldParentList.ListId, userName);
            }

            return true;
        }

    }
}
