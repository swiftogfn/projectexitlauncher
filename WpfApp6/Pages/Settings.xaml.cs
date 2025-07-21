using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace WpfApp5.Pages
{
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent(); // default page for settings navigation
        }
        private void SettingsLoaded(object sender, RoutedEventArgs e)
        {
            AboutToggleButton.Checked += ToggleDropdown;
            AboutToggleButton.Unchecked += ToggleDropdown;
        }
        private void ToggleDropdown(object sender, RoutedEventArgs e)
        {
            bool isChecked = AboutToggleButton.IsChecked == true;

            var rotate = new DoubleAnimation
            {
                To = isChecked ? 180 : 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };
            ArrowRotate.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, rotate);

            if (isChecked)
            {
                Dropping.Visibility = Visibility.Visible;
                AboutDropdownContainer.Visibility = Visibility.Visible;

                var expandAnim = new DoubleAnimation
                {
                    From = 0,
                    To = 130, // Full height of AboutDropdownContainer
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = new QuadraticEase()
                };

                AboutDropdownContainer.BeginAnimation(FrameworkElement.HeightProperty, expandAnim);
            }
            else
            {
                var collapseAnim = new DoubleAnimation
                {
                    From = AboutDropdownContainer.ActualHeight,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new QuadraticEase()
                };

                collapseAnim.Completed += (s, _) =>
                {
                    Dropping.Visibility = Visibility.Collapsed;
                    AboutDropdownContainer.Visibility = Visibility.Collapsed;
                    AboutDropdownContainer.Height = 0; // Reset height so next expand starts from 0
                };

                AboutDropdownContainer.BeginAnimation(FrameworkElement.HeightProperty, collapseAnim);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }


        private void LaunchSeason_Click(object sender, RoutedEventArgs e)
        {
            AboutToggleButton.IsChecked = !(AboutToggleButton.IsChecked ?? false);
            ToggleDropdown(sender, e);
        }

        private void AboutToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {
            // Handled by ToggleDropdown, can be empty or removed
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e) { }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e) { }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default.Loggedin = false;
            Properties.Settings.Default.Save();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new Login());
            }
            else
            {
                System.Windows.MessageBox.Show("MainFrame is null. Navigation failed.");
            }

        }
    }
}
