namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;

    public interface IHandlesStepInput { }

    public interface IHandlesStepInput<in T> : IHandlesStepInput where T : class
    {
        Task<InputResult> Execute(T message);
    }
}