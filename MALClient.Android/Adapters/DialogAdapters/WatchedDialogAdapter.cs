using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Android.Resources;

namespace MALClient.Android.DialogAdapters
{
    public class WatchedDialogAdapter : BaseAdapter<int>
    {
        private readonly Activity _context;
        private readonly int _currentEpisodes;
        private readonly int? _currentAiringEpisode;
        public List<int> Items { get; } 

        public WatchedDialogAdapter(Activity context, int currentEpisodes, int maxEpisodes, int? currentAiringEpisode)
        {
            _context = context;
            _currentEpisodes = currentEpisodes;
            _currentAiringEpisode = currentAiringEpisode;
            if (currentAiringEpisode != null && maxEpisodes == 0)
                maxEpisodes = currentEpisodes;

            var numbers = new List<int>();
            if (maxEpisodes == 0)
            {
                int i = currentEpisodes, j = currentEpisodes - 1, k = 0;
                for (; k < 10; i++, j--, k++)
                {
                    if (i <= maxEpisodes)
                        numbers.Add(i);
                    if (j >= 0)
                        numbers.Add(j);
                }
            }
            else
            {
                int i = currentEpisodes, j = currentEpisodes - 1, k = 0;
                for (; i <= maxEpisodes || k < 10; i++, j--, k++)
                {
                    if (i <= maxEpisodes)
                        numbers.Add(i);
                    if (j >= 0)
                        numbers.Add(j);
                }
            }
 
            Items = numbers.OrderBy(i1 => i1).ToList();
        }

        public override long GetItemId(int position) => Items[position];


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var ep = this[position];
            var view = convertView;
            TextView txt = null;
            if (view == null)
            {
                txt = new TextView(_context);       
      
                txt.Elevation = 2f;
                var size = DimensionsHelper.DpToPx(40);
                txt.LayoutParameters = new ViewGroup.LayoutParams(size,size);          
                txt.TextAlignment = TextAlignment.Center;
                txt.Gravity = GravityFlags.Center;
                view = txt;
            }

  

            txt = txt ?? view as TextView;
            txt.Text = ep.ToString();

            if (_currentAiringEpisode == ep)
            {
                txt.SetTypeface(Typeface.SansSerif, TypefaceStyle.Bold);
                txt.SetTextSize(ComplexUnitType.Sp, 19.5f);
            }
            else
            {
                txt.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
                txt.SetTextSize(ComplexUnitType.Sp, 15);
            }

            if (ep <= _currentEpisodes)
            {
                txt.SetBackgroundColor(new Color(ResourceExtension.AccentColour));
                txt.SetTextColor(Color.White);
            }
            else
            {
                txt.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));
                txt.SetTextColor(new Color(ResourceExtension.BrushText));
            }

            return txt;
        }

        public override int Count => Items.Count;

        public override int this[int position] => Items[position];

    }
}