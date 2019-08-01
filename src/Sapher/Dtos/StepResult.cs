using System.Collections.Generic;

namespace Sapher.Dtos
{
    public class StepResult
    {
        public string StepName { get; }

        public InputResult InputHandlerResult { get; set; }

        public ResponseResult ResponseHandlerResult { get; set; }

        public StepResult(string stepName)
        {
            this.StepName = stepName;
        }
    }
}