namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;
    using Persistence.Model;

    public interface IHandlesCompensation { }

    // TODO Maybe mix Compensation and Success and return the OutputState instead of true and false in "IsSuccess"
    public interface IHandlesCompensation<in T> : IHandlesCompensation where T : class
    {
        Task<Result> Compensate(T message, SapherStepData sapherData);
    }
}