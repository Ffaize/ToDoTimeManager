namespace ToDoTimeManager.WebApi.AdditionalComponents
{
    public class CustomException : ApplicationException
    {
        public DateTime ErrorTimeStamp { get; set; }
        public string CauseOfError { get; set; }

        private string msgDetails = string.Empty;

        #region Constructors

        public CustomException() { }

        public CustomException(string msg)
        {
            msgDetails = msg;
            CauseOfError = "Unknown";
        }

        public CustomException(string msg, string cause, DateTime dateTime)
        {
            msgDetails = msg;
            CauseOfError = cause;
            ErrorTimeStamp = dateTime;
        }

        #endregion

        public override string Message => msgDetails;
    }
}
