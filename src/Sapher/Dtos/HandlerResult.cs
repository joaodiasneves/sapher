namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class HandlerResult
    {
        internal string ExecutedHandlerName { get; set; }

        public IDictionary<string,string> DataToPersist { get; set; }
    }
}