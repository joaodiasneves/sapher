namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;

    public interface IHandlesInput<in T> where T : class
    {
        Task<InputResult> Execute(T message, MessageSlip messageSlip);
    }
}