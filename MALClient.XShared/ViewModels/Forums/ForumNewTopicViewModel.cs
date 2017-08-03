using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumNewTopicViewModel : ViewModelBase
    {
        public event EventHandler<string> UpdatePreview;

        private ICommand _previewCommand;
        private ICommand _createTopicCommand;
        private string _header;

        public ObservableCollection<ForumTopicQestionModel> Answers { get; } = new ObservableCollection<ForumTopicQestionModel>();

        private ForumsNewTopicNavigationArgs _prevArgs;
        private string _question;
        private ICommand _addAnswerCommand;
        private ICommand _removeAnswerCommand;
        private string _message;

        public void Init(ForumsNewTopicNavigationArgs args)
        {
            Answers.Clear();
            var model = new ForumTopicQestionModel {Removable = false};
            model.AnswerChanged += ModelOnAnswerChanged;
            Answers.Add(model);
            Question = null;
            _prevArgs = args;
            if (args.BoardType != null)
            {
                Header = $"Post New Topic in {args.BoardType.GetDescription()}";
            }
            else if (args.TopicType != null)
            {
                Header = $"Post New Topic in {args.Title}";
            }
            else
            {
                Header = $"Post New topic in {args.ClubName} club";
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => IsSendButtonEnabled);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                SubmitMessageForDelayedPreview();
                RaisePropertyChanged(() => PreviewAvailable);
                RaisePropertyChanged(() => IsSendButtonEnabled);
            }
        }

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        public string Question
        {
            get { return _question; }
            set
            {
                _question = value;
                RaisePropertyChanged(() => AnswersVisibility);
                RaisePropertyChanged(() => IsSendButtonEnabled);
            }
        }

        public bool AnswersVisibility => !string.IsNullOrEmpty(Question);

        public bool PreviewAvailable => !string.IsNullOrWhiteSpace(Message);

        public bool IsSendButtonEnabled
            =>
                !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Message) &&
                (string.IsNullOrEmpty(Question) || Answers.All(model => !string.IsNullOrWhiteSpace(model.Answer)))
        ;

        public ICommand PreviewCommand => _previewCommand ?? (_previewCommand
                                              = new RelayCommand(() =>
                                              {
                                                  string msg; 
                                                  try
                                                  {
                                                      msg = BBCode.BBCode.ToHtml(Message);
                                                  }
                                                  catch (Exception e)
                                                  {
                                                      msg = $"Error occured while parsing BBCode ---- {Message}";
                                                  }
                                                  UpdatePreview?.Invoke(this, msg);

                                              }));

        public ICommand CreateTopicCommand => _createTopicCommand ?? (_createTopicCommand = new RelayCommand(
                                                   () =>
                                                  {
                                                      ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput("Are you sure to post it?","Are you sure?","Yeah!","No",CreateTopic);
                                                  }));

        public ICommand AddAnswerCommand
            => _addAnswerCommand ?? (_addAnswerCommand = new RelayCommand(
                   () =>
                   {
                       if (Answers.Count <= 4)
                       {
                           var model = new ForumTopicQestionModel();
                           model.AnswerChanged += ModelOnAnswerChanged;
                           Answers.Add(model);
                       }
                       RaisePropertyChanged(() => IsSendButtonEnabled);
                   }));

        private void ModelOnAnswerChanged(object sender, string s)
        {
            RaisePropertyChanged(() => IsSendButtonEnabled);
        }

        public ICommand RemoveAnswerCommand
                        => _removeAnswerCommand ?? (_removeAnswerCommand = new RelayCommand<ForumTopicQestionModel>(
                   model =>
                   {
                       Answers.Remove(model);
                       RaisePropertyChanged(() => IsSendButtonEnabled);
                   }));

        private bool _messageQueued;
        private bool _previewAvailable;
        private string _title;

        private async void SubmitMessageForDelayedPreview()
        {
            if(_messageQueued)
                return;
            _messageQueued = true;
                await Task.Delay(500);
                PreviewCommand.Execute(null);
            _messageQueued = false;
        }

        private async void CreateTopic()
        {
            Tuple<bool,string> result;
            if (_prevArgs.TopicType != null)
            {
                result = await ForumTopicQueries.CreateNewTopic(Title, Message,
                    _prevArgs.TopicType.Value, _prevArgs.Id, Question,
                    Answers.Any(model => !string.IsNullOrEmpty(model.Answer))
                        ? Answers.Select(model => model.Answer).ToList()
                        : null);
            }
            else
            {
                result = await ForumTopicQueries.CreateNewTopic(Title, Message,
                    _prevArgs.ForumType, _prevArgs.Id, Question,
                    Answers.Any(model => !string.IsNullOrEmpty(model.Answer))
                        ? Answers.Select(model => model.Answer).ToList()
                        : null);
            }
            if (result.Item1)
            {
                ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.CreatedTopic);
                if (result.Item2 != null)
                {
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,new ForumsTopicNavigationArgs(result.Item2,null));
                }
                else
                {
                    ViewModelLocator.NavMgr.CurrentMainViewOnBackRequested();
                }

                Message = Title = string.Empty;
            }
            else
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Something went wrong while creating your topic. Remember that you can post new topics every 5 minutes.","Something went wrong!");
            }
        }
    }
}
