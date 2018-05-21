using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STIG_Fusion.ViewModel;

namespace STIG_Fusion.Model
{
    public class Deltas : BaseInpc
    {
        private string _stigId
        {
            get;
            set;
        }
        public string StigId
        {
            get { return _stigId; }
            set
            {
                if (_stigId != value)
                {
                    _stigId = value;
                    OnPropertyChanged("StigId");
                }
            }
        }

        private string _stigTitle
        {
            get;
            set;
        }
        public string StigTitle
        {
            get { return _stigTitle; }
            set
            {
                if (_stigTitle != value)
                {
                    _stigTitle = value;
                    OnPropertyChanged("StigTitle");
                }
            }
        }

        private string _stigSeverity
        {
            get;
            set;
        }
        public string StigSeverity
        {
            get { return _stigSeverity; }
            set
            {
                if (_stigSeverity != value)
                {
                    _stigSeverity = value;
                    OnPropertyChanged("StigSeverity");
                }
            }
        }

        private string _stigResponsibility
        {
            get;
            set;
        }
        public string StigResponsibility
        {
            get { return _stigResponsibility; }
            set
            {
                if (_stigResponsibility != value)
                {
                    _stigResponsibility = value;
                    OnPropertyChanged("StigResponsibility");
                }
            }
        }

        public Deltas(string id, string title, string severity, string responsibility)
        {
            this._stigId = id;
            this._stigTitle = title;
            this._stigSeverity = severity;
            this._stigResponsibility = responsibility;
        }

    }
}
