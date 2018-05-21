using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace STIG_Fusion.Model
{
    public class TextDocCreator
    {
        public void CreateTextDocList(MTObservableCollection<Deltas> versionDeltas)
        {
            string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = saveLocation + @"/VersionDeltas_" + DateTime.Now.ToLongDateString() + ".txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (StreamWriter sw = File.CreateText(fileName))
            {
                foreach (Deltas d in versionDeltas)
                {
                    sw.WriteLine("STIG Title: " + d.StigTitle.ToString());
                    sw.WriteLine("STIG ID: " +d.StigId.ToString());
                    sw.WriteLine("STIG Severity: " + d.StigSeverity.ToString());
                    sw.WriteLine("STIG Responsibility: " + d.StigResponsibility.ToString());
                    sw.WriteLine("");
                    sw.WriteLine("");
                }
            }
        }
    }
}
