namespace Sapher.Persistence.TypeAdapters
{
    using System.Linq;

    using Model = Persistence.Model;

    public static class SapherStepDataAdapterExtensions
    {
        public static Model.SapherStepData ToDataModel(this Dtos.SapherStepData data)
            => new Model.SapherStepData
            {
                StepName = data.StepName,
                State = data.State.ToDataModel(),
                DataToPersist = data.DataToPersist,
                InputMessageSlip = data.InputMessageSlip.ToDataModel(),
                PublishedMessageIdsResponseState = data
                    .PublishedMessageIdsResponseState
                    .ToDictionary(k => k.Key, v => v.Value.ToDataModel()),
                CreationDate = data.CreationDate,
                UpdatedOn = data.UpdatedOn
            };

        public static Dtos.SapherStepData ToDto(this Model.SapherStepData data)
            => new Dtos.SapherStepData
            {
                StepName = data.StepName,
                State = data.State.ToDto(),
                DataToPersist = data.DataToPersist,
                InputMessageSlip = data.InputMessageSlip.ToDto(),
                PublishedMessageIdsResponseState = data
                    .PublishedMessageIdsResponseState
                    .ToDictionary(k => k.Key, v => v.Value.ToDto()),
                CreationDate = data.CreationDate,
                UpdatedOn = data.UpdatedOn
            };
    }
}