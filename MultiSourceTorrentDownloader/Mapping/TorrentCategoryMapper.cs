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
            switch (category)
            {
                case TorrentCategory.Applications:
                    return "300";
                case TorrentCategory.Games:
                    return "400";
                case TorrentCategory.Movies:
                    return "201,202,207";
                case TorrentCategory.Music:
                    return "101";
                case TorrentCategory.TV:
                    return "205,208";
                case TorrentCategory.XXX:
                    return "500";

                default: throw new ArgumentException($"Unexpected torrent category: {category}");
            }
        }

        public static string ToLeetxCategory(TorrentCategory category)
        {
            switch (category)
            {
                case TorrentCategory.Applications:
                    return "Apps";
                case TorrentCategory.Games:
                    return "Games";
                case TorrentCategory.Movies:
                    return "Movies";
                case TorrentCategory.Music:
                    return "Music";
                case TorrentCategory.TV:
                    return "TV";
                case TorrentCategory.XXX:
                    return "XXX";

                default: throw new ArgumentException($"Unexpected torrent category: {category}");
            }
        }
    }
}
