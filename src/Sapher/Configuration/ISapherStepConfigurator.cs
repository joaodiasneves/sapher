namespace Sapher.Configuration
{
    public interface ISapherStepConfigurator
    {
        ISapherStepConfigurator AddResponseHandler<T>();
    }
}