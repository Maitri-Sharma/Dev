using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSearchUtvalgListSimpleByOrdreReferanse:  IRequest<List<ResponseSearchUtvalgListSimpleByOrdreReferanse>>
    {
        /// <summary>
        /// Gets or sets the ordre referanse.
        /// </summary>
        /// <value>
        /// The ordre referanse.
        /// </value>
        public string OrdreReferanse { get; set; }

        /// <summary>
        /// Gets or sets the type of the ordre.
        /// </summary>
        /// <value>
        /// The type of the ordre.
        /// </value>
        public string OrdreType { get; set; }


        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod SearchMethod { get; set; }
    }
}
