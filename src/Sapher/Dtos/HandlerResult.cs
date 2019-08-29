namespace Sapher.Dtos
{
    using System.Collections.Generic;

    /// <summary>
    /// DTO to provide information regarding the result of a handler execution
    /// </summary>
    public class HandlerResult
    {

        /// <summary>
        /// Name of the Executed Handler
        /// </summary>
        public string ExecutedHandlerName { get; internal set; }

        /// <summary>
        /// A dictionary with Data defined to be persisted in order to be available for future processing of a Step instance (compensation actions or others).
        /// </summary>
        public IDictionary<string, string> DataToPersist { get; set; }
    }
}