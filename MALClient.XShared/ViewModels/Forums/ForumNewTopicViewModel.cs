using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeKicker.BBCode;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
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

        public void Init(ForumsNewTopicNavigationArgs args)
        {
            Answers.Clear();
            Answers.Add(new ForumTopicQestionModel {Removable = false});
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
                Header = $"Post New topic in {args.Title} club";
            }
        }

        public string Title { get; set; }
        public string Message { get; set; }

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
            }
        }

        public bool AnswersVisibility => !string.IsNullOrEmpty(Question);
        

        public ICommand PreviewCommand => _previewCommand ?? (_previewCommand
                                              = new RelayCommand(() =>
                                              {
                                                  string msg; 
                                                  try
                                                  {
                                                      msg = BBCode.ToHtml(Message);
                                                  }
                                                  catch (Exception e)
                                                  {
                                                      msg = $"{e.Message} ---- {Message}";
                                                  }
                                                  UpdatePreview?.Invoke(this, msg.Replace("\r\r", "<br><br>"));

                                              }));

        public ICommand CreateTopicCommand => _createTopicCommand ?? (_createTopicCommand = new RelayCommand(
                                                  async () =>
                                                  {
                                                      await ForumNewTopicQuery.CreateNewTopic(Title, Message,
                                                          _prevArgs.ForumType, _prevArgs.Id,Question,Answers.Any(model => !string.IsNullOrEmpty(model.Answer)) ? Answers.Select(model => model.Answer).ToList() : null);
                                                  }));

        public ICommand AddAnswerCommand
            => _addAnswerCommand ?? (_addAnswerCommand = new RelayCommand(
                   () =>
                   {
                       Answers.Add(new ForumTopicQestionModel());
                   }));

        public ICommand RemoveAnswerCommand
                        => _removeAnswerCommand ?? (_removeAnswerCommand = new RelayCommand<ForumTopicQestionModel>(
                   model =>
                   {
                       Answers.Remove(model);
                   }));
    }
}
