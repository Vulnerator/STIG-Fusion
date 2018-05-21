using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Runtime;
using Microsoft.Win32;
using System.Windows.Input;
using STIG_Fusion.Model;

namespace STIG_Fusion.ViewModel
{
    class MainWindowViewModel : BaseInpc
    {

        #region Properties

        string progressOutput = null;

        private string _currentStig;
        public string CurrentStig
        {
            get { return _currentStig; }
            set
            {
                if (_currentStig != value)
                {
                    _currentStig = value;
                    OnPropertyChanged("CurrentStig");
                }
            }
        }

        private string _newStig;
        public string NewStig
        {
            get { return _newStig; }
            set
            {
                if (_newStig != value)
                {
                    _newStig = value;
                    OnPropertyChanged("NewStig");
                }
            }
        }

        private bool _progressRingIsActive;
        public bool ProgressRingIsActive
        {
            get { return _progressRingIsActive; }
            set
            {
                if (_progressRingIsActive != value)
                {
                    _progressRingIsActive = value;
                    OnPropertyChanged("ProgressRingIsActive");
                }
            }
        }

        private string _progressLabel = "Waiting...";
        public string ProgressLabel
        {
            get { return _progressLabel; }
            set
            {
                if (_progressLabel != value)
                {
                    _progressLabel = value;
                    OnPropertyChanged("ProgressLabel");
                }
            }
        }

        private MTObservableCollection<Deltas> _versionDeltas;
        public MTObservableCollection<Deltas> VersionDeltas
        {
            get { return _versionDeltas; }
            set
            {
                if (_versionDeltas != value)
                {
                    _versionDeltas = value;
                    OnPropertyChanged("VersionDeltas");
                }
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        private bool _ruleTitleIsChecked = false;
        public bool RuleTitleIsChecked
        {
            get { return _ruleTitleIsChecked; }
            set
            {
                if (_ruleTitleIsChecked != value)
                {
                    _ruleTitleIsChecked = value;
                    OnPropertyChanged("RuleTitleIsChecked");
                }
            }
        }

        private bool _vulnIdIsChecked = true;
        public bool VulnIdIsChecked
        {
            get { return _vulnIdIsChecked; }
            set
            {
                if (_vulnIdIsChecked != value)
                {
                    _vulnIdIsChecked = value;
                    OnPropertyChanged("VulnIdIsChecked");
                }
            }
        }

        #endregion

        #region MainWindowViewModel Constuctor

        public MainWindowViewModel()
        {
            VersionDeltas = new MTObservableCollection<Deltas>();
        }

        #endregion

        #region OpenDialogCommand

        public ICommand OpenDialogCommand
        {
            get { return new DelegateCommand(OpenDialog); }
        }

        private void OpenDialog(object param)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.CheckFileExists = true;
            openDialog.Filter = "CKL Files (*.ckl)|*.ckl";
            openDialog.Title = "Please select DISA STIG Checklist file";
            openDialog.DefaultExt = ".ckl";
            openDialog.ShowDialog();

            string p = param.ToString();

            if (p.Contains("Current"))
            {
                CurrentStig = openDialog.FileName;
            }

            else if (p.Contains("Updated"))
            {
                NewStig = openDialog.FileName;
            }

        }

        #endregion

        #region GenerateButtonCommand

        public ICommand GenerateButtonCommand
        {
            get { return new DelegateCommand(GenerateButton); }
        }
        
        private void GenerateButton()
        {
            Thread t = new Thread(new ThreadStart(WorkerThread));
            t.IsBackground = true;
            t.Start();
        }

        private void WorkerThread()
        {
            IsEnabled = false;

            ProgressRingIsActive = true;
            ProgressLabel = "Working...";

            CklParser c = new CklParser();
            progressOutput = c.ParseCkl(CurrentStig, NewStig, VulnIdIsChecked);

            if (!String.IsNullOrWhiteSpace(CurrentStig) && !String.IsNullOrWhiteSpace(NewStig))
            {
                CurrentStig = string.Empty;
                NewStig = string.Empty;
            }

            VersionDeltas = CklParser.newDeltas;
            CklParser.newDeltas = null;

            ProgressLabel = progressOutput;
            ProgressRingIsActive = false;

            IsEnabled = true;
        }

        #endregion

        #region ClearListCommand

        public ICommand ClearListCommand
        {
            get { return new DelegateCommand(ClearList); }
        }
        
        private void ClearList()
        {
            if (VersionDeltas.Count > 0)
            {
                VersionDeltas.Clear();
            }
        }

        #endregion

        #region ExportListCommand

        public ICommand ExportListCommand
        {
            get { return new DelegateCommand(ExportList); }
        }

        private void ExportList()
        {
            if (VersionDeltas.Count > 0)
            {
                TextDocCreator tdCreator = new TextDocCreator();
                tdCreator.CreateTextDocList(VersionDeltas);
                ProgressLabel = @"List Exported to Desktop/VersionDeltas_" + DateTime.Now.ToLongDateString() + ".txt";
            }
            else
            {
                ProgressLabel = "Please generate a delta list before attempting to export.";
            }
        }

        #endregion
    }
}
