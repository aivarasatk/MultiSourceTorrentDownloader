using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public abstract class ParserBase
    {
        protected virtual DateTime ParseDate(string date, string[] formats)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            return parsedDate;
        }

        protected string TrimUriStart(string uri) => uri.TrimStart(new char[] { '\\', '/' });
    }
}
