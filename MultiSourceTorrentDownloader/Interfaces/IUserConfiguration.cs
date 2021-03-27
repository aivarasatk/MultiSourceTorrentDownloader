using MultiSourceTorrentDownloader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface IUserConfiguration
    {
        Settings GetConfiguration();
        void SaveSettings<T>(T settings) where T:class;
    }
}
