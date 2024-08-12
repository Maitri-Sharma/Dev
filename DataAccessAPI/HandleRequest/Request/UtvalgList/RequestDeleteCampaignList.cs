using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestDeleteCampaignList
    /// </summary>
    public class RequestDeleteCampaignList : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public int listId { get; set; }

        /// <summary>
        /// Gets or sets the based on.
        /// </summary>
        /// <value>
        /// The based on.
        /// </value>
        public int BasedOn { get; set; }
    }
}
