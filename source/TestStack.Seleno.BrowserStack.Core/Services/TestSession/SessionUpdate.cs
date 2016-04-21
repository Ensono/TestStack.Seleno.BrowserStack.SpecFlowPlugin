namespace TestStack.Seleno.BrowserStack.Core.Services.TestSession
{
    public class SessionUpdate
    {
        public string Status { get; }

        public string Reason { get; }

        public SessionUpdate(SessionStatus sessionStatus, string reason)
        {
            Status = sessionStatus.ToString().ToLower();
            Reason = reason;
        }
    }
}