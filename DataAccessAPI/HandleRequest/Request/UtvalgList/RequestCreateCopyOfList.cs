using MediatR;
using System;
using Puma.Shared;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestCreateCopyOfList
    /// </summary>
    public class RequestCreateCopyOfList : IRequest<ResponseCreateCopyOfUtalgList>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public long ListId { get; set; }

        /// <summary>
        /// Gets or sets the selection criteria.
        /// </summary>
        /// <value>
        /// The selection criteria.
        /// </value>
        public PumaEnum.SelectionCriteria SelectionCriteria { get; set; }

        /// <summary>
        /// Gets or sets the kunde number.
        /// </summary>
        /// <value>
        /// The kunde number.
        /// </value>
        public string kundeNumber { get; set; }

        /// <summary>
        /// Gets or sets the custom text.
        /// </summary>
        /// <value>
        /// The custom text.
        /// </value>
        public string CustomText { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string userName { get; set; }

        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        /// <value>
        /// The name of the list.
        /// </value>
        public string ListName { get; set; }
    }
}
