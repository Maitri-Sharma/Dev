using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;
using DataAccessAPI.HandleRequest.Response.UtvalgList;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestChangeParentListOfList
    /// </summary>
    public class RequestChangeParentListOfList : IRequest<(ResponseGetUtvalgListWithAllReferences utvalgListData,string message)>
    {
        /// <summary>
        /// Gets or sets the list to be changed.
        /// </summary>
        /// <value>
        /// The list to be changed.
        /// </value>
        public Puma.Shared.UtvalgList ListToBeChanged { get; set; }

        /// <summary>
        /// Gets or sets the new parent list.
        /// </summary>
        /// <value>
        /// The new parent list.
        /// </value>
        public Puma.Shared.UtvalgList NewParentList { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        
        [JsonIgnore]
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the deleted lists.
        /// </summary>
        /// <value>
        /// The deleted lists.
        /// </value>
        public List<Puma.Shared.UtvalgList> DeletedLists { get; set; }
    }
}
