using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// SaveManyUtvalgsHandler
    /// </summary>
    public class SaveManyUtvalgsHandler : IRequestHandler<RequestSaveManyUtvalgs, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;

        /// <summary>
        /// The utvalg list repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveManyUtvalgsHandler> _logger;

        public SaveManyUtvalgsHandler(IUtvalgRepository utvalgRepository, IUtvalgListRepository utvalgListRepository, ILogger<SaveManyUtvalgsHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
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
        public async Task<bool> Handle(RequestSaveManyUtvalgs request, CancellationToken cancellationToken)
        {
            //_logger.LogDebug("Handler called for save many utvalgs with request" + JsonConvert.SerializeObject(request));
            List<UtvalgIdWrapper> ids = new List<UtvalgIdWrapper>();
            try
            {
                foreach (Puma.Shared.Utvalg utv in request.UtvalgList.GetAllUtvalgs())
                {
                    await SaveUtvalgInternal(utv, request.UserName, false, 0, ids);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(Puma.DataLayer.BusinessEntity.Constants.errMsgSaveUtvalgError + " Mld:" + ex.ToString());
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgSaveUtvalgError, ex);

            }
            finally
            {
                //SendBasisUtvalgFordelingToQue for utvagls
                await _utvalgListRepository.SendBasisUtvalgFordelingToQue(ids.Where(x => x.TypeUtvalg == UtvalgType.Utvalg)?.Select(x => Convert.ToInt64(x.Id)).ToList(), "U");

                //SendBasisUtvalgFordelingToQue for utvaglsList
                await _utvalgListRepository.SendBasisUtvalgFordelingToQue(ids.Where(x => x.TypeUtvalg == UtvalgType.UtvalgList)?.Select(x => Convert.ToInt64(x.Id)).ToList(), "L");
            }

            return true;
        }

        private async Task SaveUtvalgInternal(Puma.Shared.Utvalg utv, string userName, bool skipHistory, int utvalgListId, List<UtvalgIdWrapper> oebsNotifications)
        {
            if (utv.OrdreType == OrdreType.O) throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgSaveUtvalgWithOrdre);
            if (utv.TotalAntall == 0) throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgHasNoReceivers);
            if (utv.IsBasis && utv.UtvalgId > 0)
            {
                oebsNotifications.AddRange(await _utvalgRepository.GetUtvalgsToRefreshDueToUpdate(utv));
                if (utv.List != null)
                {
                    List<long> utvalgIdWrappers = await _utvalgListRepository.GetListsToRefreshDueToUpdate(utv.List);

                    foreach (var itemIdWrapper in utvalgIdWrappers)
                    {
                        oebsNotifications.Add(new UtvalgIdWrapper()
                        {
                            TypeUtvalg = UtvalgType.UtvalgList,
                            Id = Convert.ToInt32(itemIdWrapper)
                        });
                    }
                }
            }
            await _utvalgRepository.SaveUtvalgData(utv, userName, false, skipHistory, utvalgListId);
        }
    }
}
