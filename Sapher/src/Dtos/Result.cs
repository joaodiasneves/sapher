namespace Sapher.Dtos
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        public object DataToPersist { get; set; }
    }
}