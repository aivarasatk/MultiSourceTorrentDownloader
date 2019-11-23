using Constants.MultiSourceTorrentDownloader;
using HtmlAgilityPack;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class LeetxParser : ILeetxParser
    {
        private readonly ILogService _logger;
        public LeetxParser(ILogService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TorrentQueryResult> ParsePageForTorrentEntries(string pageContents)
        {
            return await Task.Run(() =>
            {
                _logger.Information("Leetx parsing");
                var htmlAgility = new HtmlDocument();
                htmlAgility.LoadHtml(pageContents);

                var tableRows = htmlAgility.DocumentNode.SelectNodes("//table[@class='table-list table table-responsive table-striped']/tbody/tr");//gets table rows that contain torrent data
                if (NoTableEntries(tableRows))
                    return new TorrentQueryResult { LastPage = true };

                var result = new List<TorrentEntry>();
                foreach (var dataRow in tableRows)
                {
                    var dataColumns = dataRow.SelectNodes("td");
                    if (dataColumns == null || dataColumns.Count != 6)
                    {
                        _logger.Warning($"Could not find all columns for torrent {Environment.NewLine} {dataRow.OuterHtml}");
                        continue;
                    }

                    var titleNode = dataColumns[LeetxTorrentColumnIndexer.Name]
                                    .SelectNodes("//td[@class='coll-1 name']/a")?
                                    .FirstOrDefault(a => a.Attributes.Any(atr => atr.Name == "href" && atr.Value.Contains("torrent")));
                    if (titleNode == null)
                    {
                        _logger.Warning($"Could not find title node for torrent {Environment.NewLine} {dataColumns[LeetxTorrentColumnIndexer.Name].OuterHtml}");
                        continue;
                    }

                    var title = titleNode.InnerText;
                    if (string.IsNullOrEmpty(title))//empty title entry makes no sense. log and skip
                    {
                        _logger.Warning($"Empty title from {Environment.NewLine}{titleNode.OuterHtml}");
                        continue;
                    }

                    var torrentUri = titleNode.Attributes.FirstOrDefault(a => a.Name == "href")?.Value;
                    if (string.IsNullOrEmpty(torrentUri))
                    {
                        _logger.Warning($"Empty torrent uri from{Environment.NewLine}{titleNode.OuterHtml}");
                        continue;
                    }

                    var magnetLink = string.Empty;
                    
                    if (!int.TryParse(dataColumns[LeetxTorrentColumnIndexer.Seeders].InnerText, out var seeders))
                        _logger.Warning($"Could not parse seeders {Environment.NewLine}{dataColumns[LeetxTorrentColumnIndexer.Seeders].OuterHtml}");

                    if (!int.TryParse(dataColumns[LeetxTorrentColumnIndexer.Leechers].InnerText, out var leechers))
                        _logger.Warning($"Could not parse leechers {Environment.NewLine}{dataColumns[LeetxTorrentColumnIndexer.Leechers].OuterHtml}");

                    var date = dataColumns[LeetxTorrentColumnIndexer.Date].InnerText;
                    var size = dataColumns[LeetxTorrentColumnIndexer.Size].InnerText;
                    var uploader = dataColumns[LeetxTorrentColumnIndexer.Uploader].SelectSingleNode("a")?.InnerText;

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

                var pagination = htmlAgility.DocumentNode.SelectNodes("//div[@class='pagination']");
                return new TorrentQueryResult
                {
                    TorrentEntries = result,
                    LastPage = pagination == null
                };
            });
        }

        private bool NoTableEntries(HtmlNodeCollection tableRows) => tableRows == null;
    }
}
