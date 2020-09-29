using HealthCheckUI.ViewModel;
using NuGet;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HealthCheckUI.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            //Setup ViewModel
            var viewModel = new MainViewModel();
            viewModel.SiteText = "https://example.eq.edu.au";
            viewModel.OutDir = string.Format("{0}\\WFSHealthTests", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            viewModel.OutFile = "TestResults";
            viewModel.FileTypes.AddRange(new List<string>() { ".html", ".xml" });
            viewModel.SelectedType = viewModel.FileTypes[0];
            viewModel.BrowserTypes.AddRange(new List<string>() { "Chrome", "Firefox", "Edge" });
            viewModel.SelectedBrowser = viewModel.BrowserTypes[0];

            //Set Data Context and Initialize
            InitializeComponent();
            this.DataContext = viewModel;
        }

        #endregion Constructor

        #region Events

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
        }

        private void _winMain_ContentRendered(object sender, EventArgs e)
        {
            ViewModel.
        }

        #endregion Events
    }
}