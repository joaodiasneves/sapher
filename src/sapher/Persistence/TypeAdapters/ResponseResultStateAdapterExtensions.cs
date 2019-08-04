namespace Sapher.Persistence.TypeAdapters
{
    using Model = Persistence.Model;

    public static class ResponseResultStateAdapterExtensions
    {
        public static Model.ResponseResultState ToDataModel(this Dtos.ResponseResultState responseResultState)
        {
            switch (responseResultState)
            {
                case Dtos.ResponseResultState.None:
                    return Model.ResponseResultState.None;

                case Dtos.ResponseResultState.Successful:
                    return Model.ResponseResultState.Successful;

                case Dtos.ResponseResultState.Failed:
                    return Model.ResponseResultState.Failed;

                case Dtos.ResponseResultState.Compensated:
                    return Model.ResponseResultState.Compensated;

                default:
                    return Model.ResponseResultState.None;
            }
        }

        public static Dtos.ResponseResultState ToDto(this Model.ResponseResultState responseResultState)
        {
            switch (responseResultState)
            {
                case Model.ResponseResultState.None:
                    return Dtos.ResponseResultState.None;

                case Model.ResponseResultState.Successful:
                    return Dtos.ResponseResultState.Successful;

                case Model.ResponseResultState.Failed:
                    return Dtos.ResponseResultState.Failed;

                case Model.ResponseResultState.Compensated:
                    return Dtos.ResponseResultState.Compensated;

                default:
                    return Dtos.ResponseResultState.None;
            }
        }
    }
}