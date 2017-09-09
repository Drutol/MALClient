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
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;

namespace MALClient.Android.Dialogs
{
    public class AddFriendDialog
    {
        #region Singleton

        private static AddFriendDialog _instance;

        private AddFriendDialog()
        {

        }

        public static AddFriendDialog Instance => 
            _instance ?? (_instance = new AddFriendDialog());


        #endregion

        public void ShowDialog(ProfileData data)
        {
            
        }
    }
}