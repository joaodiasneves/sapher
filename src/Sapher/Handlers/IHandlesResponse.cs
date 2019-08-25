namespace Sapher.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;

    public interface IHandlesResponse<in T> where T : class
    {

        Task<ResponseResult> Execute(T message, MessageSlip messageSlip, IDictionary<string, string> previouslyPersistedData);
    }
}