using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class SourceState
    {
        public SourceState(string source, bool isAlive)
        {
            SourceName = source;
            IsAlive = isAlive;
        }

        public string SourceName { get; private set; }
        public bool IsAlive{ get; private set; }
    }
}
