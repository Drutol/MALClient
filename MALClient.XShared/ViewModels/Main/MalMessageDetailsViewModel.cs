using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Messages;
using MALClient.XShared.Comm.MagicalRawQueries.Profile;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public class MalMessageDetailsViewModel : ViewModelBase
    {
        private static readonly Dictionary<string, List<MalMessageModel>> MessageThreads =
            new Dictionary<string, List<MalMessageModel>>();

        public SmartObservableCollection<MalMessageModel> MessageSet { get; set; } =
            new SmartObservableCollection<MalMessageModel>();

        private bool _loadingVisibility = false;

        public bool LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private bool _newMessageFieldsVisibility;

        public bool NewMessageFieldsVisibility
        {
            get { return _newMessageFieldsVisibility; }
            set
            {
                _newMessageFieldsVisibility = value;
                RaisePropertyChanged(() => NewMessageFieldsVisibility);
            }
        }

        private bool _sendingMessageSpinnerVisibility = false;

        public bool SendingMessageSpinnerVisibility
        {
            get { return _sendingMessageSpinnerVisibility; }
            set
            {
                _sendingMessageSpinnerVisibility = value;
                RaisePropertyChanged(() => SendingMessageSpinnerVisibility);
            }
        }

        private bool _isSendButtonEnabled = true;

        public bool IsSendButtonEnabled
        {
            get { return _isSendButtonEnabled; }
            set
            {
                _isSendButtonEnabled = value;
                SendingMessageSpinnerVisibility = !value;
                RaisePropertyChanged(() => IsSendButtonEnabled);
            }
        }

        private ICommand _sendMessageCommand;


        public ICommand SendMessageCommand
            => _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand(SendMessage));

        public string MessageText { get; set; } //body from text box
        public string MessageTarget { get; set; } //targetted user
        public string MessageSubject { get; set; }

        private bool _newMessage;
        private MalMessageModel _prevMsg;
        private MalMessageDetailsNavArgs _prevArgs;

        public MalMessageDetailsViewModel()
        {
            MessageSet.CollectionChanged += (a, e) => RaisePropertyChanged(() => MessageSet);
        }

        public async void Init(MalMessageDetailsNavArgs args,bool force = false)
        {
            if (args.WorkMode == MessageDetailsWorkMode.Message)
            {
                var arg = args.Arg as MalMessageModel;
                if (arg == null) //compose new
                {
                    _newMessage = true;
                    MessageSet.Clear();
                    NewMessageFieldsVisibility = true;
                    ViewModelLocator.GeneralMain.OffRefreshButtonVisibility = false;
                    MessageTarget = args.NewMessageTarget;
                    RaisePropertyChanged(() => MessageTarget);
                    return;
                }
                NewMessageFieldsVisibility = false;
                _newMessage = false;

                if (!force &&_prevMsg?.Id == arg.Id)
                    return;
                _prevMsg = arg;
                MessageSet.Clear();
                LoadingVisibility = true;
                if (!force && arg.ThreadId != null && MessageThreads.ContainsKey(arg.ThreadId))
                {
                    MessageSet.AddRange(MessageThreads[arg.ThreadId]);
                }
                else
                {
                    var msgs = await new MalMessageDetailsQuery().GetMessagesInThread(arg);
                    msgs.Reverse();
                    MessageSet.AddRange(msgs);
                }
                
            }
            else
            {
                NewMessageFieldsVisibility = false;
                var arg = args.Arg as MalComment;
                if(!force && arg.ComToCom == (_prevArgs?.Arg as MalComment)?.ComToCom)
                    return;
                _prevMsg = null;
                LoadingVisibility = true;
                MessageSet.Clear();
                MessageSet =
                    new SmartObservableCollection<MalMessageModel>(
                        (await ProfileCommentQueries.GetComToComMessages(arg.ComToCom)));
                RaisePropertyChanged(() => MessageSet);
            }
            _prevArgs = args;
            LoadingVisibility = false;
        }

        public void RefreshData()
        {
            if(!NewMessageFieldsVisibility)
                Init(_prevArgs,true);
        }

        //private async void FetchHistory()
        //{
        //    LoadingVisibility = true;
        //    FetchHistoryVisibility = false;
        //    MessageSet.Clear();
        //    var result = await new MalMessageDetailsQuery().GetMessagesInThread(_prevMsg);
        //    result.Reverse(); //newest first
        //    _messageThreads[_prevMsg.ThreadId] = result;
        //    MessageSet.AddRange(result.Select(model => new MessageEntry(model)));
        //    LoadingVisibility = false;
        //}

        private async void SendMessage()
        {
            IsSendButtonEnabled = false;
            if (!string.IsNullOrEmpty(MessageText))
            {
                if (_prevArgs == null || _prevArgs.WorkMode == MessageDetailsWorkMode.Message)
                {
                    if (_newMessage)
                    {
                        if (MessageSubject == null || MessageTarget == null)
                        {
                            IsSendButtonEnabled = true;
                            return;
                        }
                        if (await new SendMessageQuery().SendMessage(MessageSubject, MessageText, MessageTarget))
                        {
                            try
                            {
                                await Task.Delay(500); //let mal process the thing
                                var message = new MalMessageModel();
                                var id = await new MalMessagesQuery().GetFirstSentMessageId();
                                message.Id = id;
                                message = await new MalMessageDetailsQuery().GetMessageDetails(message,true);
                                message.Target = MessageTarget;
                                message.Sender = Credentials.UserName;
                                message.IsRead = true;
                                message.Date = DateTime.Now.ToString("d", CultureInfo.InvariantCulture);
                                message.Subject = MessageSubject;
                                MessageThreads[message.ThreadId] = new List<MalMessageModel> {message};
                                _prevMsg = message;
                                _newMessage = false;
                                NewMessageFieldsVisibility = false;
                                ViewModelLocator.GeneralMain.CurrentOffStatus = $"{message.Sender} - {message.Subject}";
                                MessageSet.Clear();
                                MessageSet.AddRange(new[]
                                {
                                    message
                                });
                                MessageText = "";
                                MessageSubject = "";
                                MessageTarget = "";
                            }
                            catch (Exception)
                            {
                                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to send this message.",
                                    "Error");

                            }
                        }
                        else
                        {
                            ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to send this message.",
                                "Error");
                        }
                        IsSendButtonEnabled = true;
                        return;
                    }

                    if (
                        await
                            new SendMessageQuery().SendMessage(_prevMsg.Subject, MessageText, _prevMsg.Target,
                                _prevMsg.ThreadId,
                                _prevMsg.ReplyId))
                    {
                        var message = new MalMessageModel
                        {
                            Subject = _prevMsg.Subject,
                            Content = MessageText,
                            Date = DateTime.Now.ToString("d", CultureInfo.InvariantCulture),
                            Id = "0",
                            Sender = Credentials.UserName,
                            Target = _prevMsg.Target,
                            ThreadId = _prevMsg.ThreadId,
                            ReplyId = _prevMsg.ReplyId
                        };
                        if (MessageThreads.ContainsKey(_prevMsg.ThreadId))
                        {
                            MessageThreads[_prevMsg.ThreadId].Insert(0, message);
                        }
                        else
                        {
                            MessageThreads[_prevMsg.ThreadId] = new List<MalMessageModel> {_prevMsg, message};
                        }
                        MessageSet.AddRange(new[]
                        {
                            message
                        });
                        MessageText = "";
                        MessageSubject = "";
                        MessageTarget = "";
                        RaisePropertyChanged(() => MessageText);
                    }
                    else
                    {
                        ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to send this message.", "Error");
                    }
                }
                else //comment
                {
                    var comment = _prevArgs.Arg as MalComment;
                    if (await ProfileCommentQueries.SendCommentReply(comment.ComToCom.Split('=').Last(), MessageText))
                    {
                        MessageSet.Add(
                        
                            new MalMessageModel
                            {
                                Content = MessageText,
                                Sender = Credentials.UserName,
                                Date = DateTime.Now.ToString("d", CultureInfo.InvariantCulture)
                            }
                        );
                        MessageText = "";
                        RaisePropertyChanged(()=> MessageSet);
                        RaisePropertyChanged(() => MessageText);
                    }
                }
            }
            IsSendButtonEnabled = true;
        }

        
    }
}