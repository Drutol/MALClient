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
using GalaSoft.MvvmLight.Helpers;
using Java.Util;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;


namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    public class AnimeDetailsPageGeneralTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;

        private AnimeDetailsPageGeneralTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.LoadingGlobal).WhenSourceChanges(() =>
            {
                try
                {
                    if (!ViewModel.LoadingGlobal)
                    {
                        //left details
                        AnimeDetailsPageGeneralTabFragmentType.Text = ViewModel.RightDetailsRow[0].Item2;
                        AnimeDetailsPageGeneralTabFragmentStatus.Text = ViewModel.RightDetailsRow[1].Item2;
                        AnimeDetailsPageGeneralTabFragmentEnd.Text = ViewModel.RightDetailsRow[2].Item2;
                        AnimeDetailsPageGeneralTabFragmentMyStartButton.SetOnClickListener(new OnClickListener(view =>
                        {
                            var date = ViewModel.StartDateValid ? ViewModel.StartDateTimeOffset : DateTimeOffset.Now;
                            DatePickerDialog dpd = new DatePickerDialog(Activity, new DateSetListener((i, i1, arg3) =>
                                {
                                    ViewModel.StartDateTimeOffset =
                                        new DateTimeOffset(i, i1, arg3, 0, 0, 0, TimeSpan.Zero);
                                    AnimeDetailsPageGeneralTabFragmentMyStart.Text = ViewModel.MyStartDate;
                                }),
                                date.Year, date.Month - 1, date.Day);
                            dpd.Show();
                        }));
                        AnimeDetailsPageGeneralTabFragmentMyStartButton.SetOnLongClickListener(new OnLongClickListener(view => ViewModel.ResetStartDateCommand.Execute(null)));

                        AnimeDetailsPageGeneralTabFragmentEpisodesLabel.Text = ViewModel.LeftDetailsRow[0].Item1;
                        //right details
                        AnimeDetailsPageGeneralTabFragmentEpisodes.Text = ViewModel.LeftDetailsRow[0].Item2;
                        AnimeDetailsPageGeneralTabFragmentScore.Text = ViewModel.LeftDetailsRow[1].Item2;
                        AnimeDetailsPageGeneralTabFragmentStart.Text = ViewModel.LeftDetailsRow[2].Item2;
                        AnimeDetailsPageGeneralTabFragmentMyEndButton.SetOnClickListener(new OnClickListener(view =>
                        {
                            var date = ViewModel.EndDateValid ? ViewModel.EndDateTimeOffset : DateTimeOffset.Now;
                            DatePickerDialog dpd = new DatePickerDialog(Activity, new DateSetListener((i, i1, arg3) =>
                                {
                                    ViewModel.EndDateTimeOffset =
                                        new DateTimeOffset(i, i1, arg3, 0, 0, 0, TimeSpan.Zero);
                                    AnimeDetailsPageGeneralTabFragmentMyEnd.Text = ViewModel.MyEndDate;
                                }),
                                date.Year, date.Month - 1, date.Day);
                            dpd.Show();
                        }));
                        AnimeDetailsPageGeneralTabFragmentMyEndButton.SetOnLongClickListener(new OnLongClickListener(view => ViewModel.ResetEndDateCommand.Execute(null)));
                        //rest
                        if (!string.IsNullOrEmpty(ViewModel.Synopsis))
                        {
                            AnimeDetailsPageGeneralTabFragmentSynopsis.Text = ViewModel.Synopsis;
                            AnimeDetailsPageGeneralTabFragmentSynopsis.Gravity = GravityFlags.Left;
                        }
                        else
                        {
                            AnimeDetailsPageGeneralTabFragmentSynopsis.Text = "Synopsis unavailable...";
                            AnimeDetailsPageGeneralTabFragmentSynopsis.Gravity = GravityFlags.CenterHorizontal;
                        }
                    }
                }
                catch (Exception)
                {
                    //data loading has failed

                }
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.AddAnimeVisibility)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.AddAnimeVisibility)
                    {
                        AnimeDetailsPageGeneralTabFragmentMyStartButton.Visibility =
                            AnimeDetailsPageGeneralTabFragmentMyEndButton.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        AnimeDetailsPageGeneralTabFragmentMyStartButton.Visibility =
                            AnimeDetailsPageGeneralTabFragmentMyEndButton.Visibility = ViewStates.Visible;
                    }
                }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.MyStartDate,
                    () => AnimeDetailsPageGeneralTabFragmentMyStart.Text));

            Bindings.Add(
                this.SetBinding(() => ViewModel.MyEndDate,
                    () => AnimeDetailsPageGeneralTabFragmentMyEnd.Text));

            Bindings.Add(this.SetBinding(() => ViewModel.EndDateTimeOffset).WhenSourceChanges(() =>
            {
                AnimeDetailsPageGeneralTabFragmentMyStart.Text = ViewModel.MyStartDate;
                AnimeDetailsPageGeneralTabFragmentMyEnd.Text = ViewModel.MyEndDate;
            }));
        }


        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageGeneralTab;

        public static AnimeDetailsPageGeneralTabFragment Instance => new AnimeDetailsPageGeneralTabFragment();

        #region Views

        private TextView _animeDetailsPageGeneralTabFragmentEpisodesLabel;
        private TextView _animeDetailsPageGeneralTabFragmentEpisodes;
        private TextView _animeDetailsPageGeneralTabFragmentScore;
        private TextView _animeDetailsPageGeneralTabFragmentStart;
        private TextView _animeDetailsPageGeneralTabFragmentMyStart;
        private FrameLayout _animeDetailsPageGeneralTabFragmentMyStartButton;
        private TextView _animeDetailsPageGeneralTabFragmentType;
        private TextView _animeDetailsPageGeneralTabFragmentStatus;
        private TextView _animeDetailsPageGeneralTabFragmentEnd;
        private TextView _animeDetailsPageGeneralTabFragmentMyEnd;
        private FrameLayout _animeDetailsPageGeneralTabFragmentMyEndButton;
        private TextView _animeDetailsPageGeneralTabFragmentSynopsis;

        public TextView AnimeDetailsPageGeneralTabFragmentEpisodesLabel => _animeDetailsPageGeneralTabFragmentEpisodesLabel ?? (_animeDetailsPageGeneralTabFragmentEpisodesLabel = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentEpisodesLabel));

        public TextView AnimeDetailsPageGeneralTabFragmentEpisodes => _animeDetailsPageGeneralTabFragmentEpisodes ?? (_animeDetailsPageGeneralTabFragmentEpisodes = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentEpisodes));

        public TextView AnimeDetailsPageGeneralTabFragmentScore => _animeDetailsPageGeneralTabFragmentScore ?? (_animeDetailsPageGeneralTabFragmentScore = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentScore));

        public TextView AnimeDetailsPageGeneralTabFragmentStart => _animeDetailsPageGeneralTabFragmentStart ?? (_animeDetailsPageGeneralTabFragmentStart = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentStart));

        public TextView AnimeDetailsPageGeneralTabFragmentMyStart => _animeDetailsPageGeneralTabFragmentMyStart ?? (_animeDetailsPageGeneralTabFragmentMyStart = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentMyStart));

        public FrameLayout AnimeDetailsPageGeneralTabFragmentMyStartButton => _animeDetailsPageGeneralTabFragmentMyStartButton ?? (_animeDetailsPageGeneralTabFragmentMyStartButton = FindViewById<FrameLayout>(Resource.Id.AnimeDetailsPageGeneralTabFragmentMyStartButton));

        public TextView AnimeDetailsPageGeneralTabFragmentType => _animeDetailsPageGeneralTabFragmentType ?? (_animeDetailsPageGeneralTabFragmentType = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentType));

        public TextView AnimeDetailsPageGeneralTabFragmentStatus => _animeDetailsPageGeneralTabFragmentStatus ?? (_animeDetailsPageGeneralTabFragmentStatus = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentStatus));

        public TextView AnimeDetailsPageGeneralTabFragmentEnd => _animeDetailsPageGeneralTabFragmentEnd ?? (_animeDetailsPageGeneralTabFragmentEnd = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentEnd));

        public TextView AnimeDetailsPageGeneralTabFragmentMyEnd => _animeDetailsPageGeneralTabFragmentMyEnd ?? (_animeDetailsPageGeneralTabFragmentMyEnd = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentMyEnd));

        public FrameLayout AnimeDetailsPageGeneralTabFragmentMyEndButton => _animeDetailsPageGeneralTabFragmentMyEndButton ?? (_animeDetailsPageGeneralTabFragmentMyEndButton = FindViewById<FrameLayout>(Resource.Id.AnimeDetailsPageGeneralTabFragmentMyEndButton));

        public TextView AnimeDetailsPageGeneralTabFragmentSynopsis => _animeDetailsPageGeneralTabFragmentSynopsis ?? (_animeDetailsPageGeneralTabFragmentSynopsis = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentSynopsis));


        #endregion
    }
}