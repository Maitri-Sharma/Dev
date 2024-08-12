using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSearchUtvalgListByUserID: IRequest<List<ResponseSearchUtvalgListByUserID>>
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod SearchMethod { get; set; }
    }
}
