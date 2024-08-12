using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestCheckIfUtvalgsNeedOnTheFlyUpdate
    /// </summary>
    public class RequestCheckIfUtvalgListsDistributionIsToClose : IRequest<List<int>>
    {
        /// <summary>
        /// Gets or sets the idus.
        /// </summary>
        /// <value>
        /// The idus.
        /// </value>
        public int[] Idus { get; set; }
    }
}
