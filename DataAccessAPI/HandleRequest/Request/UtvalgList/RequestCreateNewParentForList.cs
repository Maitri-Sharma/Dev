using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestCreateNewParentForList
    /// </summary>
    public class RequestCreateNewParentForList : IRequest<(ResponseGetUtvalgListWithAllReferences utvalgListData, string message)>
    {
        /// <summary>
        /// Gets or sets the list to be changed.
        /// </summary>
        /// <value>
        /// The list to be changed.
        /// </value>
        public Puma.Shared.UtvalgList ListToBeChanged { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the new name of the parent.
        /// </summary>
        /// <value>
        /// The new name of the parent.
        /// </value>
        public string NewParentName { get; set; }

        /// <summary>
        /// Gets or sets the new parent customer.
        /// </summary>
        /// <value>
        /// The new parent customer.
        /// </value>
        public string NewParentCustomer { get; set; }

        /// <summary>
        /// Gets or sets the new parent logo.
        /// </summary>
        /// <value>
        /// The new parent logo.
        /// </value>
        public string NewParentLogo { get; set; }

        /// <summary>
        /// Gets or sets the deleted lists.
        /// </summary>
        /// <value>
        /// The deleted lists.
        /// </value>
        public List<Puma.Shared.UtvalgList> DeletedLists { get; set; }
    }
}
