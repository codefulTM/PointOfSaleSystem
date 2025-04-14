using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using PointOfSaleSystem.Views;
using PointOfSaleSystem.Services;
using DotNetEnv;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Set up connection string
            Env.Load();
            string? dbHost = Env.GetString("DB_HOST");
            string? dbUsername = Env.GetString("DB_USERNAME");
            string? dbPassword = Env.GetString("DB_PASSWORD");
            string? dbName = Env.GetString("DB_NAME");
            Configuration.CONNECTION_STRING = $"Host={dbHost};Username={dbUsername};Password={dbPassword};Database={dbName}";
            Debug.WriteLine(Configuration.CONNECTION_STRING);

            // Set up the proper IDao
            Services.Services.AddKeyedSingleton<IDao, PostgresDao>();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window? m_window;
    }
}
