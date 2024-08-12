using DataAccessAPI.HandleRequest.Request.AvisDekning;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.AvisDekning
{
    public class AvisExistsHandler : IRequestHandler<RequestAvisExists, bool>
    {
        private readonly IAvisDekningRepository _avisDekningRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<AvisExistsHandler> _logger;

        public AvisExistsHandler(IAvisDekningRepository avisDekningRepository_, ILogger<AvisExistsHandler> logger)
        {
            _avisDekningRepository = avisDekningRepository_ ?? throw new ArgumentNullException(nameof(avisDekningRepository_));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RequestAvisExists request, CancellationToken cancellationToken)
        {
            return await _avisDekningRepository.AvisExists(request.utgave);
        }
    }
}
