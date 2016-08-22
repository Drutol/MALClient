namespace MALClient.XShared.NavArgs
{
    public class CharacterDetailsNavigationArgs
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            var arg = obj as CharacterDetailsNavigationArgs;
            return arg?.Id == Id;
        }
    }
}
