using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.XShared.Utils;

namespace MALClient.Android.UserControls
{
    public enum BBCodeMarkers
    {
        Bold,
        Italic,
        Underline,
        AlignRight,
        AlignCenter,
        Code,
        Video,
        Spoiler,
        Image,
        List,
    }

    public class BBCodeEditor : LinearLayout
    {
        public string Text { get; set; }
        private View _editorView;
        private EditText _contentBox;

        private EditText ContentBox
            => _contentBox ?? (_contentBox = _editorView.FindViewById<EditText>(Resource.Id.BBCodeEditorTextBox));

        #region Contstructors

        public BBCodeEditor(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        public BBCodeEditor(Context context) : base(context)
        {
            Init();
        }

        public BBCodeEditor(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public BBCodeEditor(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public BBCodeEditor(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }

        #endregion

        private void Init()
        {
            Orientation = Orientation.Vertical;
            LayoutParameters = new ViewGroup.LayoutParams(-1, -2);
            _editorView = (Context as Activity).LayoutInflater.Inflate(Resource.Layout.BBCodeEditor, null);


            AddView(_editorView);
        }

        private void EditorButtonOnClick(BBCodeMarkers marker)
        {
            switch (marker)
            {
                case BBCodeMarkers.Bold:
                    InsertBasicTags("b");
                    break;
                case BBCodeMarkers.Italic:
                    InsertBasicTags("i");
                    break;
                case BBCodeMarkers.Underline:
                    InsertBasicTags("u");
                    break;
                case BBCodeMarkers.AlignRight:
                    InsertBasicTags("right");
                    break;
                case BBCodeMarkers.AlignCenter:
                    InsertBasicTags("center");
                    break;
                case BBCodeMarkers.Code:
                    InsertBasicTags("code");
                    break;
                case BBCodeMarkers.Video:
                    InsertBasicTags("yt");
                    break;
                case BBCodeMarkers.Spoiler:
                    InsertBasicTags("spoiler", true);
                    break;
                case BBCodeMarkers.Image:
                    InsertBasicTags("img");
                    break;
                case BBCodeMarkers.List:
                    InsertListTag();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void InsertBasicTags(string tag, bool parametric = false)
        {
            var text = ContentBox.Text;
            if (ContentBox.Selected)
            {
                var length = ContentBox.SelectionEnd - ContentBox.SelectionStart;
                var selectedText = ContentBox.Text.Substring(ContentBox.SelectionStart, ContentBox.SelectionEnd);
                text = text.Remove(ContentBox.SelectionStart, length);
                var openTag = $"[{tag}{(parametric ? "=" : "")}]";
                text = text.Insert(ContentBox.SelectionStart, selectedText.Wrap(openTag, $"[/{tag}]"));
                var selStart = ContentBox.SelectionStart + openTag.Length;
                Text = text;
                ContentBox.SetSelection(selStart, length);
            }
            else
            {
                text = text.Insert(ContentBox.SelectionStart, "".Wrap($"[{tag}{(parametric ? "=" : "")}]", $"[/{tag}]"));
                Text = text;
            }

        }

        private void InsertListTag()
        {
            var text = ContentBox.Text;
            text = text.Insert(ContentBox.SelectionStart, "".Wrap("[list]\n[*]\n", "[/list]"));
            Text = text;
        }
    }
}