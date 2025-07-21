using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfApp5.Utilities;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using System.ComponentModel;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp5.Pages
{
    public partial class Library : Page
    {
        private WebClient? _webClient = null;
        private CancellationTokenSource? _cts = null;
        private string? _currentDownloadPath = null;
        private bool _isDownloading = false;
        private DateTime _downloadStartTime;

        public Library()
        {
            InitializeComponent();
            LoadPathFromFile(); // Load saved path when page initializes
        }

        private async void Library_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsProcessRunning("FortniteLauncher")
                || IsProcessRunning("FortniteClient-Win64-Shipping_EAC")
                || IsProcessRunning("Poopy-AC-Launcher")
                || IsProcessRunning("FortniteClient-Win64-Shipping_BE")
                || IsProcessRunning("FortniteClient-Win64-Shipping"))
            {
                // Your logic here if needed
            }

            /*   DoubleAnimation opacityAnimation = new DoubleAnimation
               {
                   From = 0,
                   To = 1,
                   Duration = TimeSpan.FromSeconds(.2)
               };

               DoubleAnimation slideDownAnimation = new DoubleAnimation
               {
                   From = -20,
                   To = 0,
                   Duration = TimeSpan.FromSeconds(.3)
               };

               TranslateTransform translateTransform = new TranslateTransform();
               LibraryGrid.RenderTransform = translateTransform;

               LibraryGrid.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
               translateTransform.BeginAnimation(TranslateTransform.YProperty, slideDownAnimation);*/
        }

        private async void Build1_Click(object sender, RoutedEventArgs e)
        {

            var openFileDialog = new OpenFolderDialog
            {
                Title = "Select a Folder"
            };

            FadeInLoadingBuffer();

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
            {
                ShowError("No folder selected.");
                FadeOutLoadingBuffer();
                return;
            }

            string selectedFolder = openFileDialog.FolderName;
            bool executableExists = File.Exists(Path.Combine(selectedFolder, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe"));

            if (executableExists)
            {
                string buildVersion = await Utilities.VersionChecker.GetBuildVersion(selectedFolder);
                Logs.Log($"Build Version: {buildVersion}");
                Logs.Log($"Added Build: {selectedFolder}");

                if (buildVersion == "14.60")
                {
                    // Save path in Globals
                    Globals.CurrentPath = selectedFolder;

                    // Update label
                    PathLabel.Content = selectedFolder;

                    // Save in settings too if you want (optional here)
                    Properties.Settings.Default.Path = selectedFolder;
                    Properties.Settings.Default.Save();

                    FadeOutLoadingBuffer();
                }

                else
                {
                    FadeOutLoadingBuffer();
                    ShowError("You can only play 14.60!");
                }
            }
            else
            {
                FadeOutLoadingBuffer();
                ShowError("The selected folder does not contain the required executable.");
            }
        }

        private async void ModifyBuild1Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFolderDialog
            {
                Title = "Select a Folder"
            };

            FadeInLoadingBuffer();

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
            {
                ShowError("No folder selected.");
                FadeOutLoadingBuffer();
                return;
            }

            string selectedFolder = openFileDialog.FolderName;
            bool executableExists = File.Exists(Path.Combine(selectedFolder, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe"));

            if (executableExists)
            {
                string buildVersion = await Utilities.VersionChecker.GetBuildVersion(selectedFolder);
                Logs.Log($"Build Version: {buildVersion}");
                Logs.Log($"Added Build: {selectedFolder}");

                if (buildVersion == "14.60")
                {
                    // Save the path to application user settings
                     Properties.Settings.Default.Path = selectedFolder;
                     Properties.Settings.Default.Save();

                    // Update the label with the selected folder path
                    PathLabel.Content = selectedFolder;

                    FadeOutLoadingBuffer();
                    //imag.Visibility = Visibility.Visible;
                    // bgrou.Visibility = Visibility.Visible;
                    //Build1_Copy.Visibility = Visibility.Visible;
                }
                else
                {
                    FadeOutLoadingBuffer();
                    ShowError("You can only play 14.60!");
                }
            }
            else
            {
                FadeOutLoadingBuffer();
                ShowError("The selected folder does not contain the required executable.");
            }

            FadeOutLoadingBuffer();

            // Show or hide UI based on selected path
            if (PathLabel.Content.ToString() == "Path must contain FortniteGame and Engine folders!")
            {
            }
            else
            {
            }

        }

        private void AnimateElement(UIElement element)
        {
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4)
            };

            TranslateTransform transform = element.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                element.RenderTransform = transform;
            }

            DoubleAnimation slideIn = new DoubleAnimation
            {
                From = 200, // Slide from right on X axis
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            element.Visibility = Visibility.Visible;

            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            transform.BeginAnimation(TranslateTransform.XProperty, slideIn);
        }

        private void FadeInLoadingBuffer()
        {
            LoadingGrid.Visibility = Visibility.Visible;

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            LoadingGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void FadeOutLoadingBuffer()
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            fadeOutAnimation.Completed += (s, e) =>
            {
                LoadingGrid.Visibility = Visibility.Hidden;
            };

            LoadingGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private async void ShowError(string message)
        {
            ErrorGrid.Visibility = Visibility.Visible;
            ErrorButton.Content = message;

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };
            ErrorGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            await Task.Delay(3000);

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1)
            };
            ErrorGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(1000);
            ErrorGrid.Visibility = Visibility.Collapsed;
        }

        private bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

     

        private void LoadPathFromFile()
        {
            string savedPath =  Properties.Settings.Default.Path;

            if (string.IsNullOrWhiteSpace(savedPath) || savedPath == "NONE")
            {
                Globals.CurrentPath = "Path must contain FortniteGame and Engine folders!";
                PathLabel.Content = Globals.CurrentPath;
            }
            else
            {
                Globals.CurrentPath = savedPath;
                PathLabel.Content = savedPath;
            }
        }


        private void LaunchSeason_Click1(object sender, RoutedEventArgs e)
        {

        }




        private async void Discord_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDownloading)
            {
                // Start download

                // Example download URL for Fortnite 14.60 build (replace with actual URL)
                string downloadUrl = "https://public.simplyblk.xyz/14.60.rar";

                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Fortnite 14.60 Build",
                    Filter = "RAR Files (*.rar)|*.rar",
                    FileName = "14.60.rar"
                };

                bool? result = saveFileDialog.ShowDialog();

                if (result != true)
                {
                    System.Windows.MessageBox.Show("Download canceled.");
                    return;
                }

                _currentDownloadPath = saveFileDialog.FileName;

                _webClient = new WebClient();
                _cts = new CancellationTokenSource();

                DownloadButton.Content = "Stop Download";
                _isDownloading = true;

                _webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                _webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                try
                {
                    await _webClient.DownloadFileTaskAsync(new Uri(downloadUrl), _currentDownloadPath);

                    // DownloadFileTaskAsync completes only on successful download or cancel
                    // The completion event will handle reset on success
                }
                catch (OperationCanceledException)
                {
                    // Download cancelled, handled below
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Download failed: " + ex.Message);
                    ResetDownload();
                }
            }
            else
            {
                // Stop download
                if (_webClient != null && _cts != null)
                {
                    _cts.Cancel();
                    _webClient.CancelAsync();

                    // Delete partial file if exists
                    if (!string.IsNullOrEmpty(_currentDownloadPath) && File.Exists(_currentDownloadPath))
                    {
                        try
                        {
                            File.Delete(_currentDownloadPath);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    ResetDownload();
                    System.Windows.MessageBox.Show("Download stopped and partial file deleted.");
                }
            }
        }


        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double downloadedGB = e.BytesReceived / (1024.0 * 1024 * 1024);
            double totalGB = e.TotalBytesToReceive / (1024.0 * 1024 * 1024);
            double secondsElapsed = (DateTime.Now - _downloadStartTime).TotalSeconds;
            double speedMBps = secondsElapsed > 0 ? (e.BytesReceived / 1024.0 / 1024.0) / secondsElapsed : 0;

            ProgressBar.Value = e.ProgressPercentage;
            PathLabel_Copy.Content = $"{e.ProgressPercentage}%";
            PathLabel_Copy1.Content = $"{downloadedGB:F2} GB / {totalGB:F2} GB";
            PathLabel_Copy2.Content = $"{speedMBps:F2} MB/s";
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Download cancelled.");
                DeleteFileIfExists(_currentDownloadPath);
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Download failed: " + e.Error.Message);
                DeleteFileIfExists(_currentDownloadPath);
            }
            else
            {
                MessageBox.Show("Download completed successfully!");
            }

            StopDownload();
        }

        private void StopDownload()
        {
            if (_webClient != null && _webClient.IsBusy)
            {
                _webClient.CancelAsync();
            }
            else
            {
                ResetDownload();
            }
        }

        private void ResetDownload()
        {
            DownloadButton.Content = "Start Download";
            ProgressBar.Value = 0;
            PathLabel_Copy.Content = "0%";
            PathLabel_Copy1.Content = "0 GB / 0 GB";
            PathLabel_Copy2.Content = "0 MB/s";

            if (_webClient != null)
            {
                _webClient.DownloadProgressChanged -= WebClient_DownloadProgressChanged;
                _webClient.DownloadFileCompleted -= WebClient_DownloadFileCompleted;
                _webClient.Dispose();
                _webClient = null;
            }

            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }

            _currentDownloadPath = null;
            _isDownloading = false;
        }


        private void ResetProgressUI()
        {
            ProgressBar.Value = 0;
            PathLabel_Copy.Content = "0%";
            PathLabel_Copy1.Content = "0 GB / 0 GB";
            PathLabel_Copy2.Content = "0 MB/s";
        }

        private void DeleteFileIfExists(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete incomplete file: " + ex.Message);
            }
        }

        private void Build1_Click1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Path = (string)PathLabel.Content;
            Properties.Settings.Default.Save();
        }
    }
}
