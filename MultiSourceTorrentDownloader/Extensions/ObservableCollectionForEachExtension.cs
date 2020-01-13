using System;
using System.Collections.ObjectModel;

namespace MultiSourceTorrentDownloader.Extensions
{
    public static class ObservableCollectionForEachExtension
    {
        public static void ForEach<T>(this ObservableCollection<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action.Invoke(item);
            }
        }
    }
}
