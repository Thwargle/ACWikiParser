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
            if (File.Exists(filename))
            {
                return;
            }
//            const string url = "http://s3.amazonaws.com/wikia_xml_dumps/a/as/asheron_backup2_pages_current.xml.7z";
            throw new Exception("File not found: " + filename);
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