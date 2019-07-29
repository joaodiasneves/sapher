namespace Sapher.Dtos
{
    public class HandlerResult
    {
        internal string ExecutedHandlerName { get; set; }

        public object DataToPersist { get; set; }
    }
}