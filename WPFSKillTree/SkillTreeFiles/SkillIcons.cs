using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace POESKillTree.SkillTreeFiles
{
    public class SkillIcons
    {

        public static int NormalIconWidth = 27;
        public static int NotableIconWidth = 38;
        public static int KeystoneIconWidth = 53;
        public static int MasteryIconWidth = 99;

        public static string urlpath = "http://www.pathofexile.com/image/build-gen/passive-skill-sprite/";
        public Dictionary<string, BitmapImage> Images = new Dictionary<string, BitmapImage>();

        public Dictionary<string, KeyValuePair<Rect, string>> SkillPositions =
            new Dictionary<string, KeyValuePair<Rect, string>>();

        public void OpenOrDownloadImages(SkillTree.UpdateLoadingWindow update = null)
        {
            //Application
            foreach (string image in Images.Keys.ToArray())
            {
                if (!File.Exists(SkillTree.AssetsFolderPath + image))
                {
                    var _WebClient = new WebClient();
                    _WebClient.DownloadFile(urlpath + image, SkillTree.AssetsFolderPath + image);
                }
                Images[image] = ImageHelper.OnLoadBitmapImage(new Uri(SkillTree.AssetsFolderPath + image, UriKind.Absolute));
            }
        }
    }
}