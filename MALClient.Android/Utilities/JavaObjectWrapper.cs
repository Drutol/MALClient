namespace MALClient.Android
{
    public class JavaObjectWrapper<TObj> : Java.Lang.Object where TObj : class
    {
        public TObj Instance { get; }

        public JavaObjectWrapper(TObj obj)
        {
            Instance = obj;
        }
    }
}