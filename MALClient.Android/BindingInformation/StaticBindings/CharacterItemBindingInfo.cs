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
using MALClient.Models.Models.Favourites;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BindingInformation.StaticBindings
{
    public class CharacterItemBindingInfo : IStaticBindingInfo<FavouriteViewModel>
    {
        private static CharacterItemBindingInfo _instance;

        private CharacterItemBindingInfo()
        { }

        public static CharacterItemBindingInfo Instance => _instance ?? (_instance = new CharacterItemBindingInfo());

        public void Bind(View target, FavouriteViewModel model)
        {
            target.FindViewById<TextView>(Resource.Id.CharacterItemName).Text = model.Data.Name;
            target.FindViewById<TextView>(Resource.Id.CharacterItemNotes).Text = model.Data.Notes;
            var img = target.FindViewById<ImageViewAsync>(Resource.Id.CharacterItemImage);
            ImageService.Instance.LoadUrl(model.Data.ImgUrl).FadeAnimation(false).Success(() => img.AnimateFadeIn()).Into(img);
        }
    }
}