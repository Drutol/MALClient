using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm.MagicalRawQueries;
using MalClient.Shared.Comm.MagicalRawQueries.Messages;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Main
{
    public class MalMessagingViewModel : ViewModelBase
    {
        private ICommand _composeNewCommand;

        private bool _displaySentMessages;
        private int _loadedPages = 1;

        private Visibility _loadingVisibility;

        private ICommand _loadMoreCommand;

        private Visibility _loadMorePagesVisibility = Visibility.Collapsed;
        private int _selectedMessageIndex = -1;

        private bool _skipLoading;
        private bool _loadedSomething;

        public SmartObservableCollection<MalMessageModel> MessageIndex { get; } =
            new SmartObservableCollection<MalMessageModel>();

        public List<MalMessageModel> Outbox { get; set; } = new List<MalMessageModel>();
        public List<MalMessageModel> Inbox { get; set; } = new List<MalMessageModel>();

        public int SelectedMessageIndex
        {
            get { return _selectedMessageIndex; }
            set
            {
                if(value < 0)
                    return;
                if (MessageIndex[value].IsMine)
                    return;
                _selectedMessageIndex = value;
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails, MessageIndex[value]);
                RaisePropertyChanged(() => SelectedMessageIndex);
            }
        }

        public bool DisplaySentMessages
        {
            get { return _displaySentMessages; }
            set
            {
                _displaySentMessages = value;
                _skipLoading = true;
                LoadMore();
                RaisePropertyChanged(() => DisplaySentMessages);
            }
        }

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        public Visibility LoadMorePagesVisibility
        {
            get { return _loadMorePagesVisibility; }
            set
            {
                _loadMorePagesVisibility = value;
                RaisePropertyChanged(() => LoadMorePagesVisibility);
            }
        }

        public ICommand LoadMoreCommand => _loadMoreCommand ?? (_loadMoreCommand = new RelayCommand(() => LoadMore()));

        public ICommand ComposeNewCommand => _composeNewCommand ?? (_composeNewCommand = new RelayCommand(ComposeNew));


        public void Init(bool force = false)
        {
            if(!force && _loadedSomething)
                return;
            LoadMore(force);
        }

        private async void LoadMore(bool force = false)
        {
            LoadingVisibility = Visibility.Visible;
            if (force)
            {
                if (DisplaySentMessages)
                {
                    Outbox = new List<MalMessageModel>();
                }
                else
                {
                    _loadedPages = 1;
                    Inbox = new List<MalMessageModel>();
                }
            }
            if (!DisplaySentMessages)
                try
                {
                    if (!_skipLoading)
                    {
                        _loadedSomething = true;
                        try
                        {
                            Inbox.AddRange(await AccountMessagesManager.GetMessagesAsync(_loadedPages++));
                        }
                        catch (WebException)
                        {
                            MalHttpContextProvider.ErrorMessage("Messages");
                        }
                    }
                    _skipLoading = false;
                    MessageIndex.Clear();
                    MessageIndex.AddRange(Inbox);
                    LoadMorePagesVisibility = Visibility.Visible;
                }
                catch (ArgumentOutOfRangeException)
                {
                    LoadMorePagesVisibility = Visibility.Collapsed;
                }
            else
                try
                {
                    if (Outbox.Count == 0)
                        Outbox = await AccountMessagesManager.GetSentMessagesAsync();
                    MessageIndex.Clear();
                    MessageIndex.AddRange(Outbox);
                    LoadMorePagesVisibility = Visibility.Collapsed;
                }
                catch (Exception)
                {
                    MalHttpContextProvider.ErrorMessage("Messages");
                }

            LoadingVisibility = Visibility.Collapsed;
        }

        private void ComposeNew()
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails, null); // null for new message
        }
    }
}