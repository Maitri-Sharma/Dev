//using AutoMapper;
//using DataAccessAPI.HandleRequest.Request.Utvalg;
//using DataAccessAPI.HandleRequest.Response.Utvalg;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using Puma.DataLayer.DatabaseModel.KspuDB;
//using Puma.Infrastructure.Interface.KsupDB;
//using Puma.Shared;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DataAccessAPI.HandleRequest.Handler.Utvalg
//{
//    /// <summary>
//    ///SelectionDistributionHandler
//    /// </summary>
//    public class AddSelectionDistributionHandler : IRequestHandler<RequestAddSelectionDistribution, bool>
//    {
//        /// <summary>
//        /// The utvalg repository
//        /// </summary>
//        private readonly ISelectionDistributionRepository _selectionDistributionRepository;

//        private readonly IKspUDBUnitOfWork _kspUDBUnitOfWork;

//        private readonly ILogger<AddSelectionDistributionHandler> _logger;

//        public AddSelectionDistributionHandler(ISelectionDistributionRepository selectionDistributionRepository,
//            IKspUDBUnitOfWork kspUDBUnitOfWork, ILogger<AddSelectionDistributionHandler> logger)
//        {
//            _selectionDistributionRepository = selectionDistributionRepository ?? throw new ArgumentNullException(nameof(selectionDistributionRepository));
//            _kspUDBUnitOfWork = kspUDBUnitOfWork ?? throw new ArgumentNullException(nameof(kspUDBUnitOfWork));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

//        }



//        /// <summary>
//        /// Handles a request
//        /// </summary>
//        /// <param name="request">The request</param>
//        /// <param name="cancellationToken">Cancellation token</param>
//        /// <returns>
//        /// Response from the request
//        /// </returns>
//        /// <exception cref="System.NotImplementedException"></exception>
//        public async Task<bool> Handle(RequestAddSelectionDistribution request, CancellationToken cancellationToken)
//        {
//            if (request.SelectionId > 0)
//            {
//                try
//                {
//                    selectionDistribution selectionDistributionToAdd = new selectionDistribution()
//                    {
//                        CreatedDate = DateTime.UtcNow,
//                        IsDataPostedToOEBS = false,
//                        IsOrderStatusUpdated = false,
//                        Selectionid = request.SelectionId,
//                        Id = await _selectionDistributionRepository.GetMaxId()
//                    };

//                    _selectionDistributionRepository.Add(selectionDistributionToAdd);
//                    await _kspUDBUnitOfWork.Commit();
//                }
//                catch (Exception ex)
//                {

//                    _logger.LogError(ex, ex.Message);
//                }
//            }

//            return true;
//        }

       
//    }
//}
