namespace STIG_Fusion.Model
{
    public class StigCheck
    {
        public string Comments = string.Empty;
        public string FindingDetails = string.Empty;
        public bool? IsDocumentable = null;
        public bool Matched = false;
        public string OverrideJustification = string.Empty;
        public string Responsibility = string.Empty;
        public string RuleId = string.Empty;
        public string Severity = string.Empty;
        public string SeverityOverride = string.Empty;
        public string Status = string.Empty;
        public string Title = string.Empty;
        public string VulnId = string.Empty;

        public StigCheck() { }
    }
}
