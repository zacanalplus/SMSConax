using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConaxSMS
{
    class CSVParser
    {
        public string CSVFilename { get; set; }
        private bool csvOK;
        private List<List<string>> lst = new List<List<string>>();
        private int nlines = 0;
        public bool csvReady
        {
            get
            {
                return csvOK;
            }
            private set
            {
                csvOK = value;
            }
        }
        private int CountLines(string filePath)
        {
            int lineCount = 0;
            using (StreamReader reader = File.OpenText(filePath))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }
            return lineCount;
        }
        public CSVParser(string csvName)
        {
            csvOK = false;
            if (csvName.Length > 0 && File.Exists(csvName))
            {
                CSVFilename = csvName;
                csvOK = true;
            }
            nlines = CountLines(csvName);
        }
        public List<string> LoadL()
        {
            List<string> lns = new List<string>();
            using (StreamReader reader = new StreamReader(CSVFilename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lns.Add(line);
                }
            }
            return lns;
        }
        public void Load(int nCols = 1, char csvDelimiter = ' ')
        {
            using (StreamReader reader = new StreamReader(CSVFilename))
            {
                List<string>[] innLst = new List<string>[nlines];
                for (int i = 0; i < nlines; i++)
                    innLst[i] = new List<string>();
                int lno = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(csvDelimiter);

                    for (int i = 0; i < nCols; i++)
                    {
                        innLst[lno].Add(values[i]);
                    }
                    lno++;
                }
                for (int i = 0; i < nlines; i++)
                    lst.Add(innLst[i]);
            }
        }
        public string[] getItem(int idx)
        {
            if (idx >= 0 && idx < lst.Count)
                return lst[idx].ToArray<string>();
            return null;
        }
        public IEnumerable<List<string>> GetV()
        {
            for (int i = 0; i < lst.Count; i++)
            {
                yield return lst[i];
            }
        }
    }
}
