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
        public string Text
        {
            get { return _contentBox.Text; }
            set { _contentBox.Text = value; }
        }

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

            var bold = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBold);
            bold.Tag = (int)BBCodeMarkers.Bold;
            bold.Click += ButtonOnClick;

            var italic = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnItalic);
            italic.Tag = (int)BBCodeMarkers.Italic;
            italic.Click += ButtonOnClick;

            var underline = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldUnderline);
            underline.Tag = (int)BBCodeMarkers.Underline;
            underline.Click += ButtonOnClick;

            var center = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldCenter);
            center.Tag = (int)BBCodeMarkers.AlignCenter;
            center.Click += ButtonOnClick;

            var right = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldRight);
            right.Tag = (int)BBCodeMarkers.AlignRight;
            right.Click += ButtonOnClick;

            var list = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldList);
            list.Tag = (int)BBCodeMarkers.List;
            list.Click += ButtonOnClick;

            var spoiler = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldSpoiler);
            spoiler.Tag = (int)BBCodeMarkers.Spoiler;
            spoiler.Click += ButtonOnClick;

            var code = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldCode);
            code.Tag = (int)BBCodeMarkers.Code;
            code.Click += ButtonOnClick;

            var image = _editorView.FindViewById(Resource.Id.BBCodeEditorBtnBoldImage);
            image.Tag = (int)BBCodeMarkers.Image;
            image.Click += ButtonOnClick;

            AddView(_editorView);
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            EditorButtonOnClick((BBCodeMarkers)(int)(sender as View).Tag);
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