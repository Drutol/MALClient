using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.MagicalRawQueries.Messages;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public class MalMessagingViewModel : ViewModelBase
    {
        private ICommand _composeNewCommand;

        private bool _displaySentMessages;
        private int _loadedPages = 1;

        private bool _loadingVisibility;

        private ICommand _loadMoreCommand;

        private bool _loadMorePagesVisibility = false;

        private bool _skipLoading;
        private bool _loadedSomething;
        private ICommand _navigateMessageCommand;

        public SmartObservableCollection<MalMessageModel> MessageIndex { get; } =
            new SmartObservableCollection<MalMessageModel>();

        public List<MalMessageModel> Outbox { get; set; } = new List<MalMessageModel>();
        public List<MalMessageModel> Inbox { get; set; } = new List<MalMessageModel>();


        public ICommand NavigateMessageCommand
            => _navigateMessageCommand ?? (_navigateMessageCommand = new RelayCommand<MalMessageModel>(
                   model =>
                   {
                       if(DisplaySentMessages)
                           return;
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails,
                           new MalMessageDetailsNavArgs {WorkMode = MessageDetailsWorkMode.Message, Arg = model});
                   }));



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

        public bool LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        public bool LoadMorePagesVisibility
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
            LoadingVisibility = true;
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
                        catch (Exception)
                        {
                            ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                        }
                    }
                    _skipLoading = false;
                    MessageIndex.Clear();
                    MessageIndex.AddRange(Inbox);
                    LoadMorePagesVisibility = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    LoadMorePagesVisibility = false;
                }
            else
                try
                {
                    if (Outbox.Count == 0)
                        Outbox = await AccountMessagesManager.GetSentMessagesAsync();
                    MessageIndex.Clear();
                    MessageIndex.AddRange(Outbox);
                    LoadMorePagesVisibility = false;
                }
                catch (Exception)
                {
                    ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                }

            LoadingVisibility = false;
        }

        private void ComposeNew()
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails, new MalMessageDetailsNavArgs {WorkMode = MessageDetailsWorkMode.Message}); // null for new message
        }
    }
}