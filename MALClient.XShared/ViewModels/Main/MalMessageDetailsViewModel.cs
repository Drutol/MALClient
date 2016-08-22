using System;
using System.Collections.Generic;
using System.Linq;
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

        public SmartObservableCollection<MessageEntry> MessageSet { get; set; } =
            new SmartObservableCollection<MessageEntry>();

        private Visibility _loadingVisibility = Visibility.Collapsed;

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private Visibility _newMessageFieldsVisibility;

        public Visibility NewMessageFieldsVisibility
        {
            get { return _newMessageFieldsVisibility; }
            set
            {
                _newMessageFieldsVisibility = value;
                RaisePropertyChanged(() => NewMessageFieldsVisibility);
            }
        }

        private Visibility _sendingMessageSpinnerVisibility = Visibility.Collapsed;

        public Visibility SendingMessageSpinnerVisibility
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
                SendingMessageSpinnerVisibility = value ? Visibility.Collapsed : Visibility.Visible;
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

        public async void Init(MalMessageDetailsNavArgs args)
        {
            if (args.WorkMode == MessageDetailsWorkMode.Message)
            {
                var arg = args.Arg as MalMessageModel;
                if (arg == null) //compose new
                {
                    _newMessage = true;
                    MessageSet.Clear();
                    NewMessageFieldsVisibility = Visibility.Visible;
                    return;
                }
                NewMessageFieldsVisibility = Visibility.Collapsed;
                _newMessage = false;

                if (_prevMsg?.Id == arg.Id)
                    return;
                _prevMsg = arg;
                MessageSet.Clear();
                LoadingVisibility = Visibility.Visible;
                if (MessageThreads.ContainsKey(arg.ThreadId))
                {
                    MessageSet.AddRange(MessageThreads[arg.ThreadId].Select(model => new MessageEntry(model)));
                }
                else
                {
                    var msgs = await new MalMessageDetailsQuery().GetMessagesInThread(arg);
                    msgs.Reverse();
                    MessageSet.AddRange(msgs.Select(model => new MessageEntry(model)));
                }
                
            }
            else
            {
                NewMessageFieldsVisibility = Visibility.Collapsed;
                var arg = args.Arg as MalComment;
                if(arg.ComToCom == (_prevArgs?.Arg as MalComment)?.ComToCom)
                    return;
                _prevMsg = null;
                LoadingVisibility = Visibility.Visible;
                MessageSet.Clear();
                MessageSet =
                    new SmartObservableCollection<MessageEntry>(
                        (await ProfileCommentQueries.GetComToComMessages(arg.ComToCom)).Select(
                            model => new MessageEntry(model)));
                RaisePropertyChanged(() => MessageSet);
            }
            _prevArgs = args;
            LoadingVisibility = Visibility.Collapsed;
        }

        //private async void FetchHistory()
        //{
        //    LoadingVisibility = Visibility.Visible;
        //    FetchHistoryVisibility = Visibility.Collapsed;
        //    MessageSet.Clear();
        //    var result = await new MalMessageDetailsQuery().GetMessagesInThread(_prevMsg);
        //    result.Reverse(); //newest first
        //    _messageThreads[_prevMsg.ThreadId] = result;
        //    MessageSet.AddRange(result.Select(model => new MessageEntry(model)));
        //    LoadingVisibility = Visibility.Collapsed;
        //}

        private async void SendMessage()
        {
            IsSendButtonEnabled = false;
            if (_prevArgs.WorkMode == MessageDetailsWorkMode.Message)
            {
                if (_newMessage)
                {
                    if (await new SendMessageQuery().SendMessage(MessageSubject, MessageText, MessageTarget))
                    {
                        try
                        {
                            var message = new MalMessageModel();
                            var id = await new MalMessagesQuery().GetFirstSentMessageId();
                            message.Id = id;
                            message = await new MalMessageDetailsQuery().GetMessageDetails(message);
                            message.Target = MessageTarget;
                            message.Sender = Credentials.UserName;
                            message.IsRead = true;
                            message.Date = DateTime.Now.ToString("d");
                            message.Subject = MessageSubject;
                            MessageThreads[message.ThreadId] = new List<MalMessageModel> {message};
                            _prevMsg = message;
                            _newMessage = false;
                            NewMessageFieldsVisibility = Visibility.Collapsed;
                            ViewModelLocator.GeneralMain.CurrentOffStatus = $"{message.Sender} - {message.Subject}";
                            MessageSet.Clear();
                            MessageSet.AddRange(new[]
                            {
                                new MessageEntry(message)
                            });
                            MessageText = "";
                            MessageSubject = "";
                            MessageTarget = "";
                        }
                        catch (Exception)
                        {
                            var msg = new MessageDialog("Unable to send this message.", "Error");
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        var msg = new MessageDialog("Unable to send this message.", "Error");
                        await msg.ShowAsync();
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
                        Date = DateTime.Now.ToString("d"),
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
                        new MessageEntry(message)
                    });
                    MessageText = "";
                    MessageSubject = "";
                    MessageTarget = "";
                    RaisePropertyChanged(() => MessageText);
                }
                else
                {
                    var msg = new MessageDialog("Unable to send this message.", "Error");
                    await msg.ShowAsync();
                }
            }
            else //comment
            {
                var comment = _prevArgs.Arg as MalComment;
                if (await ProfileCommentQueries.SendCommentReply(comment.ComToCom.Split('=').Last(), MessageText))
                {
                    MessageSet.AddRange(new[]
                    {
                        new MessageEntry(new MalMessageModel
                        {
                            Content = MessageText,
                            Sender = Credentials.UserName,
                            Date = DateTime.Now.ToString("d")
                        })
                    });
                    MessageText = "";
                    RaisePropertyChanged(() => MessageText);
                }
            }
            IsSendButtonEnabled = true;
        }

        public class MessageEntry
        {
            public MessageEntry(MalMessageModel msg)
            {
                Msg = msg;
                if (Msg.Sender.Equals(Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    HorizontalAlignment = HorizontalAlignment.Right;
                    Margin = new Thickness(20, 0, 0, 0);
                    CornerRadius = new CornerRadius(10, 10, 0, 10);
                    Background = Application.Current.Resources["SystemControlHighlightAltListAccentLowBrush"] as Brush;
                }
                else
                {
                    HorizontalAlignment = HorizontalAlignment.Left;
                    Margin = new Thickness(0, 0, 20, 0);
                    CornerRadius = new CornerRadius(10, 10, 10, 0);
                    Background = Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] as Brush;
                    Background.Opacity = .5;
                }
            }

            public MalMessageModel Msg { get; }
            public HorizontalAlignment HorizontalAlignment { get; }
            public Thickness Margin { get; }
            public CornerRadius CornerRadius { get; }
            public Brush Background { get; }
        }
    }
}