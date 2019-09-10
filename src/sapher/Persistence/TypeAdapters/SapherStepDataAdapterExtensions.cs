namespace Sapher.Persistence.TypeAdapters
{
    using Model = Persistence.Model;

    internal static class SapherStepDataAdapterExtensions
    {
        internal static Model.SapherStepData ToDataModel(this Dtos.SapherStepData data)
            => new Model.SapherStepData
            {
                StepName = data.StepName,
                State = data.State.ToDataModel(),
                DataToPersist = data.DataToPersist,
                InputMessageSlip = data.InputMessageSlip.ToDataModel(),
                MessagesWaitingResponse = data.MessagesWaitingResponse,
                SuccessfulMessages = data.SuccessfulMessages,
                FailedMessages = data.FailedMessages,
                CompensatedMessages = data.CompensatedMessages,
                CreationDate = data.CreationDate,
                UpdatedOn = data.UpdateDate
            };

        internal static Dtos.SapherStepData ToDto(this Model.SapherStepData data)
            => new Dtos.SapherStepData
            {
                StepName = data.StepName,
                State = data.State.ToDto(),
                DataToPersist = data.DataToPersist,
                InputMessageSlip = data.InputMessageSlip.ToDto(),
                MessagesWaitingResponse = data.MessagesWaitingResponse,
                SuccessfulMessages = data.SuccessfulMessages,
                FailedMessages = data.FailedMessages,
                CompensatedMessages = data.CompensatedMessages,
                CreationDate = data.CreationDate,
                UpdateDate = data.UpdatedOn
            };
    }
}