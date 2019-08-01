namespace Sapher.TypeAdapters
{
    using Model = Persistence.Model;

    public static class StepStateAdapterExtensions
    {
        public static Model.StepState ToDataModel(this Dtos.StepState stepState)
        {
            switch (stepState)
            {
                case Dtos.StepState.None:
                    return Model.StepState.None;

                case Dtos.StepState.Successful:
                    return Model.StepState.Successful;

                case Dtos.StepState.Compensated:
                    return Model.StepState.Compensated;

                case Dtos.StepState.ExecutedInput:
                    return Model.StepState.ExecutedInput;

                case Dtos.StepState.FailedOnExecution:
                    return Model.StepState.FailedOnExecution;

                case Dtos.StepState.FailedOnResponses:
                    return Model.StepState.FailedOnResponses;

                default:
                    return Model.StepState.None;
            }
        }

        public static Dtos.StepState ToDto(this Model.StepState stepState)
        {
            switch (stepState)
            {
                case Model.StepState.None:
                    return Dtos.StepState.None;

                case Model.StepState.Successful:
                    return Dtos.StepState.Successful;

                case Model.StepState.Compensated:
                    return Dtos.StepState.Compensated;

                case Model.StepState.ExecutedInput:
                    return Dtos.StepState.ExecutedInput;

                case Model.StepState.FailedOnExecution:
                    return Dtos.StepState.FailedOnExecution;

                case Model.StepState.FailedOnResponses:
                    return Dtos.StepState.FailedOnResponses;

                default:
                    return Dtos.StepState.None;
            }
        }
    }
}