namespace MALClient.Comm
{
    internal class AnimeListParameters : IParameters
    {
        public string user { get; set; }
        public string status { get; set; }
        public string type { get; set; }

        public string GetParamChain()
        {
            return $"u={user}&status={status}&type={type}";
        }
    }
}