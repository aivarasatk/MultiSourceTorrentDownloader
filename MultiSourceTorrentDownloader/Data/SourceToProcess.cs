using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class SourceToProcess
    {
        public string SourceName { get; set; }
        public SourceInformation SourceInformation { get; set; }
        public Task<bool> TaskToPerform { get; set; }
    }
}
