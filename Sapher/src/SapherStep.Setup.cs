namespace Sapher
{
    public partial class SapherStep
    {
        public void Init()
        {
            this.StepName = this.configuration.StepName;
            this.inputMessageType = this.configuration.InputMessageType;
            this.inputHandlerType = this.configuration.InputHandlerType;
            this.dataRepository = this.configuration.DataRepository;
            this.serviceCollection = this.configuration.ServiceCollection;

            this.SetupResponseHandlers();
        }

        internal void SetupResponseHandlers()
        {
            if (this.configuration.ResponseHandlers?.Count > 0)
            {
                this.responseHandlers = this.configuration.ResponseHandlers;
            }
        }
    }
}