using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// UpdateAllowDoubleHandler
    /// </summary>
    public class UpdateAllowDoubleHandler : IRequestHandler<RequestUpdateAllowDouble, bool>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<UpdateAllowDoubleHandler> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAllowDoubleHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <exception cref="System.ArgumentNullException">utvalgListRepository</exception>
        public UpdateAllowDoubleHandler(IUtvalgListRepository utvalgListRepository, ILogger<UpdateAllowDoubleHandler> logger)
        {
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> Handle(RequestUpdateAllowDouble request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling UpdateAllowDouble from Repository");
            await _utvalgListRepository.UpdateAllowDouble(request.ListId, request.AllowDouble);
            return true;
        }
    }
}
