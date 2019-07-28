namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;
    using Persistence.Model;

    public interface IHandlesResponse
    {
        //bool HandlesFinalMessage { get; set; } TODO - Think about how to do this.
    }

    public interface IHandlesResponse<in T> : IHandlesResponse where T : class
    {
        Task<ResponseResult> Execute(T message, SapherStepData data);
    }
}