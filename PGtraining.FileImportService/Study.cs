using System.Collections.Generic;

namespace PGtraining.FileImportService
{
    public class Study
    {
        public string OrderNo { get; set; }
        public string StudyDate { get; set; }
        public string ProcessingType { get; set; }
        public string InspectionTypeCode { get; set; }
        public string InspectionTypeName { get; set; }
        public string PatientId { get; set; }
        public string PatientNameKanji { get; set; }
        public string PatientNameKana { get; set; }
        public string PatientBirth { get; set; }
        public string PatientSex { get; set; }
        public List<string> MenuCodes { get; set; } = new List<string>();
        public List<string> MenuNames { get; set; } = new List<string>();

        public Study(string row)
        {
            string[] values = row.Split(',');

            if (values.Length < 12)
            {
            }

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
        }
    }
}