namespace Com.Shehabic.Droppy
{ 
    public partial class DroppyMenuPopup
    {
        public static float RequestedElevation { get; set; } = 0;

        public void Show()
        {
            ShowBase();
            MenuView.Elevation = RequestedElevation;
        }
    }
}