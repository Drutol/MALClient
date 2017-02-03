using System;

namespace MALClient.Models.Models.Forums
{
    public class ForumTopicQestionModel
    {
        public event EventHandler<string> AnswerChanged;
        private string _answer;

        public string Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                AnswerChanged?.Invoke(this,value);
            }
        }

        public bool Removable { get; set; } = true;
    }
}
