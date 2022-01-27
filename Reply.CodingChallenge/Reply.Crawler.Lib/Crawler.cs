using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reply.Crawler.Lib
{
    public class Crawler
    {
        private HashSet<string> _CrawledSites = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private HttpClient _Client = new HttpClient();

        // courtesy of https://stackoverflow.com/a/8049311
        private readonly Regex _HREF_REGEX = new Regex(@"<a.*?href=(""|')(?<href>.*?)(""|').*?>(?<value>.*?)</a>");

        private readonly Regex _URL_REGEX = new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$");

        private string _RootURL;

        private string _DomainToSearch;

        private Action<string> _ActionToExecute;

        public async void Start(string pSiteToCrawl, Action<string> pActionToExecuteOnEachSite)
        {
            _ActionToExecute = pActionToExecuteOnEachSite;

            _Client.BaseAddress = new System.Uri(pSiteToCrawl);
            _RootURL = pSiteToCrawl;

            GetDomain(pSiteToCrawl);

            await Crawl(pSiteToCrawl);

            _Client.Dispose();
        }

        private void GetDomain(string pSite)
        {
            var protocolSplit = pSite.Split("//");

            var domainSplit = protocolSplit[^1].Split('/');

            _DomainToSearch = domainSplit[0];
        }

        private async Task Crawl(string pSiteToCrawl)
        {
            if (_CrawledSites.Contains(pSiteToCrawl))
            {
                return;
            }

            var rawTree = await GetSite(pSiteToCrawl);

            if (rawTree == null)
            {
                return;
            }

            var reachableSites = ParseTree(rawTree);

            foreach (var reachableSite in reachableSites)
            {
                await Crawl(reachableSite);
            }
        }

        private async Task<string> GetSite(string pSite)
        {

            try
            {
                // catching relative urls
                var target = pSite[0] == '/' ?
                             _RootURL + pSite :
                             pSite;

                if (!target.Contains(_DomainToSearch))
                {
                    return null;
                }

                var response = await _Client.GetAsync(pSite);

                var body = await response.Content.ReadAsStringAsync();

                _ActionToExecute?.Invoke(body);

                _CrawledSites.Add(pSite);

                return body;
            }
            catch (Exception pExc)
            {

                throw;
            }
        }

        private List<string> ParseTree(string pRawTree)
        {
            var matches = _HREF_REGEX.Matches(pRawTree);

            var ret = new List<string>(matches.Count); // might be a bit too big, depending on how many a-elements actually contain a link.

            foreach (Match match in matches)
            {
                var url = match.Groups["href"].Value;

                // this should filter any phone numbers and links to home from home
                if (!string.Equals(url, "/") && url.Contains('/'))
                {
                    ret.Add(url);
                }
            }

            return ret;
        }
    }
}
