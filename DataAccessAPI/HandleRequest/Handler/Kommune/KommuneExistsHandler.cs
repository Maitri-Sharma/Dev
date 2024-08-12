﻿
using DataAccessAPI.HandleRequest.Request.Kommune;
using MediatR;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kommune
{
    /// <summary>
    /// KommuneExistsHandler
    /// </summary>
    public class KommuneExistsHandler : IRequestHandler<RequestKommuneExists, bool>
    {
        /// <summary>
        /// The kommune repository
        /// </summary>
        private readonly IKommuneRepository _kommuneRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="KommuneExistsHandler"/> class.
        /// </summary>
        /// <param name="kommuneRepository">The kommune repository.</param>
        /// <exception cref="System.ArgumentNullException">kommuneRepository</exception>
        public KommuneExistsHandler(IKommuneRepository kommuneRepository)
        {
            _kommuneRepository = kommuneRepository ?? throw new ArgumentNullException(nameof(kommuneRepository));
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
        public async Task<bool> Handle(RequestKommuneExists request, CancellationToken cancellationToken)
        {
            return await _kommuneRepository.KommuneExists(request.Kommunenavn);
        }
    }
}
