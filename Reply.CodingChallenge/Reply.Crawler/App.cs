using System;

namespace Reply.Crawler
{
    class App
    {
        private static Action<string> _WriteToConsoleAction = (pSite) => Console.WriteLine(pSite);

        private const string _CrawlerOutputDirectoryName = "crawleroutput";

        private static string _CrawlerOutputRootDir = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            _CrawlerOutputDirectoryName
        );

        static void Main(string[] args)
        {
            Console.WriteLine("Hello neveling reply!");

            var running = true;

            while (running)
            {
                Console.WriteLine("Enter the site you wish to crawl.");

                var targetURL = Console.ReadLine();

                var crawler = new Lib.Crawler();

                var fsAction = new Action<string, string>((path, content) => WriteToFileSystem(path, content));

                crawler.Start(targetURL, fsAction);

                Console.WriteLine("Done crawling");
            }
        }

        private static void WriteToFileSystem(string pName, string pContent)
        {

            if (pName.StartsWith("https://"))
            {
                pName = pName.Replace("https://", string.Empty);
            }

            if (pName.StartsWith("http://"))
            {
                pName = pName.Replace("http://", string.Empty);
            }

            var sanitizedName = pName.Replace('.', '_').Replace('/', '-');

            var fullPath = System.IO.Path.Combine(_CrawlerOutputRootDir, sanitizedName);

            System.IO.File.WriteAllText(fullPath + ".txt", pContent);
        }
    }
}
