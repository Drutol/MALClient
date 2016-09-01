namespace MALClient.XShared.NavArgs
{
    public enum MessageDetailsWorkMode
    {
        Message,
        ProfileComments,
    }

    public class MalMessageDetailsNavArgs
    {
        public MessageDetailsWorkMode WorkMode { get; set; }
        public object Arg { get; set; } //either comment or message
    }
}
