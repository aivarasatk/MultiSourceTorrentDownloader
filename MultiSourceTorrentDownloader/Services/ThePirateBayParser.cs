using HtmlAgilityPack;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class ThePirateBayParser : IThePirateBayParser
    {
        private readonly ILogService _logger;

        public ThePirateBayParser(ILogService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<TorrentEntry>> ParsePageForTorrentEntries(string pageContents)
        {
            return await Task.Run(() =>
            {
                var htmlAgility = new HtmlDocument();
                htmlAgility.LoadHtml(pageContents);

                var tableRows = htmlAgility.DocumentNode.SelectNodes("//table[@id='searchResult']/tr");//gets table rows that contain torrent data
                if (NoTableEntries(tableRows))//probably end of results
                    return Enumerable.Empty<TorrentEntry>();

                var result = new List<TorrentEntry>();
                foreach (var dataRow in tableRows)
                {
                    var dataColumns = dataRow.SelectNodes("td[position()>1]");//skips first column because it does not contain useful info
                    if (dataColumns == null || dataColumns.Count != 3)
                    {
                        _logger.Warning($"Could not find all columns for torrent {Environment.NewLine} {dataColumns[0].OuterHtml}");
                        continue;
                    }

                    var titleNode = dataColumns[0].SelectSingleNode("div[@class='detName']/a[@class='detLink']");
                    if (titleNode == null)
                    {
                        _logger.Warning($"Could not find title node for torrent {Environment.NewLine} {dataColumns[0].OuterHtml}");
                        continue;
                    }

                    var title = titleNode.InnerText;
                    if (string.IsNullOrEmpty(title))//empty title entry makes no sense. log and skip
                    {
                        _logger.Warning($"Empty title from {Environment.NewLine}{titleNode.OuterHtml}");
                        continue;
                    }
                    var torrentUri = titleNode.Attributes?.FirstOrDefault(a => a.Name == "href")?.Value;//this field is not vital, null is acceptable

                    var magnetLink = string.Empty;
                    try
                    {
                        magnetLink = dataColumns[0].SelectNodes("a")
                                                   .First(a => a.Attributes.Any(atr => atr.Name == "href" && atr.Value.Contains("magnet")))
                                                   .Attributes.First(atr => atr.Name == "href")
                                                   .Value;
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"Could not find magnet link for {Environment.NewLine}{dataColumns[0].OuterHtml}", ex);
                        continue;//no point in showing non-downloadable entry
                    }

                    var detailsNode = dataColumns[0].SelectSingleNode("font[@class='detDesc']");
                    if (detailsNode == null)
                        _logger.Warning($"Could not find details node for {Environment.NewLine}{dataColumns[0].OuterHtml}");

                    var details = (detailsNode.InnerText + detailsNode.SelectSingleNode("a")?.InnerText).Replace("&nbsp;", " ").Split(',');//date, size, uploader
                    var date = string.Empty;
                    var size = string.Empty;
                    var uploader = string.Empty;
                    if (details.Length == 3)
                    {
                        date = details[(int)ThePirateBayDetails.Date];
                        size = details[(int)ThePirateBayDetails.Size];
                        uploader = details[(int)ThePirateBayDetails.Uploader];
                    }

                    if (!int.TryParse(dataColumns[1].InnerText, out var seeders))
                        _logger.Warning($"Could not parse seeders {Environment.NewLine}{dataColumns[1].OuterHtml}");

                    if (!int.TryParse(dataColumns[2].InnerText, out var leechers))
                        _logger.Warning($"Could not parse leechers {Environment.NewLine}{dataColumns[2].OuterHtml}");

                    result.Add(new TorrentEntry
                    {
                        Title = title,
                        TorrentUri = torrentUri,
                        TorrentMagnet = magnetLink,
                        Date = date,
                        Size = size,
                        Uploader = uploader,
                        Seeders = seeders,
                        Leechers = leechers
                    });
                }

                return result;
            });
        }

        private bool NoTableEntries(HtmlNodeCollection tableRows) => tableRows == null;
    }
}
