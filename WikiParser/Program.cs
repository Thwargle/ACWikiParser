using System;
using System.IO;

namespace WikiParser
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filename = "asheron_backup2_pages_current.xml";
            CheckFile(filename);
            ParseAndOutputFile(filename);
        }
        private static void CheckFile(string filename)
        {
            if (!File.Exists(filename))
            {
                DownloadFile(filename);
            }
            if (!File.Exists(filename))
            {
                throw new Exception("File not found: " + filename);
            }
        }
        private static void DownloadFile(string filename)
        {
            const string url = "http://s3.amazonaws.com/wikia_xml_dumps/a/as/asheron_backup2_pages_current.xml.7z";
            const string localzipfile = "asheron_backup2_pages_current.xml.7z";
            using (var client = new System.Net.WebClient())
            {
                Log("Downloading file: " + url);
                client.DownloadFile(url, localzipfile);
            }
            Log("Extracting file");
            ExtractFile(localzipfile, filename);
        }
        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
        private static string Find7zipExe()
        {
            string dirpath = GetHklmValue(@"SOFTWARE\7-Zip", "Path64", defaultValue: null);
            if (dirpath == null)
            {
                dirpath = @"C:\Program Files\7-Zip\";
            }
            string filepath = System.IO.Path.Combine(dirpath, "7zG.exe");
            if (File.Exists(filepath))
            {
                return filepath;
            }
            else
            {
                return null;
            }
        }
        private static string GetHklmValue(string key, string name, string defaultValue)
        {
            Microsoft.Win32.RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            else
                localKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);

            object value = localKey.OpenSubKey(key).GetValue(name);
            if (value != null) { return value.ToString(); }
            else { return defaultValue; }
        }
        private static void ExtractFile(string source, string destination)
        {
            string zPath = Find7zipExe();
            if (zPath == null)
            {
                throw new Exception("7Zip not found");
            }
            try
            {
                var pro = new System.Diagnostics.ProcessStartInfo();
                pro.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                pro.FileName = zPath;
                pro.Arguments = "e \"" + source + "\""; //+destination;
                var x = System.Diagnostics.Process.Start(pro);
                x.WaitForExit();
            }
            catch (System.Exception exc)
            {
                throw new Exception("Exception invoking 7Zip", exc);
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