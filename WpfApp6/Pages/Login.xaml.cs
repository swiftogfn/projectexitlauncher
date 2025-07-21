using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace WpfApp5.Pages
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            AutoLoginSwitch.IsChecked = Properties.Settings.Default.AutoLogin;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0;
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.6),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            this.BeginAnimation(Page.OpacityProperty, fadeIn);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var email = Email.Text.Trim();
            var password = Password.Password;

            try
            {
                // Show loading animation while login in progress
                FadeInLoadingBuffer();

                using (var client = new HttpClient())
                {
                    var loginData = new
                    {
                        email = email,
                        password = password
                    };

                    var jsonContent = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var url = "http://86.25.153.166:3551/api/launcher/login";

                    var response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            using var result = JsonDocument.Parse(responseString);

                            if (result.RootElement.TryGetProperty("username", out var usernameElement))
                            {
                                string username = usernameElement.GetString();

                                // Save credentials to your config/INI
                                AutoLoginSwitch.IsEnabled = false;
                                Properties.Settings.Default.Email = Email.Text;
                                Properties.Settings.Default.Password = Password.Password;
                                Properties.Settings.Default.AutoLogin = AutoLoginSwitch.IsChecked == true;
                                Properties.Settings.Default.Save();

                                SuccessGrid.Visibility = Visibility.Visible;
                                await Task.Delay(1000);

                                // Navigate and pass username to the next page
                                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                                if (mainWindow != null)
                                {
                                    mainWindow.SetLoggedInUsername(username);  // <-- pass username to main window
                                    mainWindow.MainFrame.Navigate(new Navigation(username));  // navigate home with username
                                }
                                else
                                {
                                    await ShowError("Main window not found.");
                                }
                            }
                            else
                            {
                                await ShowError("Login succeeded but response is missing 'username'.");
                            }
                        }
                        catch (Exception parseEx)
                        {
                            await ShowError("Failed to parse login response: " + parseEx.Message);
                        }
                    }
                    else
                    {
                        await ShowError("" + responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowError("The backend is offline.");
            }
            finally
            {
                FadeOutLoadingBuffer();
            }
        }

        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Email = Email.Text;
            Properties.Settings.Default.Save();
        }

        private void Password_TextChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Password = Password.Password;
            Properties.Settings.Default.Save();
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoLogin = true;
            Properties.Settings.Default.Save();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Email))
            {
                Email.Text = Properties.Settings.Default.Email;
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Password))
            {
                Password.Password = Properties.Settings.Default.Password;
            }
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoLogin = false;
            Properties.Settings.Default.Save();

            Email.Text = string.Empty;
            Password.Password = string.Empty;
        }

        private async Task ShowError(string message)
        {
            ErrorGrid.Visibility = Visibility.Visible;
            ErrorButton.Content = message;

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };
            ErrorGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            await Task.Delay(3000);

            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1)
            };
            ErrorGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(1000);
            ErrorGrid.Visibility = Visibility.Collapsed;
        }

        public void FadeInLoadingBuffer()
        {
            LoadingGrid.Visibility = Visibility.Visible;

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            Storyboard.SetTarget(fadeInAnimation, LoadingGrid);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(OpacityProperty));

            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeInAnimation);

            storyboard.Begin();
        }

        public void FadeOutLoadingBuffer()
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            Storyboard.SetTarget(fadeOutAnimation, LoadingGrid);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(OpacityProperty));

            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeOutAnimation);

            storyboard.Completed += (s, e) =>
            {
                LoadingGrid.Visibility = Visibility.Hidden;
            };

            storyboard.Begin();
        }
    }
}
