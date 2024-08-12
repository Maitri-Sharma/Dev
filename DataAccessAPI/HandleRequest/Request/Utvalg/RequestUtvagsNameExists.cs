using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestUtvagsNameExists : IRequest<bool>
    {
        /// <summary>
        /// List of utvalg names
        /// </summary>
        public List<string> utvalgNames { get; set; }
    }
}
