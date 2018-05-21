using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace STIG_Fusion.Model
{
    public class CklParser
    {
        public static MTObservableCollection<Deltas> newDeltas = new MTObservableCollection<Deltas>(); 
        
        public string ParseCkl(string currentStig, string newStig, bool vulnIdIsChecked)
        {
            try
            {
                if (newDeltas == null)
                { newDeltas = new MTObservableCollection<Deltas>(); }

                if (String.IsNullOrWhiteSpace(currentStig) | String.IsNullOrWhiteSpace(newStig))
                { return "Empty field(s)"; }

                if (currentStig.Equals(newStig))
                { return "Current artifact and updated artifact may not be the same file"; }

                List<StigCheck> currentStigChecks = IngestChecklist(currentStig);
                List<StigCheck> newStigChecks = IngestChecklist(newStig);
                CompareChecklists(currentStigChecks, newStigChecks, vulnIdIsChecked);
                WriteToNewCkl(newStigChecks, newStig);
                UpdateView(newDeltas, newStigChecks);

                return "Overlay complete";
            }
            catch (Exception exception)
            { return "Error"; }
        }

        private List<StigCheck> IngestChecklist(string checklist)
        {
            List<StigCheck> stigList = new List<StigCheck>();
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(checklist);
            foreach (DataRow dataRow in dataSet.Tables["VULN"].Rows)
            {
                StigCheck stigCheck = new StigCheck();
                stigCheck.Status = dataRow["STATUS"].ToString();
                stigCheck.FindingDetails = dataRow["FINDING_DETAILS"].ToString();
                stigCheck.Comments = dataRow["COMMENTS"].ToString();
                stigCheck.SeverityOverride = dataRow["SEVERITY_OVERRIDE"].ToString();
                stigCheck.OverrideJustification = dataRow["SEVERITY_JUSTIFICATION"].ToString();
                ParseStigChildNodes(dataRow, stigCheck);
                stigList.Add(stigCheck);
            }
            return stigList;
        }

        private void ParseStigChildNodes(DataRow dataRow, StigCheck stigCheck)
        {
            DataRow[] childRows = dataRow.GetChildRows("VULN_STIG_DATA");
            foreach (DataRow childRow in childRows)
            {
                switch (childRow["VULN_ATTRIBUTE"].ToString())
                {
                    case "Vuln_Num":
                        {
                            stigCheck.VulnId = childRow["ATTRIBUTE_DATA"].ToString();
                            break;
                        }
                    case "Severity":
                        {
                            stigCheck.Severity = childRow["ATTRIBUTE_DATA"].ToString();
                            break;
                        }
                    case "Rule_Title":
                        {
                            stigCheck.Title = childRow["ATTRIBUTE_DATA"].ToString();
                            break;
                        }
                    case "Rule_ID":
                        {
                            stigCheck.RuleId = childRow["ATTRIBUTE_DATA"].ToString();
                            break;
                        }
                    case "Responsibility":
                        {
                            stigCheck.Responsibility = childRow["ATTRIBUTE_DATA"].ToString();
                            break;
                        }
                    case "Documentable":
                        {
                            stigCheck.IsDocumentable = bool.Parse(childRow["ATTRIBUTE_DATA"].ToString());
                            break;
                        }
                    default:
                        { break; }
                }
            }
        }

        private void CompareChecklists(List<StigCheck> currentStig, List<StigCheck> newStig, bool vulnIdIsChecked)
        {
            if (vulnIdIsChecked)
            {
                foreach(StigCheck stigCheck in currentStig)
                {
                    foreach (StigCheck newStigCheck in newStig.Where(x => !x.Matched))
                    {
                        if (newStigCheck.VulnId == stigCheck.VulnId)
                        {
                            newStigCheck.FindingDetails = stigCheck.FindingDetails;
                            newStigCheck.Comments = stigCheck.Comments;
                            newStigCheck.SeverityOverride = stigCheck.SeverityOverride;
                            newStigCheck.OverrideJustification = stigCheck.OverrideJustification;
                            newStigCheck.Status = stigCheck.Status;
                            newStigCheck.Matched = true;
                        }
                    }
                }
            }
            else
            {
                foreach(StigCheck stigCheck in currentStig)
                {
                    foreach(StigCheck newStigCheck in newStig.Where(x => !x.Matched))
                    {
                        if (StringSimilarityComparisson.CompareTitles(stigCheck.Title, newStigCheck.Title))
                        {
                            newStigCheck.FindingDetails = stigCheck.FindingDetails;
                            newStigCheck.Comments = stigCheck.Comments;
                            newStigCheck.SeverityOverride = stigCheck.SeverityOverride;
                            newStigCheck.OverrideJustification = stigCheck.OverrideJustification;
                            newStigCheck.Status = stigCheck.Status;
                            newStigCheck.Matched = true;
                        }
                    }
                }
            }
        }

        private void WriteToNewCkl(List<StigCheck> newStigChecks, string newStig)
        {
            XDocument stigFile = XDocument.Load(newStig);
            IEnumerable<XElement> vulnElements = stigFile.Descendants().Elements("VULN");
            foreach (StigCheck stigCheck in newStigChecks)
            {
                XElement vulnElement = stigFile.Root.Descendants().FirstOrDefault(
                    x => x.Name.LocalName == "ATTRIBUTE_DATA" && x.Value == stigCheck.RuleId).Parent.Parent;
                vulnElement.Element("STATUS").Value = stigCheck.Status;
                vulnElement.Element("FINDING_DETAILS").Value = stigCheck.FindingDetails;
                vulnElement.Element("COMMENTS").Value = stigCheck.Comments;
                vulnElement.Element("SEVERITY_OVERRIDE").Value = stigCheck.SeverityOverride;
                vulnElement.Element("SEVERITY_JUSTIFICATION").Value = stigCheck.OverrideJustification;
            }
            stigFile.Save(newStig);
        }

        private void UpdateView(MTObservableCollection<Deltas> newDeltas, List<StigCheck> newStigChecks)
        {
            foreach (StigCheck stigCheck in newStigChecks.Where(x => !x.Matched))
            {
                newDeltas.Add(
                    new Deltas(
                        stigCheck.VulnId, stigCheck.Title, ConvertSeverity(stigCheck.Severity), stigCheck.Responsibility));
            }
        }

        private string ConvertSeverity(string severity)
        {
            switch (severity)
            {
                case "high":
                    { return "High"; }
                case "medium":
                    { return "Medium"; }
                case "low":
                    { return "Low"; }
                case "unknown":
                    { return "Unknown"; }
                default:
                    { return "Unknown"; }
            }
        }
    }
}
