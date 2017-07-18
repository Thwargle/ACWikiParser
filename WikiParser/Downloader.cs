using System;
using System.IO;

namespace WikiParser
{
    class Downloader
    {
        public void DownloadFile(string filename)
        {
            const string url = "http://s3.amazonaws.com/wikia_xml_dumps/a/as/asheron_backup2_pages_current.xml.7z";
            const string localzipfile = "asheron_backup2_pages_current.xml.7z";
            Log("Downloading file: " + url);
            var task = ReadToFile(url, localzipfile);
            task.Wait();
            Log("Extracting file");
            ExtractFile(localzipfile, filename);
        }
        private static void ExtractFile(string source, string destination)
        {
            var unzipper = new Unzipper();
            unzipper.ExtractFile(srcFilename: source, destFilename: destination);
        }
        private static async System.Threading.Tasks.Task ReadToFile(string url, string targetfile)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                using (var response = await client.GetAsync(url, System.Net.Http.HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    using (var httpStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (var targetStream = File.OpenWrite(targetfile))
                        {
                            await httpStream.CopyToAsync(targetStream).ConfigureAwait(false);
                        }
                    }
                }
            }
        }
        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
