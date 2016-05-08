namespace MALClient.Comm
{
    public class MalListParameters : IParameters
    {
        public string User { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        public string GetParamChain()
        {
            return $"u={User}&status={Status}&type={Type}";
        }
    }
}