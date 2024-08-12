using DataAccessAPI.HandleRequest.Response.UtvalgSaver;
using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAccessAPI.HandleRequest.Request.UtvalgSaver
{
    /// <summary>
    /// RequestUtvalgSaver
    /// </summary>
    public class RequestUtvalgSaver : IRequest<string>
    {

        /// <summary>
        /// Gets or sets the working list entries.
        /// </summary>
        /// <value>
        /// The working list entries.
        /// </value>
        public List<Puma.Shared.WorkingListEntry> workingListEntries { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string userName { get; set; }
    }

}
