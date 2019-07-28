namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class InputResult : Result
    {
        public InputResultState State { get; set; }

        public IEnumerable<string> OutputMessagesIds { get; set; }
    }
}