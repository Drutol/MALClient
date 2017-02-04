using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CodeKicker.BBCode;
using MALClient.Shared.Managers;
using MALClient.XShared.Utils;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Shared.UserControls
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

    public sealed partial class BBCodeTextBox : UserControl
    {
        private static readonly TypeInfo TypeInfo;
        static BBCodeTextBox()
        {
            var typeInfo = TypeInfo ?? (TypeInfo = typeof(FrameworkElement).GetTypeInfo());
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(BBCodeTextBox), new PropertyMetadata(default(string),TextPropertyChangedCallback));

        private static void TextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as BBCodeTextBox;
            control._lockTextChange = true;
            control.ContentBox.Text = dependencyPropertyChangedEventArgs.NewValue as string ?? "";
            control._lockTextChange = false;
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private bool _lockTextChange;

        public BBCodeTextBox()
        {
            this.InitializeComponent();
        }

        private void EditorButtonOnClick(object sender, RoutedEventArgs e)
        {
            switch ((BBCodeMarkers)(sender as FrameworkElement).Tag)
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
                    InsertBasicTags("spoiler",true);
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


        private void InsertBasicTags(string tag,bool parametric = false)
        {
            var text = ContentBox.Text;
            if (ContentBox.SelectionLength > 0)
            {
                var length = ContentBox.SelectionLength;
                text = text.Remove(ContentBox.SelectionStart, length);
                var openTag = $"[{tag}{(parametric ? "=" : "")}]";
                text = text.Insert(ContentBox.SelectionStart,ContentBox.SelectedText.Wrap(openTag, $"[/{tag}]"));
                var selStart = ContentBox.SelectionStart + openTag.Length;
                Text = text;
                ContentBox.Select(selStart, length);
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

        private void ContentBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_lockTextChange)
                Text = ContentBox.Text;
        }

        private void PreviewButtonOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                PreviewWebView.NavigateToString(CssManager.WrapWithCss(BBCode.ToHtml(ContentBox.Text)));
            }
            catch (Exception)
            {
               //bbcode parsing
            }

        }


        private void ButtonOnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var prop = TypeInfo.GetDeclaredProperty("AllowFocusOnInteraction");
                prop?.SetValue(sender, false);
            }
            catch (Exception)
            {
                //not AU
            }
        }
    }
}
