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
namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class DeleteFromUtvalgOldReolHandler : IRequestHandler<RequestDeleteFromUtvalgOldReol, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DeleteFromUtvalgOldReolHandler> _logger;

        public DeleteFromUtvalgOldReolHandler(IUtvalgRepository utvalgRepository, ILogger<DeleteFromUtvalgOldReolHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RequestDeleteFromUtvalgOldReol request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling DeleteFromUtvalgOldReol from Repository");
            await _utvalgRepository.DeleteFromUtvalgOldReol(request.UtvalgId);
            return true;
        }
    }
}
