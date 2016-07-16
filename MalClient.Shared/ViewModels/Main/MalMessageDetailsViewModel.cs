using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm.MagicalRawQueries.Messages;
using MALClient.Models;
using MALClient.Utils;

namespace MALClient.ViewModels.Messages
{
    public class MalMessageDetailsViewModel : ViewModelBase
    {
        private static readonly Dictionary<string, List<MalMessageModel>> _messageThreads =
            new Dictionary<string, List<MalMessageModel>>();

        private Visibility _loadingVisibility = Visibility.Collapsed;
        private bool _newMessage;

        private Visibility _newMessageFieldsVisibility;

        private MalMessageModel _prevMsg;

        private ICommand _sendMessageCommand;

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

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

        public ICommand SendMessageCommand
            => _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand(SendMessage));

        public string MessageText { get; set; } //body from text box
        public string MessageTarget { get; set; } //targetted user
        public string MessageSubject { get; set; }

        

        public SmartObservableCollection<MessageEntry> MessageSet { get; } =
            new SmartObservableCollection<MessageEntry>();

        public async void Init(MalMessageModel args)
        {
            if (args == null) //compose new
            {
                _newMessage = true;
                MessageSet.Clear();
                NewMessageFieldsVisibility = Visibility.Visible;
                return;
            }
            NewMessageFieldsVisibility = Visibility.Collapsed;
            _newMessage = false;

            if (_prevMsg?.Id == args.Id)
                return;
            _prevMsg = args;
            MessageSet.Clear();
            LoadingVisibility = Visibility.Visible;
            if (_messageThreads.ContainsKey(args.ThreadId))
            {
                MessageSet.AddRange(_messageThreads[args.ThreadId].Select(model => new MessageEntry(model)));
            }
            else
            {
                var msgs = await new MalMessageDetailsQuery().GetMessagesInThread(args);
                msgs.Reverse();
                MessageSet.AddRange(msgs.Select(model => new MessageEntry(model)));
            }
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
                        _messageThreads[message.ThreadId] = new List<MalMessageModel> { message };
                        _prevMsg = message;
                        _newMessage = false;
                        NewMessageFieldsVisibility = Visibility.Collapsed;
                        ViewModelLocator.GeneralMain.CurrentStatus = $"{message.Sender} - {message.Subject}";
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
                        var msg = new MessageDialog("Unable to send this message.","Error");
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
                    new SendMessageQuery().SendMessage(_prevMsg.Subject, MessageText, _prevMsg.Target, _prevMsg.ThreadId,
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
                if (_messageThreads.ContainsKey(_prevMsg.ThreadId))
                {
                    _messageThreads[_prevMsg.ThreadId].Insert(0, message);
                }
                else
                {
                    _messageThreads[_prevMsg.ThreadId] = new List<MalMessageModel> {_prevMsg, message};
                }
                MessageSet.AddRange(new[]
                {
                    new MessageEntry(message)
                });
                MessageText = "";
                MessageSubject = "";
                MessageTarget = "";
            }
            else
            {
                var msg = new MessageDialog("Unable to send this message.", "Error");
                await msg.ShowAsync();
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