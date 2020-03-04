using HtmlAgilityPack;
using MultiSourceTorrentDownloader.Constants;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class RargbParser : ParserBase, IRargbParser
    {
        private readonly ILogService _logger;

        private const int DataColumnCount = 8;

        public RargbParser(ILogService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> ParsePageForDescriptionHtmlAsync(string pageContents)
        {
            return await Task.Run(() =>
            {
                var htmlDocument = LoadedHtmlDocument(pageContents);

                var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//td[@id='description']");
                if (descriptionNode == null)
                {
                    _logger.Warning("Could not find description node for RARGB");
                    return string.Empty;
                }

                return descriptionNode.InnerHtml;
            });
        }

        public async Task<string> ParsePageForMagnetAsync(string pageContents)
        {
            return await Task.Run(() =>
            {
                _logger.Information("RARGB magnet parsing parsing");
                var htmlAgility = LoadedHtmlDocument(pageContents);

                var magnetNode = htmlAgility.DocumentNode
                                            .SelectNodes("//a")
                                            .FirstOrDefault(a => a.Attributes.Any(atr => atr.Name == "href" && atr.Value.Contains("magnet")));

                if (magnetNode == null)
                    throw new Exception($"Magnet node is not found");

                return magnetNode.Attributes.First(m => m.Name == "href").Value;
            });
        }

        public async Task<TorrentQueryResult> ParsePageForTorrentEntriesAsync(string pageContents)
        {
            return await Task.Run(() => 
            {
                try
                {
                    _logger.Information("RARGB parsing");
                    var htmlAgility = new HtmlDocument();
                    htmlAgility.LoadHtml(pageContents);

                    var tableRows = htmlAgility.DocumentNode.SelectNodes("//tr[@class='lista2']");//gets table rows that contain torrent data
                    if (NoTableEntries(tableRows))
                        return new TorrentQueryResult { IsLastPage = true };

                    var result = new List<TorrentEntry>();
                    foreach (var row in tableRows)
                    {
                        var columns = row.SelectNodes("td");
                        if(columns == null || columns.Count != DataColumnCount)
                        {
                            _logger.Warning($"Could not find all columns for torrent {Environment.NewLine} {row.OuterHtml}");
                            continue;
                        }

                        var titleNode = columns[RargbTorrentIndexer.Name]
                                        .SelectSingleNode("a");
                        if (titleNode == null)
                        {
                            _logger.Warning($"Could not find title node for torrent {Environment.NewLine} {columns[RargbTorrentIndexer.Name].OuterHtml}");
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

                        if (!int.TryParse(columns[RargbTorrentIndexer.Seeders].InnerText, out var seeders))
                            _logger.Warning($"Could not parse seeders {Environment.NewLine}{columns[RargbTorrentIndexer.Seeders].OuterHtml}");

                        if (!int.TryParse(columns[RargbTorrentIndexer.Leechers].InnerText, out var leechers))
                            _logger.Warning($"Could not parse leechers {Environment.NewLine}{columns[RargbTorrentIndexer.Leechers].OuterHtml}");

                        var date = columns[RargbTorrentIndexer.Date].InnerText;

                        var size = columns[RargbTorrentIndexer.Size].InnerText;
                        var uploader = columns[RargbTorrentIndexer.Uploader].InnerText;

                        var splitSize = size.Split(' ');
                        result.Add(new TorrentEntry
                        {
                            Title = title,
                            TorrentUri = TrimUriStart(torrentUri),
                            TorrentMagnet = magnetLink,
                            Date = DateTime.Parse(date),
                            Size = new SizeEntity
                            {
                                Value = double.Parse(splitSize[0], CultureInfo.InvariantCulture),
                                Postfix = splitSize[1]
                            },
                            Uploader = uploader,
                            Seeders = seeders,
                            Leechers = leechers,
                            Source = TorrentSource.Rargb
                        });
                    }

                    var pagination = htmlAgility.DocumentNode.SelectSingleNode("//div[@id='pager_links']");

                    return new TorrentQueryResult
                    {
                        TorrentEntries = result,
                        IsLastPage = IsLastPage(pagination)
                    };

                }
                catch(Exception ex)
                {
                    _logger.Warning("RARGB parse exception", ex);
                    throw;
                }
            });
        }

        private bool IsLastPage(HtmlNode pagination) => pagination == null || pagination.SelectNodes("a").All(n => n.InnerText != ">>");

        private bool NoTableEntries(HtmlNodeCollection tableRows) => tableRows == null;

        protected override string ParseSizePostfix(string postfix)
        {
            throw new NotImplementedException();
        }
    }
}
