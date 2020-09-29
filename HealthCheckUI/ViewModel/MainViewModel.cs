using HealthCheckUI.Model;
using NuGet;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HealthCheckUI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            _runTestsCommand = new DelegateCommand(OnRunTests, CanRunTests);
            _openDialogCommand = new DelegateCommand(OnOpenDialog, CanOpenDialog);
        }

        #endregion Constructor

        #region DataBindings

        private string _siteText;

        public string SiteText
        {
            get => _siteText;
            set => SetProperty(ref _siteText, value);
        }

        private string _outDir;

        public string OutDir
        {
            get => _outDir;
            set => SetProperty(ref _outDir, value);
        }

        private string _outFile;

        public string OutFile
        {
            get => _outFile;
            set => SetProperty(ref _outFile, value);
        }

        private List<string> _fileTypes = new List<string>();

        public List<string> FileTypes
        {
            get => _fileTypes;
            set => SetProperty(ref _fileTypes, value);
        }

        private string _selectedType;

        public string SelectedType

        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        private List<string> _browserTypes = new List<string>();

        public List<string> BrowserTypes
        {
            get => _browserTypes;
            set => SetProperty(ref _browserTypes, value);
        }

        private string _selectedBrowser;

        public string SelectedBrowser
        {
            get => _selectedBrowser;
            set => SetProperty(ref _selectedBrowser, value);
        }

        #endregion DataBindings

        #region CommandBindings

        #region runTests

        private readonly DelegateCommand _runTestsCommand;
        public ICommand ChangeNameCommand => _runTestsCommand;

        private void OnRunTests(object commandParameter)
        {
            string rawInput = SiteText;
            string[] lines = rawInput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            List<string> sites = new List<string>(lines);
            if (sites.Count <= 0)
            {
                return; // todo: make uo indication of lack of input
            }
            else if (sites.Count == 1)
            {
                if (!HelperFunctions.isValidURL(sites[0]))
                {
                    return; // todo: make some kind of UI indication of what went wrong.
                }
                string[] config = new string[]
{
                            HelperFunctions.getValidatedPath(OutDir),
                            OutFile,
                            SelectedType,
                            SelectedBrowser
};

                string pathToTestLibrary = Directory.GetCurrentDirectory() + @"\WFSTestFramework.dll";
                string programFiles = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                string folderName = (sites[0].Split(new[] { "://" }, StringSplitOptions.None)[1]).Split('.')[0];
                string outPath = String.Format(@"{0}config[0]\{1}\{2}{3}", config[0], folderName, OutFile, SelectedType);
                Directory.CreateDirectory(config[0] + @"\" + folderName);
                string args = string.Format("\"{0}\" --noheader --where \"test == WFSTestFramework.HealthCheck\" --tp url={1} --tp browser={2} --result={3};transform=\"{4}\\Transforms\\{5}-report.xslt\"", pathToTestLibrary, sites[0], SelectedBrowser, outPath, Directory.GetCurrentDirectory() + "\\", config[2].TrimStart('.'));

                Process process = new Process();
                process.StartInfo.FileName = string.Format(@"{0}\NUnit.org\nunit-console\nunit3-console.exe", programFiles);

                process.StartInfo.Arguments = args;
                process.Start();
            }
            else
            {
                foreach (string site in sites)
                {
                    if (!HelperFunctions.isValidURL(site))
                    {
                        return; // todo: make some kind of UI indication of what went wrong.
                    }
                    string[] config = new string[]
                    {
                            HelperFunctions.getValidatedPath(OutDir),
                            OutFile,
                            SelectedType,
                            SelectedBrowser
                        };

                    string pathToTestLibrary = Directory.GetCurrentDirectory() + "\\WFSTestFramework.dll";
                    string programFiles = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                    string folderName = site.Split(new[] { "://" }, StringSplitOptions.None)[1];
                    folderName = folderName.Split('.')[0];

                    string outPath = config[0] + "\\" + folderName + "\\" + config[1] + config[2];
                    Directory.CreateDirectory(config[0] + "\\" + folderName);
                    string testBrowser = config[3];
                    string args = string.Format("\"{0}\" --tp url={1} --wait --tp browser={2} --result={3};transform=\"{4}\\Transforms\\{5}-report.xslt\"", pathToTestLibrary, site, testBrowser, outPath, Directory.GetCurrentDirectory() + "\\", config[2].TrimStart('.'));

                    Process process = new Process();
                    process.StartInfo.FileName = string.Format("{0}\\NUnit.org\\nunit-console\\nunit3-console.exe", programFiles);

                    process.StartInfo.Arguments = args;
                    process.Start();
                }
            }

            _runTestsCommand.InvokeCanExecuteChanged();
        }

        private void RunTest()
        {
            // Implement handling of individual test to achieve better Code Abstraction
        }

        private bool CanRunTests(object commandParameter)
        {
            //todo: implement a mechanism to only enable run button when test could viably be run
            return SiteText != ("https://dakabinshs.eq.edu.au");
        }

        #endregion runTests

        #region openDialog

        private readonly DelegateCommand _openDialogCommand;
        public ICommand OpenDialogCommand => _openDialogCommand;

        private async void OnOpenDialog(object commandParameter)
        {
            var dialogContent = new TextBlock
            {
                Text = commandParameter.ToString(),
                Margin = new Thickness(20)
            };

            await MaterialDesignThemes.Wpf.DialogHost.Show(dialogContent);
        }

        private bool CanOpenDialog(object commandParameter)
        {
            return true;  // Satisfy Interface requirement but I should always be able to open dialog
        }

        #endregion openDialog

        #endregion CommandBindings

        #region UtilityFunctions

        private async void CheckForUpdate()
        {
            try
            {
                using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/mitch-beare/WFSTestFramework"))
                {
                    var release = await mgr.UpdateApp();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine;
                if (ex.InnerException != null)
                    message += ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        #endregion UtilityFunctions
    }
}