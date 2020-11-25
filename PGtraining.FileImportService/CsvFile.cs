using System.Collections.Generic;
using System.IO;

namespace PGtraining.FileImportService
{
    public class CsvFile
    {
        public void Import(string path)
        {
            StreamReader sr = new StreamReader(@path, System.Text.Encoding.GetEncoding("shift_jis"));
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');

                    List<string> lists = new List<string>();
                    lists.AddRange(values);

                    for (int i = 0; i < lists.Count; ++i)
                    {
                        if (lists[i] != string.Empty && lists[i].TrimStart()[0] == '"')
                        {
                            while (lists[i].TrimEnd()[lists[i].TrimEnd().Length - 1] != '"')
                            {
                                lists[i] = lists[i] + "," + lists[i + 1];
                                lists.RemoveAt(i + 1);
                            }
                        }
                    }

                    foreach (string list in lists)
                    {
                        System.Console.Write("{0} ", list);
                    }
                    System.Console.WriteLine();
                }
            }
        }
    }
}