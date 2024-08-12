using DataAccessAPI.HandleRequest.Request.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// DeleteUtvalgListHandler
    /// </summary>
    public class DeleteUtvalgListHandler : IRequestHandler<RequestDeleteUtvalgList, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DeleteUtvalgListHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUtvalgListHandler" /> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <exception cref="System.ArgumentNullException">utvalgListRepository
        /// or
        /// logger</exception>
        public DeleteUtvalgListHandler(IUtvalgListRepository utvalgListRepository, ILogger<DeleteUtvalgListHandler> logger,
            IUtvalgRepository utvalgRepository)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));

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
        public async Task<bool> Handle(RequestDeleteUtvalgList request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListWithChildren from Repository");
            var utvalgList = await _utvalgListRepository.GetUtvalgListWithAllReferences(request.UtvalgListId);
            VerifyThatUtvalgListCanBeDeleted(utvalgList);
            if (utvalgList.BasedOn > 0)
            {
                _logger.LogDebug("Calling DeleteListData from Repository");
                await DeleteListData(utvalgList);
            }
            else
            {
                _logger.LogDebug("Calling DeleteUtvalgWithAllList from Repository");
                await DeleteUtvalgWithAllList(utvalgList, request.userName);
            }

            return true;
        }

        /// <summary>
        /// Deletes the list data.
        /// </summary>
        /// <param name="utvalgList">The utvalg list.</param>
        public async Task DeleteListData(Puma.Shared.UtvalgList utvalgList)
        {

            _logger.LogDebug("Calling DeleteCampaignList from Repository");
            await _utvalgListRepository.DeleteCampaignList(utvalgList.ListId, utvalgList.BasedOn);
        }

        /// <summary>
        /// Deletes the utvalg with all list.
        /// </summary>
        /// <param name="utvalgList">The utvalg list.</param>
        /// <param name="userName">Name of the user.</param>
        /// <exception cref="System.Exception">
        /// Kan ikke slette basisutvalgslista, da det finnes kampanjer basert på lista som det er gitt tilbud på.
        /// </exception>
        public async Task DeleteUtvalgWithAllList(Puma.Shared.UtvalgList utvalgList, string userName)
        {
            try
            {
                if (utvalgList.MemberLists?.Any() == true)
                {
                    foreach (var itemMemberList in utvalgList.MemberLists)
                    {
                        VerifyThatUtvalgListCanBeDeleted(itemMemberList);
                        foreach (var itemMemberUtvalg in itemMemberList.MemberUtvalgs)
                        {
                            VerifyThatUtvalgCanBeDeleted(itemMemberUtvalg);
                        }
                    }
                }

                foreach (var memberUtvalg in utvalgList.MemberUtvalgs)
                {
                    VerifyThatUtvalgCanBeDeleted(memberUtvalg);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(Constants.errMsgDeleteListWithChildElementsOrdreTilbud, ex);
            }

            try
            {
                List<long> ids = new List<long>();
                if (utvalgList.IsBasis)
                {
                    //Get List CampainData

                    _logger.LogDebug("Calling GetListsToRefreshDueToUpdateToBasisList from Repository");
                    var campData = await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisList(utvalgList);

                    if (campData.Count() > 0)
                    {
                        throw new Exception("Kan ikke slette basisutvalgslista, da det finnes kampanjer basert på lista som det er gitt tilbud på.");
                    }
                    //Get Parent CampainList Data
                    _logger.LogDebug("Calling GetListsToRefreshDueToUpdateToBasisListChild from Repository");
                    ids.AddRange(await _utvalgListRepository.GetListsToRefreshDueToUpdateToBasisListChild(utvalgList));

                }
                _logger.LogDebug("Calling DeleteUtvalgList from Repository");
                _ = await _utvalgListRepository.DeleteUtvalgList(utvalgList.ListId, true, userName);
                _logger.LogDebug("Calling SendBasisUtvalgFordelingToQue from Repository");
                await _utvalgListRepository.SendBasisUtvalgFordelingToQue(ids,"L");

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }
        }


        /// <summary>
        /// Verifies the that utvalg list can be deleted.
        /// </summary>
        /// <param name="ul">The ul.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        public void VerifyThatUtvalgListCanBeDeleted(Puma.Shared.UtvalgList ul)
        {
            if (ul.OrdreType == Puma.Shared.PumaEnum.OrdreType.O) throw new Exception(Constants.errMsgDeleteListWithOrdre);
            if (ul.OrdreType == Puma.Shared.PumaEnum.OrdreType.T) throw new Exception(Constants.errMsgDeleteListWithTilbud);
        }

        /// <summary>
        /// Verifies the that utvalg can be deleted.
        /// </summary>
        /// <param name="utvalgData">The utvalg data.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        public void VerifyThatUtvalgCanBeDeleted(Puma.Shared.Utvalg utvalgData)
        {
            if (utvalgData.OrdreType == Puma.Shared.PumaEnum.OrdreType.O) throw new Exception(Constants.errMsgDeleteUtvalgWithOrdre);
            if (utvalgData.OrdreType == Puma.Shared.PumaEnum.OrdreType.T) throw new Exception(Constants.errMsgDeleteUtvalgWithTilbud);
        }

        
    }
}
