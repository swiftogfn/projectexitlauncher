using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using WpfApp5.Pages;
using WpfApp5.Utilities;

namespace WpfApp5
{
    public partial class MainWindow : FluentWindow
    {

        private string _loggedInUsername;
        public MainWindow()
        {
            InitializeComponent();
            StartEpicGamesBlocker();

            if (Properties.Settings.Default.AutoLogin && Properties.Settings.Default.Loggedin == true) // probably should make this more safe because someone could just change the vars in property settings
            {
                MainFrame.Content = new Navigation();
            }
            else
            {
                MainFrame.Content = new Login();
            }

            Logs.Log("Launcher Started"); 
        }
        public class Skin
        {
            public string SkinName { get; set; }
            public string SkinImageUrl { get; set; }
        }

        private CancellationTokenSource _epicMonitorToken;
        public void SetLoggedInUsername(string username)
        {
            _loggedInUsername = username;
            // Optionally do something else with username here
        }
        private void StartEpicGamesBlocker()
        {
            _epicMonitorToken = new CancellationTokenSource();
            CancellationToken token = _epicMonitorToken.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var epicProcesses = Process.GetProcessesByName("EpicGamesLauncher");
                        foreach (var proc in epicProcesses)
                        {
                            try { proc.Kill(); } catch { }
                        }
                    }
                    catch { }

                    await Task.Delay(1000); // Check every second
                }
            }, token);
        }
        private void LauncherClosed(object sender, EventArgs e)
        {
            RPC.StopRPC();
            Utils.KillProcess("FortniteClient-Win64-Shipping");
            Utils.KillProcess("FortniteClient-Win64-Shipping_BE");
            Utils.KillProcess("FortniteClient-Win64-Shipping_EAC");
            Utils.KillProcess("FortniteLauncher");
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}