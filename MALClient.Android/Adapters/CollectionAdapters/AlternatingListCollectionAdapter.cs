using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MALClient.Android.Resources;

namespace MALClient.Android.CollectionAdapters
{
    public abstract class AlternatingListCollectionAdapter<TModel> : BaseAdapter<TModel>
    {
        private readonly Activity _context;
        private readonly int _layoutResource;
        private readonly List<TModel> _items;
        private readonly int _alternator;

        protected AlternatingListCollectionAdapter(Activity context,int layoutResource,IEnumerable<TModel> items,bool startLight)
        {
            _context = context;
            _layoutResource = layoutResource;
            _items = items.ToList();
            _alternator = startLight ? 0 : 1;
        }

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(_layoutResource,null);

                InitializeView(view,_items[position]);
            }
            return view;
        }

        public override int Count => _items.Count();

        public override TModel this[int position] => _items[position];

        protected abstract void InitializeView(View view, TModel model);
    }
}