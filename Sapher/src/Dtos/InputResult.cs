namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class InputResult : Result
    {
        public IEnumerable<string> OutputMessagesIds { get; set; }
    }
}