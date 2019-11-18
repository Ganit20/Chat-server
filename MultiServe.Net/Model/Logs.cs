using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiServe.Net.Model
{
    class Logs
    {
        public StreamWriter fs;
        readonly string path = "Logs";
        readonly string file = "Logs\\Logs-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        object control = new object();
        public Logs()
        {
            
            if (!Directory.Exists(path))
            {
                DirectoryInfo logs = Directory.CreateDirectory(path);
            }
            if (!File.Exists(file)) {
                fs = new StreamWriter(Path.Combine(file));
            };

        }

        public void saveLogs(string message)
        {
            lock (control) {
                using (StreamWriter sw = File.AppendText(file))
                {
                    sw.WriteLine(message);
                    sw.Close();
                } }
        }
    }
}