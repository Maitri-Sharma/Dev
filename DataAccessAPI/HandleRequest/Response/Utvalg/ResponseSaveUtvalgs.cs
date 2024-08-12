namespace DataAccessAPI.HandleRequest.Response.Utvalg
{
    /// <summary>
    /// ResponseSaveUtvalgs
    /// </summary>
    public class ResponseSaveUtvalgs
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the utvalg.
        /// </summary>
        /// <value>
        /// The utvalg.
        /// </value>
        public ResponseSaveUtvalg utvalg { get; set; }
    }
}
