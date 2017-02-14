using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using MALClient.Android.Resources;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BindingInformation.StaticBindings
{
    public class FavouriteItemBindingInfo : IStaticBindingInfo<FavouriteViewModel>
    {
        private static FavouriteItemBindingInfo _instance;

        private FavouriteItemBindingInfo()
        { }

        public static FavouriteItemBindingInfo Instance => _instance ?? (_instance = new FavouriteItemBindingInfo());

        public void Bind(View target, FavouriteViewModel model)
        {
            target.FindViewById<TextView>(Resource.Id.CharacterItemName).Text = model.Data.Name;
            target.FindViewById<TextView>(Resource.Id.CharacterItemNotes).Text = model.Data.Notes;
            var img = target.FindViewById<ImageViewAsync>(Resource.Id.CharacterItemImage);
            img.Into(model.Data.ImgUrl);
        }
    }
}