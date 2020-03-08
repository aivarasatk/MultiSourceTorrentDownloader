using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public abstract class ParserBase
    {
        protected int DataColumnCount;
        protected virtual DateTime ParseDate(string date, string[] formats)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            return parsedDate;
        }

        protected string TrimUriStart(string uri) => uri.TrimStart(new char[] { '\\', '/' });

        protected HtmlDocument LoadedHtmlDocument(string pageContents)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContents);
            return htmlDocument;
        }

        protected async Task<string> BaseParseMagnet(string pageContents)
        {
            return await Task.Run(() =>
            {
                var htmlAgility = LoadedHtmlDocument(pageContents);

                var magnetNode = htmlAgility.DocumentNode
                                            .SelectNodes("//a")
                                            .FirstOrDefault(a => a.Attributes.Any(atr => atr.Name == "href" && atr.Value.Contains("magnet")));

                if (magnetNode == null)
                    throw new Exception($"Magnet node is not found");

                return magnetNode.Attributes.First(m => m.Name == "href").Value;
            });
        }
    }
}
