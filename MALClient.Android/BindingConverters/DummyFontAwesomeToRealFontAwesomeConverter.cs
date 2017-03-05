using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Models.Enums;

namespace MALClient.Android.BindingConverters
{
    public static class DummyFontAwesomeToRealFontAwesomeConverter
    {
        public static int Convert(FontAwesomeIcon icon)
        {
            switch (icon)
            {
                    case FontAwesomeIcon.PuzzlePiece:
                    return Resource.String.fa_icon_puzzle_piece;
                case FontAwesomeIcon.Money:
                    return Resource.String.fa_icon_money;
                case FontAwesomeIcon.Gavel:
                    return Resource.String.fa_icon_gavel;
                case FontAwesomeIcon.Bullhorn:
                    return Resource.String.fa_icon_bullhorn;
                case FontAwesomeIcon.Glass:
                    return Resource.String.fa_icon_glass;
                case FontAwesomeIcon.Support:
                    return Resource.String.fa_icon_support;
                case FontAwesomeIcon.LightbulbOutline:
                    return Resource.String.fa_icon_lightbulb_o;
                case FontAwesomeIcon.Trophy:
                    return Resource.String.fa_icon_trophy;
                case FontAwesomeIcon.NewspaperOutline:
                    return Resource.String.fa_icon_newspaper_o;
                case FontAwesomeIcon.Gift:
                    return Resource.String.fa_icon_gift;
                case FontAwesomeIcon.FolderOutline:
                    return Resource.String.fa_icon_folder_o;
                case FontAwesomeIcon.Television:
                    return Resource.String.fa_icon_television;
                case FontAwesomeIcon.Book:
                    return Resource.String.fa_icon_book;
                case FontAwesomeIcon.CommentOutline:
                    return Resource.String.fa_icon_comment_o;
                case FontAwesomeIcon.Gamepad:
                    return Resource.String.fa_icon_gamepad;
                case FontAwesomeIcon.Music:
                    return Resource.String.fa_icon_music;
                case FontAwesomeIcon.Coffee:
                    return Resource.String.fa_icon_coffee;
                case FontAwesomeIcon.PictureOutline:
                    return Resource.String.fa_icon_picture_o;
                case FontAwesomeIcon.BarChart:
                    return Resource.String.fa_icon_bar_chart;
                    
            }

            return Resource.String.fa_icon_bug;
        }
    }
}