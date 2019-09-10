namespace Sapher.Dtos
{
    /// <summary>
    /// DTO to provide information regarding the result of a resposne handler execution
    /// </summary>
    public class ResponseResult : HandlerResult
    {
        /// <summary>
        /// The result State of the resposne handler execution
        /// </summary>
        public ResponseResultState State { get; set; }
    }
}