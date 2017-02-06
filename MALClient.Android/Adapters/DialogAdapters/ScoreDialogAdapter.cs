using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;

namespace MALClient.Android.DialogAdapters
{
    public class ScoreDialogAdapter : BaseAdapter<int>
    {
        private readonly Activity _context;
        private readonly List<string> _desciptions;
        private readonly float _currentScore;



        public ScoreDialogAdapter(Activity context,IEnumerable<string> desciptions ,float currentScore)
        {
            _context = context;
            _currentScore = Settings.SelectedApiType == ApiType.Hummingbird ? currentScore*2 : currentScore;
            _desciptions = desciptions.ToList();
            _desciptions.Add("0 - Unranked");
            _desciptions.Reverse();
        }

        public override long GetItemId(int position) => 10-position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            position = 10 - position;
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.StatusDialogItem, null);

            var txt = view.FindViewById<TextView>(Resource.Id.StatusDialogItemTextView);
            txt.Text = _desciptions[position];          
            view.SetBackgroundColor(position == _currentScore
                ? new Color(ResourceExtension.BrushSelectedDialogItem)
                : Color.Transparent);        
            view.LayoutParameters = new ViewGroup.LayoutParams(-1, DimensionsHelper.DpToPx(40));
            view.Tag = position;
            return view;
        }

        public override int Count => 11;

        public override int this[int position] => position;

    }
}