using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Forums.Items
{
    public class ForumTopicMessageEntryViewModel : ViewModelBase
    {
        private bool _editMode;
        private string _bbcodeContent;
        private bool _loading;
        public ForumMessageEntry Data { get; }

        public ForumTopicMessageEntryViewModel(ForumMessageEntry data)
        {
            Data = data;
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
