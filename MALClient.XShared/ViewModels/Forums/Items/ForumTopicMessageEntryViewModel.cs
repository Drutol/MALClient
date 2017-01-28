using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Forums.Items
{
    public class ForumTopicMessageEntryViewModel : ViewModelBase
    {
        private bool _editMode;
        private string _bbcodeContent;
        private bool _loading;
        private double _computedHtmlHeight = -1;
        private bool _isStarred;
        public ForumMessageEntry Data { get; }

        public ForumTopicMessageEntryViewModel(ForumMessageEntry data)
        {
            Data = data;
            _isStarred = ResourceLocator.HandyDataStorage.IsMessageStarred(data.Id,data.Poster.MalUser);
        }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                RaisePropertyChanged(() => EditMode);
            }
        }

        public string BBcodeContent
        {
            get { return _bbcodeContent; }
            set
            {
                _bbcodeContent = value;
                RaisePropertyChanged(() => BBcodeContent);
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public double ComputedHtmlHeight
        {
            get { return _computedHtmlHeight == -1 ? double.NaN : _computedHtmlHeight; }
            set
            {
                _computedHtmlHeight = value;
                RaisePropertyChanged(() => ComputedHtmlHeight);
            }
        }

        public bool IsStarred
        {
            get { return _isStarred; }
            set
            {
                _isStarred = value;
                if (value)
                    ResourceLocator.HandyDataStorage.StarForumMessage(
                        new StarredForumMessage
                        {
                            MessageId = Data.Id,
                            TopicId = Data.TopicId,
                            Poster = Data.Poster.MalUser,
                            MessageNumber = Data.MessageNumber,
                            TopicTitle = ViewModelLocator.ForumsTopic.Header
                        });
                else
                    ResourceLocator.HandyDataStorage.UnstarForumMessage(Data.Id,Data.Poster.MalUser);
                RaisePropertyChanged(() => IsStarred);
            }
        }

        public bool SignatureVisible
            => Settings.ForumsAllowSignatures && !string.IsNullOrWhiteSpace(Data.Poster.SignatureHtml);

        public bool MessagingVisible
            => !Credentials.UserName.Equals(Data.Poster.MalUser.Name, StringComparison.CurrentCultureIgnoreCase);

        public ICommand StartEditCommand => new RelayCommand(async () =>
        {
            Loading = true;
            BBcodeContent = await ForumTopicQueries.GetMessageBbcode(Data.Id);
            EditMode = true;
            Loading = false;
        });

        public ICommand CancelEditCommand => new RelayCommand(() =>
        {
            EditMode = false;
            BBcodeContent = null;
        });

        public ICommand GoToPostersOtherPosts => new RelayCommand(() =>
        {
            ViewModelLocator.ForumsTopic.RegisterSelfBackNav();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,new ForumsBoardNavigationArgs(Data.Poster.MalUser.Name));
        });

        public ICommand SubmitEditCommand => new RelayCommand(async () =>
        {
            Loading = true;
            var resp = await ForumTopicQueries.EditMessage(Data.Id, BBcodeContent,Data.TopicId);
            if (resp == null)
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to edit this comment","Something went wrong");
            }
            else
            {
                Data.HtmlContent = resp;
                RaisePropertyChanged(() => Data);
                BBcodeContent = null;
                EditMode = false;
            }                         
            Loading = false;
        });

        public ICommand DeleteCommand => new RelayCommand(() =>
        {
            ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput("Do you really want to remove this message?","Are you sure?","Yup","Nope",Delete);
        });

        public ICommand QuoteCommand => new RelayCommand(() =>
        {
            ViewModelLocator.ForumsTopic.QuouteMessage(Data.Id,Data.Poster.MalUser.Name);
        });

        private async void Delete()
        {
            if (await ForumTopicQueries.DeleteComment(Data.Id))
            {
                ViewModelLocator.ForumsTopic.RemoveMessage(this);
            }
            else
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to remove this comment", "Something went wrong");
            }
        }
    }
}
