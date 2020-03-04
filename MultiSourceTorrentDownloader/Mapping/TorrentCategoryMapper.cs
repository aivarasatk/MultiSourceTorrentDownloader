using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Mapping
{
    public static class TorrentCategoryMapper
    {
        public static string ToThePirateBayCategory(TorrentCategory category)
        {
            return category switch
            {
                TorrentCategory.Applications => "300",
                TorrentCategory.Games => "400",
                TorrentCategory.Movies => "201,202,207",
                TorrentCategory.Music => "101",
                TorrentCategory.TV => "205,208",
                TorrentCategory.XXX => "500",
                _ => throw new ArgumentException($"Unexpected torrent category: {category}")
            };
        }

        public static string ToLeetxCategory(TorrentCategory category)
        {
            return DirectMapping(category);
        }

        public  static string ToRargbCategory(TorrentCategory category)
        {
            return DirectMapping(category).ToLower();
        }

        private static string DirectMapping(TorrentCategory category)
        {
            return category switch
            {
                TorrentCategory.Applications => "Apps",
                TorrentCategory.Games => "Games",
                TorrentCategory.Movies => "Movies",
                TorrentCategory.Music => "Music",
                TorrentCategory.TV => "TV",
                TorrentCategory.XXX => "XXX",
                _ => throw new ArgumentException($"Unexpected torrent category: {category}")
            };
        }
    }
}
