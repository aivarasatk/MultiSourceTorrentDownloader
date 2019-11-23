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
            switch (sorting)
            {
                case Sorting.LeechersAsc:
                    return (int)ThePirateBaySorting.LeechersAsc;
                case Sorting.LeecherssDesc:
                    return (int)ThePirateBaySorting.LeecherssDesc;
                case Sorting.SeedersAsc:
                    return (int)ThePirateBaySorting.SeedersAsc;
                case Sorting.SeedersDesc:
                    return (int)ThePirateBaySorting.SeedersDesc;
                case Sorting.SizeAsc:
                    return (int)ThePirateBaySorting.SizeAsc;
                case Sorting.SizeDesc:
                    return (int)ThePirateBaySorting.SizeDesc;
                case Sorting.UploadedAsc:
                    return (int)ThePirateBaySorting.UploadedAsc;
                case Sorting.UploadedDesc:
                    return (int)ThePirateBaySorting.UploadedDesc;

                default:
                    throw new ArgumentException($"Unrecognized sorting option");
            }
        }

        public static LeetxSorting SortingToLeetxSorting(Sorting sorting)
        {
            switch (sorting)
            {
                case Sorting.LeechersAsc:
                    return new LeetxSorting { Order = "asc", SortedBy = "leechers" };
                case Sorting.LeecherssDesc:
                    return new LeetxSorting { Order = "desc", SortedBy = "leechers" };
                case Sorting.SeedersAsc:
                    return new LeetxSorting { Order = "asc", SortedBy = "seeders" };
                case Sorting.SeedersDesc:
                    return new LeetxSorting { Order = "desc", SortedBy = "seeders" };
                case Sorting.SizeAsc:
                    return new LeetxSorting { Order = "asc", SortedBy = "size" };
                case Sorting.SizeDesc:
                    return new LeetxSorting { Order = "desc", SortedBy = "size" };
                case Sorting.UploadedAsc:
                    return new LeetxSorting { Order = "asc", SortedBy = "time" };
                case Sorting.UploadedDesc:
                    return new LeetxSorting { Order = "desc", SortedBy = "time" };

                default:
                    throw new ArgumentException($"Unrecognized sorting option");
            }
        }
    }
}
