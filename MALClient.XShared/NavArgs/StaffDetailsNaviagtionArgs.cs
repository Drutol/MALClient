namespace MALClient.XShared.NavArgs
{
    public class StaffDetailsNaviagtionArgs
    {
        public int Id { get; set; }

        public bool ResetNav { get; set; }

        public override bool Equals(object obj)
        {
            var arg = obj as CharacterDetailsNavigationArgs;
            return arg?.Id == Id;
        }
    }
}
