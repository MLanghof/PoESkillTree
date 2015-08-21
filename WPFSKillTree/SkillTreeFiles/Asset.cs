using System;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace POESKillTree.SkillTreeFiles
{
    public class Asset
    {
        public string Name;
        public BitmapImage PImage;
        public string URL;

        public Asset(string name, string url)
        {
            Name = name;
            URL = url;
            if (!File.Exists(SkillTree.AssetsFolderPath + Name + ".png"))
            {
                var webClient = new WebClient();
                webClient.DownloadFile(URL, SkillTree.AssetsFolderPath + Name + ".png");
            }

            PImage = ImageHelper.OnLoadBitmapImage(new Uri(SkillTree.AssetsFolderPath + Name + ".png", UriKind.Absolute));
        }
    }
}