using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Java.Lang;

namespace MALClient.Android
{
    public class LeadingSpannableString : Java.Lang.Object, ILeadingMarginSpanLeadingMarginSpan2
    {
        private readonly int _margin;

        public LeadingSpannableString(int lines, int margin)
        {
            _margin = margin;
            LeadingMarginLineCount = lines;
        }


        public void DrawLeadingMargin(Canvas c, Paint p, int x, int dir, int top, int baseline, int bottom,
            ICharSequence text,
            int start, int end, bool first, Layout layout)
        {

        }

        public int GetLeadingMargin(bool first)
        {
            if (first)
                return _margin;
            else return 0;

            //Offset for all other Layout layout ) { }  
            /*Returns * the number of rows which should be applied *     indent returned by getLeadingMargin (true)   
            * Note:* Indent only applies to N lines of the first paragraph.*/
        }

        public int LeadingMarginLineCount { get; }
    }
}