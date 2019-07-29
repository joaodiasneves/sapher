namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;

    public interface IHandlesResponse<in T> where T : class
    {
        //bool HandlesFinalMessage { get; set; } TODO - Think about how to do this.

        Task<ResponseResult> Execute(T message, object previouslyPersistedData);
    }
}