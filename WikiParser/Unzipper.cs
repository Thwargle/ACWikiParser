using System;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace WikiParser
{
    class Unzipper
    {
        public void ExtractFile(string srcFilename, string destFilename)
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
                pro.Arguments = "e \"" + srcFilename + "\""; //+destination;
                var x = System.Diagnostics.Process.Start(pro);
                x.WaitForExit();
            }
            catch (System.Exception exc)
            {
                throw new Exception("Exception invoking 7Zip", exc);
            }
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
            RegistryKey localKey = OpenHklm();
            object value = localKey.OpenSubKey(key).GetValue(name);
            if (value != null) { return value.ToString(); }
            else { return defaultValue; }
        }
        private static RegistryKey OpenHklm()
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            return localKey;
        }
    }
}
