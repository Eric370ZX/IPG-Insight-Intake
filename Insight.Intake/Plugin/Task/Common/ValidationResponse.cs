namespace Insight.Intake.Plugin.TaskEntity
{
    public class ValidationResponse
    {
        public ValidationResponse(bool succeeded, string message = "")
        {
            Succeeded = succeeded;
            Message = message;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}
