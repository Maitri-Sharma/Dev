using MediatR;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestAddUtvalgsToNewList
    /// </summary>
    public class RequestAddUtvalgsToNewList : IRequest<ResponseAddUtvalgsToNewList>
    {
        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        /// <value>
        /// The name of the list.
        /// </value>
        public string ListName { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>
        /// The name of the customer.
        /// </value>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the customer no.
        /// </summary>
        /// <value>
        /// The customer no.
        /// </value>
        public string CustomerNo { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string Logo { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        
        [JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the utvalgs.
        /// </summary>
        /// <value>
        /// The utvalgs.
        /// </value>
        public List<Puma.Shared.Utvalg> utvalgs { get; set; }


    }
}
