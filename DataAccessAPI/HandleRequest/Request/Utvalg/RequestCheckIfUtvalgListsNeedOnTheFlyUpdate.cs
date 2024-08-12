using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestCheckIfUtvalgListsNeedOnTheFlyUpdate : IRequest<List<int>>
    {
        /// <summary>
        /// Gets or sets the utvalg list i ds.
        /// </summary>
        /// <value>
        /// The utvalg list i ds.
        /// </value>
       public int[] utvalgListIDs { get; set; }

    }
}
