using DiscordRPC;
using PlooshLauncher;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfApp5.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Net.Http;

namespace WpfApp5.Pages
{
    public partial class Home : Page
    {
        private const string FolderName = "EasyAntiCheat";

        private const string ExitEacUrl = "https://github.com/swiftogfn/exiteac/releases/download/ExitEAC/Exit_EAC.exe";
        //   private const string SplashScreenUrl = "https://github.com/sweartsfn/dfghdfgghdfghg/releases/download/sdfg/SplashScreen.png";

        private const string SplashScreenUrl = "https://github.com/swiftogfn/exiteac/releases/download/ExitEAC/SplashScreen.png";
        private const string ExeFileName = "Exit_EAC.exe";
        private const string ImageFileName = "SplashScreen.png";
        private const string SettingsFileName = "Settings.json";
        private string selectedFolderPath;
        private bool isLaunching = false;
        private bool isGameRunning = false;
        private bool editOnReleaseEnabled = false;
        private const string EOR_DLL_URL = "https://cdn.discordapp.com/attachments/1343293975473426502/1378429539700576377/EOR.dll?ex=683c920c&is=683b408c&hm=16f3935188fdc734dfe743b1373b6a4b864ebd6d0700eb54559099eb09c39a1a&";


        // List of Fortnite skin head image URLs with transparent backgrounds
        private readonly string[] skinHeadUrls = new string[]
        {
            "https://tryhardguides.com/wp-content/uploads/2021/08/focus-icon.png",
            "https://media.fortniteapi.io/images/59eb4f6-e81c036-42fab23-375205c/transparent.png",
            "https://www.pngmart.com/files/12/Helmet-Fortnite-Skin-PNG-Transparent-Image.png",
            "https://clipartcraft.com/images/renegade-raider-clipart-6.png",
            "https://tryhardguides.com/wp-content/uploads/2021/08/blueprint-style-1.png",
            "https://gamepedia.cursecdn.com/fortnite_gamepedia/thumb/b/be/Elite_Agent_No_Helmet.png/120px-Elite_Agent_No_Helmet.png?version=86a0e6693bcc691d59c93506713e4cff",
            "https://www.pngarts.com/files/5/Catwoman-Fortnite-PNG-Image.png",
            "https://tryhardguides.com/wp-content/uploads/2021/08/slingshot-style.png",
            "https://clipart.info/images/ccovers/1547067425fortnite-icon-character-36.png",
            "https://gamepedia.cursecdn.com/fortnite_gamepedia/1/1d/New_Sparkplug.png",
            "https://gamepedia.cursecdn.com/fortnite_gamepedia/f/fb/New_Banner_Trooper.png",
            "https://www.pngmart.com/files/22/Fortnite-Travis-Scott-PNG.png",
            // "https://www.theaudiodb.com/images/media/artist/cutout/rid3171695285723.png",
            "https://pngimg.com/uploads/fortnite/fortnite_PNG109.png",
            "https://media.fortniteapi.io/images/10152349852b512cf59d93156e451ca7/transparent.png",
            //"https://tryhardguides.com/wp-content/uploads/2022/11/iconic-style-1.png",
            //"https://tryhardguides.com/wp-content/uploads/2022/11/iconic-style-1.png",
           // "https://tryhardguides.com/wp-content/uploads/2022/11/iconic-style-1.png",



        };

        private readonly Random random = new Random();

        public Home(string? username)
        {
            InitializeComponent();
            UsernameLabel.Text = "";
            GreetingTextBlock.Text = GetGreeting(username);
            LoadSavedPath();

        }
        private string GetGreeting(string? username)
        {
            int hour = DateTime.Now.Hour;

            if (hour >= 0 && hour < 6)
                return $"Good night, {username}!";
            else if (hour >= 6 && hour < 12)
                return $"Good morning, {username}!";
            else if (hour >= 12 && hour < 18)
                return $"Good afternoon, {username}!";
            else
                return $"Good evening, {username}!";
        }



        /*    private void SetDiscordUserAvatar(User user)
            {
                try
                {
                    string avatarUrl = user.GetAvatarURL(User.AvatarFormat.PNG);

                    if (!string.IsNullOrEmpty(avatarUrl))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            DiscordAva.Background = new ImageBrush
                            {
                                ImageSource = new BitmapImage(new Uri(avatarUrl)),
                                Stretch = Stretch.UniformToFill
                            };
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            DiscordAva.Background = new SolidColorBrush(Colors.Gray);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logs.Log("Failed to set Discord avatar: " + ex.Message);
                    Dispatcher.Invoke(() =>
                    {
                        DiscordAva.Background = new SolidColorBrush(Colors.Gray);
                    });
                }
            }*/

        private void HomeLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RPC.client != null && RPC.client.CurrentUser != null)
                {
                    var bitmapImage = new BitmapImage(new Uri(RPC.client.CurrentUser.GetAvatarURL(User.AvatarFormat.PNG)));

                }
                else
                {
                    var defaultAvatarImage = new BitmapImage(new Uri("pack://application:,,,/Assets/DefaultAvatar.png"));

                }
            }
            catch (Exception ex)
            {
                Logs.Log("Failed to load RPC: " + ex.Message);
                var defaultAvatarImage = new BitmapImage(new Uri("pack://application:,,,/Assets/DefaultAvatar.png"));

            }

            DoubleAnimation opacityAnimation = new DoubleAnimation
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
            translateTransform.BeginAnimation(TranslateTransform.YProperty, slideDownAnimation);


            LoadRandomSkinHead();
        }

        private void LoadSavedPath()
        {
            string savedPath = Properties.Settings.Default.Path;

            if (string.IsNullOrWhiteSpace(savedPath) || savedPath == "NONE")
            {
                Globals.CurrentPath = "No Folder Selected";
            }
            else
            {
                Globals.CurrentPath = savedPath;
            }
        }

        private void LoadRandomSkinHead()
        {
            if (skinHeadUrls.Length == 0)
                return;

            int index = random.Next(skinHeadUrls.Length);

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(skinHeadUrls[index]);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                SkinImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                Logs.Log("Failed to load skin image: " + ex.Message);
            }
        }

        private void NewsButton2_Click(object sender, RoutedEventArgs e) { }
        private void NewsButton1_Click(object sender, RoutedEventArgs e) { }
        private void NewsButton3_Click(object sender, RoutedEventArgs e) { }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://public.simplyblk.xyz/14.60.rar",
                UseShellExecute = true
            });
        }

        private void AnimateElement(UIElement element)
        {
            DoubleAnimation fadeIn = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromSeconds(0.4) };
            TranslateTransform transform = element.RenderTransform as TranslateTransform ?? new TranslateTransform();
            element.RenderTransform = transform;

            DoubleAnimation slideUp = new DoubleAnimation
            {
                From = 20,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.4),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            element.Visibility = Visibility.Visible;
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            transform.BeginAnimation(TranslateTransform.YProperty, slideUp);
        }

        private void NewsBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void AnimateOut(UIElement element)
        {
            DoubleAnimation fadeOut = new DoubleAnimation { From = 1, To = 0, Duration = TimeSpan.FromSeconds(0.3) };
            TranslateTransform transform = element.RenderTransform as TranslateTransform ?? new TranslateTransform();
            element.RenderTransform = transform;

            DoubleAnimation slideDown = new DoubleAnimation
            {
                From = 0,
                To = 20,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) => element.Visibility = Visibility.Collapsed;
            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            transform.BeginAnimation(TranslateTransform.YProperty, slideDown);
        }


        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {

        }
        private void StartAntiInjectorWatchdog(Process fortniteProc)
        {
            Task.Run(async () =>
            {
                string[] blacklistedProcesses = { "UUUClient", "CheatEngine", "x64dbg", "ProcessHacker" };

                while (!fortniteProc.HasExited)
                {
                    foreach (var proc in Process.GetProcesses())
                    {
                        try
                        {
                            if (blacklistedProcesses.Any(name => proc.ProcessName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0))
                            {
                                fortniteProc.Kill();
                                //MessageBox.Show($"Fortnite was terminated due to detection of {proc.ProcessName}.");
                                return;
                            }
                        }
                        catch { }
                    }

                    await Task.Delay(1000);
                }
            });
        }

        private async Task Launch(object sender, RoutedEventArgs e)
        {
            string savedPath = Globals.CurrentPath;





            // Step 2: Download PrimalCosmetics.pak and .sig
            /*  string paksPath = Path.Combine(savedPath, "FortniteGame", "Content", "Paks");
              if (!Directory.Exists(paksPath))
                  Directory.CreateDirectory(paksPath);

              var cosmeticFiles = new[]
              {
      new {
          FileName = "PrimalCosmetics.pak",
          Url = "https://cdn.discordapp.com/attachments/1343293975473426502/1379142305239531652/PrimalCosmetics.pak?ex=683f29dc&is=683dd85c&hm=7bf4adc303bc1e7789ab1a09206ed29bef729bb965d53c73251e9c74cefc6e01&"
      },
      new {
          FileName = "PrimalCosmetics.sig",
          Url = "https://cdn.discordapp.com/attachments/1343293975473426502/1379142304580767804/PrimalCosmetics.sig?ex=683f29dc&is=683dd85c&hm=fcd85e12be944ce5b0b19e088c59054d2f81e487d028e43fe2d99a6f8a89379f&"
      }
  };

              using (var handler = new HttpClientHandler { AllowAutoRedirect = true })
              using (var client = new HttpClient(handler))
              {
                  client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                  foreach (var file in cosmeticFiles)
                  {
                      string filePath = Path.Combine(paksPath, file.FileName);
                      bool download = false;

                      if (!File.Exists(filePath))
                      {
                          download = true;
                      }
                      else
                      {
                          try
                          {
                              var info = new FileInfo(filePath);
                              if (info.Length < 100 * 1024) // Less than 100 KB, treat as corrupted
                                  download = true;
                              else
                              {
                                  using (var stream = File.OpenRead(filePath)) { } // Try to read
                              }
                          }
                          catch
                          {
                              download = true;
                          }
                      }

                      if (download)
                      {
                          try
                          {
                              using (var response = await client.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead))
                              {
                                  response.EnsureSuccessStatusCode();

                                  using (var stream = await response.Content.ReadAsStreamAsync())
                                  using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                                  {
                                      await stream.CopyToAsync(fs);
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show($"Failed to download {file.FileName}: {ex.Message}");
                              return;
                          }
                      }
                  }
              }

              await Task.Delay(30000);*/

            string exePath = Path.Combine(savedPath, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");
            if (!File.Exists(exePath))
            {
                MessageBox.Show("Please set a valid Fortnite path in settings.");
                return;
            }

            string email = Properties.Settings.Default.Email;
            string password = Properties.Settings.Default.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || email == "NONE" || password == "NONE")
            {
                MessageBox.Show("Please add your Path before launching!");
                return;
            }

            string[] blacklistedProcesses = { "UUUClient", "CheatEngine", "x64dbg", "ProcessHacker" };
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (blacklistedProcesses.Any(name => proc.ProcessName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        return;
                    }
                }
                catch { }
            }

            Process.GetProcessesByName("FortniteClient-Win64-Shipping").ToList().ForEach(proc =>
            {
                try { proc.Kill(); } catch { }
            });

            try
            {
                // Step 1: Download and replace DLL
                string dllUrl = "https://github.com/Swearts/somt/raw/refs/heads/main/Starfall.dll";
                string targetPath = Path.Combine(savedPath, "Engine", "Binaries", "ThirdParty", "NVIDIA", "NVaftermath", "Win64");

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                string targetDllPath1 = Path.Combine(targetPath, "GFSDK_Aftermath_Lib.x64.dll");
                string targetDllPath2 = Path.Combine(targetPath, "GFSDK_Aftermath_Lib.dll");

                using (var client = new System.Net.Http.HttpClient())
                {
                    var dllBytes = await client.GetByteArrayAsync(dllUrl);
                    await File.WriteAllBytesAsync(targetDllPath1, dllBytes);
                    await File.WriteAllBytesAsync(targetDllPath2, dllBytes);
                }
            }

            // Step 2: Download PrimalCosmetics.pak and .sig

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during setup: " + ex.Message);
                return;
            }

            string fnArgs = $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";

            var fnStartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = fnArgs,
                WorkingDirectory = savedPath,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            };

            Process? fnProc = Process.Start(fnStartInfo);
            Process? eacProc = Proc.Start(savedPath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC.exe", fnArgs, suspend: true);
            Process? launcherProc = Proc.Start(savedPath, "FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe", fnArgs, suspend: true);

            if (fnProc != null)
            {
                StartAntiInjectorWatchdog(fnProc);

                if (editOnReleaseEnabled)
                {
                    await DLLInjector.DownloadAndInjectAsync(EOR_DLL_URL, "FortniteClient-Win64-Shipping");
                }

                await Task.Run(() => fnProc.WaitForInputIdle());
                await fnProc.WaitForExitAsync();

                try
                {
                    eacProc?.Kill();
                    launcherProc?.Kill();
                }
                catch
                {
                    MessageBox.Show("There was an error closing the helper processes.");
                }
            }
            else
            {
                MessageBox.Show("Failed to launch Fortnite.");
            }
        }

        private async Task DownloadAndRunEACAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                string basePath = Globals.CurrentPath;
                if (string.IsNullOrEmpty(basePath))
                {
                    MessageBox.Show("Globals.CurrentPath is not set.");
                    return;
                }

                string easyAntiCheatFolderPath = Path.Combine(basePath, FolderName);
                if (!Directory.Exists(easyAntiCheatFolderPath))
                    Directory.CreateDirectory(easyAntiCheatFolderPath);

                using HttpClient client = new();

                // Download Exit_EAC.exe to basePath
                string ExitExePath = Path.Combine(basePath, ExeFileName);
                if (!File.Exists(ExitExePath))
                {
                    var exeData = await client.GetByteArrayAsync(ExitEacUrl);
                    await File.WriteAllBytesAsync(ExitExePath, exeData);
                }

                // Download SplashScreen.png inside EasyAntiCheat
                string imagePath = Path.Combine(easyAntiCheatFolderPath, ImageFileName);
                if (!File.Exists(imagePath))
                {
                    var imageData = await client.GetByteArrayAsync(SplashScreenUrl);
                    await File.WriteAllBytesAsync(imagePath, imageData);
                }

                // Write Settings.json
                string settingsPath = Path.Combine(easyAntiCheatFolderPath, SettingsFileName);
                string settingsJson = @"{
""title"": ""Exit"",
""executable"": ""FortniteGame/Binaries/Win64/FortniteClient-Win64-Shipping.exe"",
""productid"": ""c557c546364948a39015f9b7106e36c0"",
""sandboxid"": ""6d0ee47cd44e43c7a663bc7a3a6156f5"",
""deploymentid"": ""6d0ee47cd44e43c7a663bc7a3a6156f5"",
""requested_splash"": ""EasyAntiCheat/SplashScreen.png"",
""wait_for_game_process_exit"": ""false"",
""hide_bootstrapper"": ""false"",
""hide_gui"": ""false""
}";
                await File.WriteAllTextAsync(settingsPath, settingsJson);

                // Kill EpicGamesLauncher if running
                foreach (var proc in Process.GetProcessesByName("EpicGamesLauncher"))
                {
                    try { proc.Kill(); } catch { }
                }

                // Launch Exit_EAC.exe and wait for exit
                var ExitProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ExitExePath,
                        WorkingDirectory = basePath,
                        UseShellExecute = true
                    }
                };

                ExitProcess.Start();
                await Task.Delay(12000);
                await PrepareAndLaunchFortniteAsync();
                await Task.Run(() => ExitProcess.WaitForExit());

                // After Exit_EAC.exe exits, run Fortnite logic
                //   await PrepareAndLaunchFortniteAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Setup failed: {ex.Message}");
            }
        }


        private async Task PrepareAndLaunchFortniteAsync()
        {
            string savedPath = Globals.CurrentPath;
            string exePath = Path.Combine(savedPath, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");
            if (!File.Exists(exePath))
            {
                MessageBox.Show("Please set a valid Fortnite path in settings.");
                return;
            }

            string email = Properties.Settings.Default.Email;
            string password = Properties.Settings.Default.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || email == "NONE" || password == "NONE")
            {
                MessageBox.Show("Please add your Path before launching!");
                return;
            }

            string[] blacklistedProcesses = { "UUUClient", "CheatEngine", "x64dbg", "ProcessHacker" };
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (blacklistedProcesses.Any(name => proc.ProcessName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        return;
                    }
                }
                catch { }
            }

            Process.GetProcessesByName("FortniteClient-Win64-Shipping").ToList().ForEach(proc =>
            {
                try { proc.Kill(); } catch { }
            });

            try
            {
                // Step 1: Download and replace DLL
                string dllUrl = "https://github.com/sweartsfn/fdsg/releases/download/curl/Starfall.dll";
                string targetPath = Path.Combine(savedPath, "Engine", "Binaries", "ThirdParty", "NVIDIA", "NVaftermath", "Win64");

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                string targetDllPath1 = Path.Combine(targetPath, "GFSDK_Aftermath_Lib.x64.dll");
                string targetDllPath2 = Path.Combine(targetPath, "GFSDK_Aftermath_Lib.dll");

                using (var client = new System.Net.Http.HttpClient())
                {
                    var dllBytes = await client.GetByteArrayAsync(dllUrl);
                    await File.WriteAllBytesAsync(targetDllPath1, dllBytes);
                    await File.WriteAllBytesAsync(targetDllPath2, dllBytes);
                }
            }

            // Step 2: Download PrimalCosmetics.pak and .sig

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during setup: " + ex.Message);
                return;
            }

            string fnArgs = $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";

            var fnStartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = fnArgs,
                WorkingDirectory = savedPath,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            };

            Process? fnProc = Process.Start(fnStartInfo);
            Process? eacProc = Proc.Start(savedPath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC.exe", fnArgs, suspend: true);
            Process? launcherProc = Proc.Start(savedPath, "FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe", fnArgs, suspend: true);

            if (fnProc != null)
            {
                StartAntiInjectorWatchdog(fnProc);

                if (editOnReleaseEnabled)
                {
                    await DLLInjector.DownloadAndInjectAsync(EOR_DLL_URL, "FortniteClient-Win64-Shipping");
                }

                await Task.Run(() => fnProc.WaitForInputIdle());
                await fnProc.WaitForExitAsync();

                try
                {
                    eacProc?.Kill();
                    launcherProc?.Kill();
                }
                catch
                {
                    MessageBox.Show("There was an error closing the helper processes.");
                }
            }
            else
            {
                MessageBox.Show("Failed to launch Fortnite.");
            }
        }



        private void EditOnReleaseCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            editOnReleaseEnabled = true;
        }

        private void EditOnReleaseCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            editOnReleaseEnabled = false;
        }
        private void CheckFortniteRunning(object? sender, EventArgs e)
        {
            // Check if any Fortnite processes are running
            bool fortniteRunning = false;
            foreach (var name in new[] { "FortniteClient-Win64-Shipping", "FortniteClient-Win64-Shipping_EAC", "FortniteLauncher" })
            {
                if (Process.GetProcessesByName(name).Length > 0)
                {
                    fortniteRunning = true;
                    break;
                }
            }

            // If we thought the game was running but no processes are found, reset UI
            if (isGameRunning && !fortniteRunning)
            {
                isGameRunning = false;
                LaunchText.Text = "Launch Season 4";
                LaunchFN.Text = "Launch Fortnite Chapter 2 Season 4 powered by Exit.";
                Launched.Visibility = Visibility.Hidden;
                ErrorloginIcon.Visibility = Visibility.Visible;
            }
        }

        private async void LaunchSeason_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameRunning)
            {
                LaunchText.Text = "Close Exit";
                LaunchFN.Text = "Click here to close Exit. If you're experiencing issues, please restart your computer.";
                Launched.Visibility = Visibility.Visible;
                ErrorloginIcon.Visibility = Visibility.Hidden;

                isGameRunning = true;
                await DownloadAndRunEACAsync(sender, e);
            }
            else
            {
                foreach (var name in new[] { "FortniteClient-Win64-Shipping", "FortniteClient-Win64-Shipping_EAC", "FortniteLauncher" })
                {
                    foreach (var proc in Process.GetProcessesByName(name))
                    {
                        try { proc.Kill(); } catch { }
                    }
                }

                // Update UI here in case closing manually via button
                LaunchText.Text = "Launch Season 4";
                LaunchFN.Text = "Launch Fortnite Chapter 2 Season 4 powered by Exit.";
                Launched.Visibility = Visibility.Hidden;
                ErrorloginIcon.Visibility = Visibility.Visible;
                isGameRunning = false;
            }
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.paypal.com/paypalme/Arx14",
                UseShellExecute = true
            });
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/EAz5NSF3U6",
                UseShellExecute = true
            });
        }

        private void Discord_Click(object sender, RoutedEventArgs e)
        {
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/EAz5NSF3U6",
                    UseShellExecute = true
                });
            }
        }
    }
    }
