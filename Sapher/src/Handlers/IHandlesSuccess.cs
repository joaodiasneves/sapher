namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;
    using Persistence.Model;

    public interface IHandlesSuccess
    {
        //bool HandlesFinalMessage { get; set; } TODO - Think about how to do this.
    }

    public interface IHandlesSuccess<in T> : IHandlesSuccess where T : class
    {
        Task<Result> Execute(T message, SapherStepData data);
    }
}