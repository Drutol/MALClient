using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace MALClient.Shared.Managers
{
    /// <summary>
    /// Permament workaround
    /// </summary>
    public static class StoreLogoWorkaroundHacker
    {
        const string xml = @"<tile>
                              <visual>
                                <binding template=""TileMedium"">
                                  <image src='ms-appx:///Assets/MalClientTransparentLogo300x300.png'/>
                                </binding>                                
                                <binding template=""TileSmall"">
                                  <image src='ms-appx:///Assets/MalClientTransparentLogo150x150.png'/>
                                </binding>
                              </visual>
                            </tile>";

        public static void Hack()
        {
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            tileUpdater.Update(new TileNotification(document));
        }
    }
}
