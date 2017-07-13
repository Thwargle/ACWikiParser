using System;
using System.IO;

namespace WikiParser
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader reader = File.OpenText("asheron_backup2_pages_current.xml"))
            {
                Parser parser = new Parser();
                parser.Parse(reader);
            }
            Console.ReadLine();
        }
    }
}