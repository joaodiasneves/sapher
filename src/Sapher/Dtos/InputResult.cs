namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class InputResult : HandlerResult
    {
        public InputResultState State { get; set; }

        public IEnumerable<string> OutputMessagesIds { get; set; }
    }
}