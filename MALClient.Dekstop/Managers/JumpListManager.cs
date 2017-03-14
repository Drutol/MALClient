using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Shared.Managers
{
    public static class JumpListManager
    {
        public static async void InitJumpList()
        {
            if(!Credentials.Authenticated)
                return;

            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.None;
            jumpList.Items.Clear();

            var item = JumpListItem.CreateWithArguments("https://myanimelist.net/anime.php", "Search");
            item.Logo = new Uri("ms-appx:///Assets/Square44x44Logo.targetsize-256_altform-unplated.png");
            jumpList.Items.Add(item);
            item = JumpListItem.CreateWithArguments("https://myanimelist.net/mymessages.php", "Messages");
            item.Logo = new Uri("ms-appx:///Assets/Square44x44Logo.targetsize-256_altform-unplated.png");
            jumpList.Items.Add(item);
            item = JumpListItem.CreateWithArguments("https://myanimelist.net/forum", "Forum");
            item.Logo= new Uri("ms-appx:///Assets/Square44x44Logo.targetsize-256_altform-unplated.png");;
            jumpList.Items.Add(item);

            await jumpList.SaveAsync();
        }
    }
}
