namespace Sapher
{
    public partial class SapherStep
    {
        public void Init()
        {
            this.StepName = this.configuration.StepName;
            this.inputMessageType = this.configuration.InputMessageType;
            this.inputHandler = this.configuration.InputHandler;
            this.dataRepository = this.configuration.DataRepository;

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