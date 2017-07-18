using System;
using System.IO;

namespace WikiParser
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filename = "asheron_backup2_pages_current.xml";
            DownloadFileIfNeeded(filename);
            ParseAndOutputFile(filename);
        }
        private static void DownloadFileIfNeeded(string filename)
        {
            if (!File.Exists(filename))
            {
                var downloader = new Downloader();
                downloader.DownloadFile(filename);
            }
            if (!File.Exists(filename))
            {
                throw new Exception("File not found: " + filename);
            }
        }
        private static void ParseAndOutputFile(string filename)
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                Parser parser = new Parser();
                parser.Parse(reader);
            }
            Console.ReadLine();
        }
    }
}