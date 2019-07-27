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

            this.SetupSuccessHandlers();
            this.SetupCompensationHandlers();
        }

        internal void SetupSuccessHandlers()
        {
            this.successHandlers.Clear();

            if (this.configuration.SuccessHandlers != null && this.configuration.SuccessHandlers.Count > 0)
            {
                this.successHandlers = this.configuration.SuccessHandlers;
            }
        }

        internal void SetupCompensationHandlers()
        {
            this.successHandlers.Clear();

            if (this.configuration.CompensationHandlers != null && this.configuration.CompensationHandlers.Count > 0)
            {
                this.compensationHandlers = this.configuration.CompensationHandlers;
            }
        }
    }
}