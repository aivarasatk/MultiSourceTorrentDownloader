using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Mapping
{
    public static class SortingMapper
    {
        public static int SortingToThePirateBaySorting(Sorting sorting)
        {
            return sorting switch
            {
                Sorting.LeechersAsc => (int)ThePirateBaySorting.LeechersAsc,
                Sorting.LeecherssDesc => (int)ThePirateBaySorting.LeecherssDesc,
                Sorting.SeedersAsc => (int)ThePirateBaySorting.SeedersAsc,
                Sorting.SeedersDesc => (int)ThePirateBaySorting.SeedersDesc,
                Sorting.SizeAsc => (int)ThePirateBaySorting.SizeAsc,
                Sorting.SizeDesc => (int)ThePirateBaySorting.SizeDesc,
                Sorting.TimeAsc => (int)ThePirateBaySorting.UploadedAsc,
                Sorting.TimeDesc => (int)ThePirateBaySorting.UploadedDesc,
                _ => throw new ArgumentException($"Unrecognized sorting option"),
            };
        }

        public static LeetxSorting SortingToLeetxSorting(Sorting sorting)
        {
            return sorting switch
            {
                Sorting.LeechersAsc => new LeetxSorting { Order = "asc", SortedBy = "leechers" },
                Sorting.LeecherssDesc => new LeetxSorting { Order = "desc", SortedBy = "leechers" },
                Sorting.SeedersAsc => new LeetxSorting { Order = "asc", SortedBy = "seeders" },
                Sorting.SeedersDesc => new LeetxSorting { Order = "desc", SortedBy = "seeders" },
                Sorting.SizeAsc => new LeetxSorting { Order = "asc", SortedBy = "size" },
                Sorting.SizeDesc => new LeetxSorting { Order = "desc", SortedBy = "size" },
                Sorting.TimeAsc => new LeetxSorting { Order = "asc", SortedBy = "time" },
                Sorting.TimeDesc => new LeetxSorting { Order = "desc", SortedBy = "time" },
                _ => throw new ArgumentException($"Unrecognized sorting option")
            };
        }

        public static RargbSorting SortingToRargbSorting(Sorting sorting)
        {
            return sorting switch
            {
                Sorting.LeechersAsc => new RargbSorting { By = "asc", Order = "leechers" },
                Sorting.LeecherssDesc => new RargbSorting { By = "desc", Order = "leechers" },
                Sorting.SeedersAsc => new RargbSorting { By = "asc", Order = "seeders" },
                Sorting.SeedersDesc => new RargbSorting { By = "desc", Order = "seeders" },
                Sorting.SizeAsc => new RargbSorting { By = "asc", Order = "size" },
                Sorting.SizeDesc => new RargbSorting { By = "desc", Order = "size" },
                Sorting.TimeAsc => new RargbSorting { By = "asc", Order = "data" },
                Sorting.TimeDesc => new RargbSorting { By = "desc", Order = "data" },
                _ => throw new ArgumentException($"Unrecognized sorting option")
            };
        }
    }
}
