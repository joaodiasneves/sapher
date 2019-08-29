namespace Sapher.Configuration
{
    /// <summary>
    /// SapherStepConfigurator provides methods to configure a SapherStep
    /// </summary>
    public interface ISapherStepConfigurator
    {

        /// <summary>
        /// Method to add a ResponseHandler to a SapherStep configuration.
        /// </summary>
        /// <typeparam name="T">The implementation Type of the response handler to add</typeparam>
        /// <returns>Returns the used SapherStepConfigurator</returns>
        ISapherStepConfigurator AddResponseHandler<T>();
    }
}