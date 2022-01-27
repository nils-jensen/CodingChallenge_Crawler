using System;

namespace Reply.Crawler
{
    class App
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var running = true;

            while (running)
            {
                Console.WriteLine("Enter the site you wish to crawl.");

                var targetURL = Console.ReadLine();

                var crawler = new Lib.Crawler();

                crawler.Start(targetURL, (pSite) => Console.WriteLine(pSite));

                Console.WriteLine("Done crawling");
            }
        }
    }
}
