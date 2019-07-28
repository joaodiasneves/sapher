namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;

    public interface IHandlesInput { }

    public interface IHandlesInput<in T> : IHandlesInput where T : class
    {
        Task<InputResult> Execute(T message);
    }
}