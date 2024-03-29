﻿using Constants.MultiSourceTorrentDownloader;
using HtmlAgilityPack;
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
    public class LeetxParser : ParserBase, ILeetxParser
    {
        private readonly ILogService _logger;

        private readonly string[] _formats = new string[]
        {
            "htt MMM. d\\t\\h", //11am Nov. 8th
            "htt MMM. d\\s\\t", //11am Nov. 1st
            "htt MMM. dn\\d",   //11am Nov. 2nd
            "htt MMM. dr\\d",   //11am Nov. 3rd

            "MMM. d\\t\\h \\'yy", //Oct. 9th '19
            "MMM. d\\s\\t \\'yy", //Oct. 1st '19
            "MMM. dn\\d \\'yy",   //Oct. 2nd '19
            "MMM. dr\\d \\'yy",   //Oct. 3rd '19
        };

        public LeetxParser(ILogService logger)
        {
            DataColumnCount = 6;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TorrentQueryResult> ParsePageForTorrentEntriesAsync(string pageContents)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.Information("Leetx parsing");
                    var htmlAgility = new HtmlDocument();
                    htmlAgility.LoadHtml(pageContents);

                    var tableRows = htmlAgility.DocumentNode.SelectNodes("//table[@class='table-list table table-responsive table-striped']/tbody/tr");//gets table rows that contain torrent data
                    if (NoTableEntries(tableRows))
                        return new TorrentQueryResult { IsLastPage = true };

                    var result = new List<TorrentEntry>();
                    foreach (var dataRow in tableRows)
                    {
                        var dataColumns = dataRow.SelectNodes("td");
                        if (dataColumns == null || dataColumns.Count != DataColumnCount)
                        {
                            _logger.Warning($"Could not find all columns for torrent {Environment.NewLine} {dataRow.OuterHtml}");
                            continue;
                        }

                        var titleNode = dataColumns[LeetxTorrentIndexer.Name]
                                        .SelectNodes("a")?
                                        .FirstOrDefault(a => a.Attributes.Any(atr => atr.Name == "href" && atr.Value.Contains("torrent")));
                        if (titleNode == null)
                        {
                            _logger.Warning($"Could not find title node for torrent {Environment.NewLine} {dataColumns[LeetxTorrentIndexer.Name].OuterHtml}");
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

                        if (!int.TryParse(dataColumns[LeetxTorrentIndexer.Seeders].InnerText, out var seeders))
                            _logger.Warning($"Could not parse seeders {Environment.NewLine}{dataColumns[LeetxTorrentIndexer.Seeders].OuterHtml}");

                        if (!int.TryParse(dataColumns[LeetxTorrentIndexer.Leechers].InnerText, out var leechers))
                            _logger.Warning($"Could not parse leechers {Environment.NewLine}{dataColumns[LeetxTorrentIndexer.Leechers].OuterHtml}");

                        var date = dataColumns[LeetxTorrentIndexer.Date].InnerText;

                        var size = dataColumns[LeetxTorrentIndexer.Size].InnerHtml.Substring(0, dataColumns[LeetxTorrentIndexer.Size].InnerHtml.IndexOf('<'));
                        var uploader = dataColumns[LeetxTorrentIndexer.Uploader].SelectSingleNode("a")?.InnerText;

                        var splitSize = size.Split(' ');
                        result.Add(new TorrentEntry
                        {
                            Title = title,
                            TorrentUri = TrimUriStart(torrentUri),
                            TorrentMagnet = magnetLink,
                            Date = ParseDate(date, _formats),
                            Size = new SizeEntity
                            {
                                Value = double.Parse(splitSize[0], CultureInfo.InvariantCulture),
                                Postfix = splitSize[1]
                            },
                            Uploader = uploader,
                            Seeders = seeders,
                            Leechers = leechers,
                            Source = TorrentSource.Leetx
                        });
                    }

                    var pagination = htmlAgility.DocumentNode.SelectSingleNode("//div[@class='pagination']");

                    return new TorrentQueryResult
                    {
                        TorrentEntries = result,
                        IsLastPage = pagination == null || !pagination.InnerHtml.Contains("href")
                    };
                }
                catch(Exception ex)
                {
                    _logger.Warning("1337X parse exception", ex);
                    throw;
                }
                
            });
        }

        private bool NoTableEntries(HtmlNodeCollection tableRows) => tableRows == null;

        public async Task<string> ParsePageForMagnetAsync(string pageContents)
        {
            _logger.Information("Leetx magnet parsing parsing");

            return await Task.Run(() =>
            {
                var htmlAgility = LoadedHtmlDocument(pageContents);

                var magnetNode = htmlAgility.DocumentNode
                                            .SelectNodes("//a")
                                            .FirstOrDefault(a => a.Attributes.Any(atr => atr.Name == "href" && atr.Value.Contains("magnet"))
                                                && a.InnerText.Equals("Magnet Download", StringComparison.InvariantCultureIgnoreCase));

                if (magnetNode == null)
                    throw new Exception($"Magnet node is not found");

                return magnetNode.Attributes.First(m => m.Name == "href").Value;
            });
        }

        public async Task<string> ParsePageForDescriptionHtmlAsync(string pageContents)
        {
            return await Task.Run(() =>
            {
                var htmlDocument = LoadedHtmlDocument(pageContents);

                var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='description']");
                if (descriptionNode == null)
                {
                    _logger.Warning("Could not find description node for 1337X");
                    return string.Empty;
                }
                //hack around images. downloaded img src is pointing to empty .svg. need to redirect to data-original
                return descriptionNode.InnerHtml.Replace("src", "nothing").Replace("data-original", "src");
            });
        }
    }
}
