﻿using Autodesk.Revit.UI;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.FamilyBrowser
{
    /// <summary>
    /// Interaction logic for FamilyBrowserWPF.xaml
    /// </summary>
    public partial class FamilyBrowserWPF : Page, IDockablePaneProvider
    {
        string _contentLibraryPath;
        string ContentLibraryPath { get; set; }

        /// <summary>
        /// Method to start and populate the custom window
        /// </summary>
        public FamilyBrowserWPF()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method to set initial dock panel state 
        /// </summary>
        /// <param name="data"></param>
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            // WPF window as dockable pane
            data.FrameworkElement = this as FrameworkElement;
            // Set initial state
            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Tabbed,
                TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
            };
        }
    }
}
