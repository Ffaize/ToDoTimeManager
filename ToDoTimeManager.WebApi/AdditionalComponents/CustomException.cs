namespace ToDoTimeManager.WebApi.AdditionalComponents
{
    public class CustomException : ApplicationException
    {
        public DateTime ErrorTimeStamp { get; set; }
        public string CauseOfError { get; set; }

        private readonly string msgDetails = string.Empty;

        #region Constructors

        public CustomException(string causeOfError)
        {
            CauseOfError = causeOfError;
        }

        public CustomException(string msg, string causeOfError)
        {
            msgDetails = msg;
            CauseOfError = "Unknown";
        }

        public CustomException(string msg, string cause, DateTime dateTime, string causeOfError)
        {
            msgDetails = msg;
            CauseOfError = cause;
            ErrorTimeStamp = dateTime;
        }

        #endregion

        public override string Message => msgDetails;
    }
}
